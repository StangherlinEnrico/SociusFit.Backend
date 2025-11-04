namespace Domain.Entities;

/// <summary>
/// Represents the relationship between a user, sport, and skill level
/// </summary>
public class UserSport
{
    public int Id { get; private set; }
    public int UserId { get; private set; }
    public int SportId { get; private set; }
    public int LevelId { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    // Navigation properties
    public User User { get; private set; } = null!;
    public Sport Sport { get; private set; } = null!;
    public Level Level { get; private set; } = null!;

    // Private constructor for EF Core
    private UserSport() { }

    public UserSport(int userId, int sportId, int levelId)
    {
        if (userId <= 0)
            throw new ArgumentException("User ID must be positive", nameof(userId));

        if (sportId <= 0)
            throw new ArgumentException("Sport ID must be positive", nameof(sportId));

        if (levelId <= 0)
            throw new ArgumentException("Level ID must be positive", nameof(levelId));

        UserId = userId;
        SportId = sportId;
        LevelId = levelId;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateLevel(int levelId)
    {
        if (levelId <= 0)
            throw new ArgumentException("Level ID must be positive", nameof(levelId));

        LevelId = levelId;
        UpdatedAt = DateTime.UtcNow;
    }
}