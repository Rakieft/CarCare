using System.ComponentModel.DataAnnotations;

namespace CarCare.Models;

/// <summary>
/// The primary vehicle a user registers during onboarding.
/// Each user owns at most one vehicle in this version of the app.
/// </summary>
public class Vehicle
{
    public int Id { get; set; }

    /// <summary>Id of the ASP.NET Identity user who owns this vehicle.</summary>
    [Required]
    public string UserId { get; set; } = string.Empty;

    [Required, StringLength(50)]
    public string Make { get; set; } = string.Empty;

    [Required, StringLength(50)]
    public string Model { get; set; } = string.Empty;

    [Range(1900, 2100)]
    public int Year { get; set; }

    [Range(0, 2_000_000)]
    public int InitialOdometerReading { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
