using Domain.Entities;

namespace Domain.Interfaces.Services;

/// <summary>
/// Service for location-based operations
/// </summary>
public interface ILocationService
{
    double CalculateDistance(decimal lat1, decimal lon1, decimal lat2, decimal lon2);
    Task<IEnumerable<User>> FindUsersNearbyAsync(
        decimal latitude,
        decimal longitude,
        int maxDistanceKm,
        CancellationToken cancellationToken = default);
}
