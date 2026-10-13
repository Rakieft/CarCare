using System.ComponentModel.DataAnnotations;

namespace CarCare.Models;

/// <summary>A service or repair done on a vehicle, optionally with when the next one is due.</summary>
public class MaintenanceRecord
{
    public int Id { get; set; }

    public int VehicleId { get; set; }

    [Required(ErrorMessage = "Enter the type of service (e.g. Oil change).")]
    [StringLength(60)]
    public string Type { get; set; } = string.Empty;

    [StringLength(300)]
    public string? Description { get; set; }

    [Required]
    public DateTime Date { get; set; } = DateTime.Today;

    [Range(0, 2_000_000, ErrorMessage = "Enter a valid odometer reading.")]
    public int Odometer { get; set; }

    [Range(0, 100_000, ErrorMessage = "Enter a valid cost.")]
    public decimal Cost { get; set; }

    /// <summary>Optional: date when this service is next due (used for reminders).</summary>
    public DateTime? NextServiceDate { get; set; }

    /// <summary>Optional: odometer reading when this service is next due (used for reminders).</summary>
    [Range(0, 2_000_000, ErrorMessage = "Enter a valid odometer reading.")]
    public int? NextServiceOdometer { get; set; }
}
