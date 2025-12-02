using Domain.Entities;
using Domain.Services;

namespace Domain.Specifications;

/// <summary>
/// Specifica per trovare profili entro una certa distanza da coordinate specifiche
/// </summary>
public class ProfilesWithinDistanceSpecification : BaseSpecification<Profile>
{
    public ProfilesWithinDistanceSpecification(double latitude, double longitude, int maxDistanceKm)
        : base(p => CalculateDistance(p.Latitude, p.Longitude, latitude, longitude) <= maxDistanceKm)
    {
    }

    private static double CalculateDistance(double lat1, double lon1, double lat2, double lon2)
    {
        return DistanceCalculator.CalculateDistanceKm(lat1, lon1, lat2, lon2);
    }
}

/// <summary>
/// Specifica per trovare profili entro la distanza massima di un altro profilo
/// </summary>
public class ProfilesWithinUserRangeSpecification : BaseSpecification<Profile>
{
    public ProfilesWithinUserRangeSpecification(Profile userProfile)
        : base(p =>
            p.Id != userProfile.Id && // Escludi il profilo stesso
            CalculateDistance(
                p.Latitude,
                p.Longitude,
                userProfile.Latitude,
                userProfile.Longitude) <= userProfile.MaxDistance)
    {
    }

    private static double CalculateDistance(double lat1, double lon1, double lat2, double lon2)
    {
        return DistanceCalculator.CalculateDistanceKm(lat1, lon1, lat2, lon2);
    }
}