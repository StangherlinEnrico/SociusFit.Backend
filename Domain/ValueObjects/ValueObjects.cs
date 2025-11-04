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

/// <summary>
/// Value object representing a full name
/// </summary>
public sealed class FullName : IEquatable<FullName>
{
    public string FirstName { get; }
    public string LastName { get; }
    public string FullNameValue => $"{FirstName} {LastName}";

    public FullName(string firstName, string lastName)
    {
        if (string.IsNullOrWhiteSpace(firstName))
            throw new ArgumentException("First name cannot be empty", nameof(firstName));

        if (string.IsNullOrWhiteSpace(lastName))
            throw new ArgumentException("Last name cannot be empty", nameof(lastName));

        FirstName = firstName.Trim();
        LastName = lastName.Trim();
    }

    public bool Equals(FullName? other)
    {
        if (other is null) return false;
        return FirstName.Equals(other.FirstName, StringComparison.OrdinalIgnoreCase) &&
               LastName.Equals(other.LastName, StringComparison.OrdinalIgnoreCase);
    }

    public override bool Equals(object? obj) => Equals(obj as FullName);

    public override int GetHashCode() =>
        HashCode.Combine(
            FirstName.ToLowerInvariant(),
            LastName.ToLowerInvariant());

    public static bool operator ==(FullName? left, FullName? right) =>
        left?.Equals(right) ?? right is null;

    public static bool operator !=(FullName? left, FullName? right) => !(left == right);

    public override string ToString() => FullNameValue;
}

/// <summary>
/// Value object representing an email address
/// </summary>
public sealed class Email : IEquatable<Email>
{
    public string Value { get; }

    public Email(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Email cannot be empty", nameof(value));

        if (!IsValidEmail(value))
            throw new ArgumentException("Invalid email format", nameof(value));

        Value = value.ToLowerInvariant().Trim();
    }

    private static bool IsValidEmail(string email)
    {
        try
        {
            var addr = new System.Net.Mail.MailAddress(email);
            return addr.Address == email;
        }
        catch
        {
            return false;
        }
    }

    public bool Equals(Email? other)
    {
        if (other is null) return false;
        return Value.Equals(other.Value, StringComparison.OrdinalIgnoreCase);
    }

    public override bool Equals(object? obj) => Equals(obj as Email);

    public override int GetHashCode() => Value.GetHashCode();

    public static bool operator ==(Email? left, Email? right) =>
        left?.Equals(right) ?? right is null;

    public static bool operator !=(Email? left, Email? right) => !(left == right);

    public override string ToString() => Value;

    public static implicit operator string(Email email) => email.Value;
}