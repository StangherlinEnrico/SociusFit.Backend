using Domain.Constants;
using Domain.Exceptions;

namespace Domain.ValueObjects;

public class City
{
    public string Value { get; private set; }

    private City() { }

    public City(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new InvalidCityException("City cannot be empty");

        if (value.Length < ProfileConstants.MinCityLength)
            throw new InvalidCityException($"City must be at least {ProfileConstants.MinCityLength} characters long");

        if (value.Length > ProfileConstants.MaxCityLength)
            throw new InvalidCityException($"City cannot exceed {ProfileConstants.MaxCityLength} characters");

        Value = value.Trim();
    }

    public static implicit operator string(City city) => city.Value;
    public static explicit operator City(string value) => new City(value);

    public override bool Equals(object? obj)
    {
        if (obj is City other)
            return Value == other.Value;
        return false;
    }

    public override int GetHashCode() => Value.GetHashCode();

    public override string ToString() => Value;
}