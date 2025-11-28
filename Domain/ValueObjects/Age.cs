using Domain.Constants;
using Domain.Exceptions;

namespace Domain.ValueObjects;

public class Age
{
    public int Value { get; private set; }

    private Age() { }

    public Age(int value)
    {
        if (value < ProfileConstants.MinAge)
            throw new InvalidAgeException($"Age must be at least {ProfileConstants.MinAge} years old");

        if (value > ProfileConstants.MaxAge)
            throw new InvalidAgeException($"Age cannot exceed {ProfileConstants.MaxAge} years");

        Value = value;
    }

    public static implicit operator int(Age age) => age.Value;
    public static explicit operator Age(int value) => new Age(value);

    public override bool Equals(object? obj)
    {
        if (obj is Age other)
            return Value == other.Value;
        return false;
    }

    public override int GetHashCode() => Value.GetHashCode();

    public override string ToString() => Value.ToString();
}