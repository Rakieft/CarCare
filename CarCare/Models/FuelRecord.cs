using System.ComponentModel.DataAnnotations;

namespace CarCare.Models;

/// <summary>One fuel fill-up for a vehicle.</summary>
public class FuelRecord
{
    public int Id { get; set; }

    public int VehicleId { get; set; }

    [Required]
    public DateTime Date { get; set; } = DateTime.Today;

    /// <summary>Odometer reading (miles) at the time of the fill-up.</summary>
    [Range(0, 2_000_000, ErrorMessage = "Enter a valid odometer reading.")]
    public int Odometer { get; set; }

    [Range(0.1, 500, ErrorMessage = "Gallons must be between 0.1 and 500.")]
    public decimal Gallons { get; set; }

    [Range(0, 10_000, ErrorMessage = "Enter a valid cost.")]
    public decimal Cost { get; set; }
}
