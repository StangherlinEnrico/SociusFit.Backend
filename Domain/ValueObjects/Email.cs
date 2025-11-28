using System.Text.RegularExpressions;
using Domain.Exceptions;

namespace Domain.ValueObjects;

public class Email
{
    public string Value { get; private set; }

    private static readonly Regex EmailRegex = new Regex(
        @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$",
        RegexOptions.Compiled | RegexOptions.IgnoreCase
    );

    private Email() { }

    public Email(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new InvalidEmailException("Email cannot be empty");

        value = value.Trim().ToLowerInvariant();

        if (!EmailRegex.IsMatch(value))
            throw new InvalidEmailException($"Invalid email format: {value}");

        Value = value;
    }

    public static implicit operator string(Email email) => email.Value;
    public static explicit operator Email(string value) => new Email(value);

    public override bool Equals(object? obj)
    {
        if (obj is Email other)
            return Value == other.Value;
        return false;
    }

    public override int GetHashCode() => Value.GetHashCode();

    public override string ToString() => Value;
}