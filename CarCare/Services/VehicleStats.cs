using CarCare.Models;

namespace CarCare.Services;

public enum ReminderStatus
{
    Upcoming,
    DueSoon,
    Overdue
}

/// <summary>A maintenance reminder derived from the latest record of each service type.</summary>
public record Reminder(string Type, string Detail, ReminderStatus Status);

/// <summary>Pure calculation helpers for the dashboard (no database access, so they are easy to test).</summary>
public static class VehicleStats
{
    /// <summary>A reminder becomes "due soon" within this many days of its date...</summary>
    public const int DueSoonDays = 30;

    /// <summary>...or within this many miles of its odometer target.</summary>
    public const int DueSoonMiles = 500;

    /// <summary>The highest odometer value known for the vehicle.</summary>
    public static int CurrentOdometer(
        Vehicle vehicle,
        IEnumerable<FuelRecord> fuel,
        IEnumerable<MaintenanceRecord> maintenance)
    {
        var max = vehicle.InitialOdometerReading;
        foreach (var f in fuel) max = Math.Max(max, f.Odometer);
        foreach (var m in maintenance) max = Math.Max(max, m.Odometer);
        return max;
    }

    /// <summary>
    /// Average miles per gallon using consecutive fill-ups: miles driven between the first and last
    /// fill-up divided by the gallons bought after the first one. Null when there is not enough data.
    /// </summary>
    public static decimal? AverageMpg(IEnumerable<FuelRecord> fuel)
    {
        var ordered = fuel.OrderBy(f => f.Odometer).ToList();
        if (ordered.Count < 2)
        {
            return null;
        }

        var miles = ordered[^1].Odometer - ordered[0].Odometer;
        var gallons = ordered.Skip(1).Sum(f => f.Gallons);
        if (miles <= 0 || gallons <= 0)
        {
            return null;
        }

        return Math.Round(miles / gallons, 1);
    }

    /// <summary>
    /// Builds reminders from the most recent record of each service type that has a next date or
    /// next odometer. Overdue reminders come first.
    /// </summary>
    public static List<Reminder> GetReminders(
        IEnumerable<MaintenanceRecord> maintenance,
        int currentOdometer,
        DateTime today)
    {
        var reminders = new List<Reminder>();

        var latestPerType = maintenance
            .Where(m => m.NextServiceDate is not null || m.NextServiceOdometer is not null)
            .GroupBy(m => m.Type.Trim().ToLowerInvariant())
            .Select(g => g.OrderByDescending(m => m.Date).ThenByDescending(m => m.Odometer).First());

        foreach (var m in latestPerType)
        {
            var status = ReminderStatus.Upcoming;
            var parts = new List<string>();

            if (m.NextServiceDate is { } dueDate)
            {
                parts.Add($"by {dueDate:MMM d, yyyy}");
                if (dueDate.Date < today.Date)
                {
                    status = ReminderStatus.Overdue;
                }
                else if (dueDate.Date <= today.Date.AddDays(DueSoonDays))
                {
                    status = Max(status, ReminderStatus.DueSoon);
                }
            }

            if (m.NextServiceOdometer is { } dueMiles)
            {
                parts.Add($"at {dueMiles:N0} mi");
                if (currentOdometer >= dueMiles)
                {
                    status = ReminderStatus.Overdue;
                }
                else if (dueMiles - currentOdometer <= DueSoonMiles)
                {
                    status = Max(status, ReminderStatus.DueSoon);
                }
            }

            reminders.Add(new Reminder(m.Type, "Due " + string.Join(" or ", parts), status));
        }

        return reminders
            .OrderByDescending(r => r.Status)
            .ThenBy(r => r.Type)
            .ToList();
    }

    private static ReminderStatus Max(ReminderStatus a, ReminderStatus b) => a > b ? a : b;
}
