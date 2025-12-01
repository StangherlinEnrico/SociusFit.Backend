using Domain.Enums;

namespace Domain.Entities;

public class ProfileSport
{
    public Guid ProfileId { get; private set; }
    public Guid SportId { get; private set; }
    public SportLevel Level { get; private set; }
    public DateTime CreatedAt { get; private set; }

    // Navigation properties
    public Profile Profile { get; private set; } = null!;
    public Sport Sport { get; private set; } = null!;

    private ProfileSport() { }

    public ProfileSport(Guid profileId, Guid sportId, SportLevel level)
    {
        ProfileId = profileId;
        SportId = sportId;
        Level = level;
        CreatedAt = DateTime.UtcNow;
    }

    public void UpdateLevel(SportLevel level)
    {
        Level = level;
    }
}