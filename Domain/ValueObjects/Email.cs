namespace Domain.ValueObjects;

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