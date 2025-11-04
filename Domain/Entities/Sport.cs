namespace Domain.Entities;

/// <summary>
/// Represents a sport in the system
/// </summary>
public class Sport
{
    public int Id { get; private set; }
    public string Name { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    // Navigation property
    private readonly List<UserSport> _userSports = new();
    public IReadOnlyCollection<UserSport> UserSports => _userSports.AsReadOnly();

    // Private constructor for EF Core
    private Sport() { }

    public Sport(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Sport name cannot be empty", nameof(name));

        Name = name;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Sport name cannot be empty", nameof(name));

        Name = name;
        UpdatedAt = DateTime.UtcNow;
    }
}