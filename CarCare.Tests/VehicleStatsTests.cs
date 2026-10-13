using CarCare.Models;
using CarCare.Services;

namespace CarCare.Tests;

public class VehicleStatsTests
{
    private static readonly DateTime Today = new(2026, 10, 13);

    [Fact]
    public void CurrentOdometer_UsesHighestKnownReading()
    {
        var vehicle = new Vehicle { InitialOdometerReading = 10_000 };
        var fuel = new[] { new FuelRecord { Odometer = 10_400 } };
        var maintenance = new[] { new MaintenanceRecord { Type = "Oil", Odometer = 10_900 } };

        Assert.Equal(10_900, VehicleStats.CurrentOdometer(vehicle, fuel, maintenance));
    }

    [Fact]
    public void CurrentOdometer_FallsBackToInitialReading()
    {
        var vehicle = new Vehicle { InitialOdometerReading = 42_000 };

        Assert.Equal(42_000, VehicleStats.CurrentOdometer(vehicle, [], []));
    }

    [Fact]
    public void AverageMpg_IsNullWithFewerThanTwoFillUps()
    {
        Assert.Null(VehicleStats.AverageMpg([]));
        Assert.Null(VehicleStats.AverageMpg([new FuelRecord { Odometer = 1000, Gallons = 10 }]));
    }

    [Fact]
    public void AverageMpg_UsesMilesBetweenFirstAndLastOverGallonsAfterFirst()
    {
        var fuel = new[]
        {
            new FuelRecord { Odometer = 1_000, Gallons = 10 },
            new FuelRecord { Odometer = 1_300, Gallons = 10 },
            new FuelRecord { Odometer = 1_600, Gallons = 10 }
        };

        // 600 miles / 20 gallons (the first fill-up's gallons are not counted)
        Assert.Equal(30.0m, VehicleStats.AverageMpg(fuel));
    }

    [Fact]
    public void Reminders_OverdueWhenDateHasPassed()
    {
        var records = new[]
        {
            new MaintenanceRecord { Type = "Oil change", Date = Today.AddDays(-200), NextServiceDate = Today.AddDays(-1) }
        };

        var reminder = Assert.Single(VehicleStats.GetReminders(records, 10_000, Today));
        Assert.Equal(ReminderStatus.Overdue, reminder.Status);
    }

    [Fact]
    public void Reminders_DueSoonWhenOdometerIsClose()
    {
        var records = new[]
        {
            new MaintenanceRecord { Type = "Tires", Date = Today.AddDays(-10), NextServiceOdometer = 10_300 }
        };

        var reminder = Assert.Single(VehicleStats.GetReminders(records, 10_000, Today));
        Assert.Equal(ReminderStatus.DueSoon, reminder.Status);
    }

    [Fact]
    public void Reminders_OverdueWhenOdometerReached()
    {
        var records = new[]
        {
            new MaintenanceRecord { Type = "Brakes", Date = Today.AddDays(-10), NextServiceOdometer = 9_500 }
        };

        var reminder = Assert.Single(VehicleStats.GetReminders(records, 10_000, Today));
        Assert.Equal(ReminderStatus.Overdue, reminder.Status);
    }

    [Fact]
    public void Reminders_OnlyLatestRecordPerTypeCounts()
    {
        var records = new[]
        {
            // Old oil change was overdue, but a newer one replaced it.
            new MaintenanceRecord { Type = "Oil change", Date = Today.AddDays(-400), NextServiceDate = Today.AddDays(-220) },
            new MaintenanceRecord { Type = "oil change", Date = Today.AddDays(-5), NextServiceDate = Today.AddDays(180) }
        };

        var reminder = Assert.Single(VehicleStats.GetReminders(records, 10_000, Today));
        Assert.Equal(ReminderStatus.Upcoming, reminder.Status);
    }

    [Fact]
    public void Reminders_IgnoreRecordsWithoutNextService()
    {
        var records = new[] { new MaintenanceRecord { Type = "Wash", Date = Today } };

        Assert.Empty(VehicleStats.GetReminders(records, 10_000, Today));
    }

    [Fact]
    public void Reminders_AreSortedMostUrgentFirst()
    {
        var records = new[]
        {
            new MaintenanceRecord { Type = "A", Date = Today, NextServiceDate = Today.AddDays(200) },
            new MaintenanceRecord { Type = "B", Date = Today, NextServiceDate = Today.AddDays(-2) },
            new MaintenanceRecord { Type = "C", Date = Today, NextServiceDate = Today.AddDays(10) }
        };

        var statuses = VehicleStats.GetReminders(records, 0, Today).Select(r => r.Type).ToArray();
        Assert.Equal(["B", "C", "A"], statuses);
    }
}
