using System.Security.Claims;
using CarCare.Data;
using CarCare.Models;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.EntityFrameworkCore;

namespace CarCare.Services;

/// <summary>Finds the vehicle that belongs to the signed-in user.</summary>
public class CurrentVehicleService(ApplicationDbContext db, AuthenticationStateProvider authStateProvider)
{
    public async Task<Vehicle?> GetAsync()
    {
        var state = await authStateProvider.GetAuthenticationStateAsync();
        var userId = state.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userId is null)
        {
            return null;
        }

        return await db.Vehicles.AsNoTracking().FirstOrDefaultAsync(v => v.UserId == userId);
    }
}
