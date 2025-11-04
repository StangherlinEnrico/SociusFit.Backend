namespace Domain.Entities;

/// <summary>
/// Represents a skill level in the system
/// </summary>
public class Level
{
    public int Id { get; private set; }
    public string Name { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    // Navigation property
    private readonly List<UserSport> _userSports = new();
    public IReadOnlyCollection<UserSport> UserSports => _userSports.AsReadOnly();

    // Private constructor for EF Core
    private Level() { }

    public Level(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Level name cannot be empty", nameof(name));

        Name = name;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Level name cannot be empty", nameof(name));

        Name = name;
        UpdatedAt = DateTime.UtcNow;
    }
}