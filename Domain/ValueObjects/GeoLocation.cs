namespace Domain.ValueObjects;

/// <summary>
/// Value object representing geographic coordinates
/// </summary>
public sealed class GeoLocation : IEquatable<GeoLocation>
{
    public decimal Latitude { get; }
    public decimal Longitude { get; }

    public GeoLocation(decimal latitude, decimal longitude)
    {
        if (latitude < -90 || latitude > 90)
            throw new ArgumentException("Latitude must be between -90 and 90", nameof(latitude));

        if (longitude < -180 || longitude > 180)
            throw new ArgumentException("Longitude must be between -180 and 180", nameof(longitude));

        Latitude = latitude;
        Longitude = longitude;
    }

    /// <summary>
    /// Calculate distance to another location using Haversine formula (in kilometers)
    /// </summary>
    public double DistanceTo(GeoLocation other)
    {
        const double earthRadiusKm = 6371;

        var dLat = DegreesToRadians((double)(other.Latitude - Latitude));
        var dLon = DegreesToRadians((double)(other.Longitude - Longitude));

        var lat1 = DegreesToRadians((double)Latitude);
        var lat2 = DegreesToRadians((double)other.Latitude);

        var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                Math.Sin(dLon / 2) * Math.Sin(dLon / 2) * Math.Cos(lat1) * Math.Cos(lat2);
        var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

        return earthRadiusKm * c;
    }

    private static double DegreesToRadians(double degrees) => degrees * Math.PI / 180;

    public bool Equals(GeoLocation? other)
    {
        if (other is null) return false;
        return Latitude == other.Latitude && Longitude == other.Longitude;
    }

    public override bool Equals(object? obj) => Equals(obj as GeoLocation);

    public override int GetHashCode() => HashCode.Combine(Latitude, Longitude);

    public static bool operator ==(GeoLocation? left, GeoLocation? right) =>
        left?.Equals(right) ?? right is null;

    public static bool operator !=(GeoLocation? left, GeoLocation? right) => !(left == right);
}
