using Domain.Constants;
using Domain.Exceptions;

namespace Domain.ValueObjects;

public class Bio
{
    public string Value { get; private set; }

    private Bio() { }

    public Bio(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new InvalidBioException("Bio cannot be empty");

        if (value.Length < ProfileConstants.MinBioLength)
            throw new InvalidBioException($"Bio must be at least {ProfileConstants.MinBioLength} characters long");

        if (value.Length > ProfileConstants.MaxBioLength)
            throw new InvalidBioException($"Bio cannot exceed {ProfileConstants.MaxBioLength} characters");

        Value = value.Trim();
    }

    public static implicit operator string(Bio bio) => bio.Value;
    public static explicit operator Bio(string value) => new Bio(value);

    public override bool Equals(object? obj)
    {
        if (obj is Bio other)
            return Value == other.Value;
        return false;
    }

    public override int GetHashCode() => Value.GetHashCode();

    public override string ToString() => Value;
}