using Domain.Entities;
using Domain.Interfaces;
using Domain.Interfaces.Services;

namespace Infrastructure.Services;

/// <summary>
/// Location service for geographic operations
/// </summary>
public class LocationService : ILocationService
{
    private readonly IUnitOfWork _unitOfWork;
    private const double EarthRadiusKm = 6371;

    public LocationService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
    }

    public double CalculateDistance(decimal lat1, decimal lon1, decimal lat2, decimal lon2)
    {
        var dLat = DegreesToRadians((double)(lat2 - lat1));
        var dLon = DegreesToRadians((double)(lon2 - lon1));

        var latitude1 = DegreesToRadians((double)lat1);
        var latitude2 = DegreesToRadians((double)lat2);

        var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                Math.Sin(dLon / 2) * Math.Sin(dLon / 2) * Math.Cos(latitude1) * Math.Cos(latitude2);

        var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

        return EarthRadiusKm * c;
    }

    public async Task<IEnumerable<User>> FindUsersNearbyAsync(
        decimal latitude,
        decimal longitude,
        int maxDistanceKm,
        CancellationToken cancellationToken = default)
    {
        return await _unitOfWork.Users.GetUsersWithinDistanceAsync(
            latitude,
            longitude,
            maxDistanceKm,
            cancellationToken);
    }

    private static double DegreesToRadians(double degrees)
    {
        return degrees * Math.PI / 180;
    }
}