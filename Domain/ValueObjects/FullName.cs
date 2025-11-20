namespace Domain.ValueObjects;

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
