using Domain.Enums;

namespace Domain.Entities;

public class Profile
{
    public Guid Id { get; private set; }
    public Guid UserId { get; private set; }
    public int Age { get; private set; }
    public string Gender { get; private set; }
    public string City { get; private set; }
    public string Bio { get; private set; }
    public string? PhotoUrl { get; private set; }
    public int MaxDistance { get; private set; } // Distanza massima in km
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }

    private readonly List<ProfileSport> _profileSports = new();
    public IReadOnlyCollection<ProfileSport> ProfileSports => _profileSports.AsReadOnly();

    private Profile() { }

    public Profile(Guid userId, int age, string gender, string city, string bio, int maxDistance = Constants.ProfileConstants.DefaultMaxDistance)
    {
        Id = Guid.NewGuid();
        UserId = userId;
        Age = age;
        Gender = gender;
        City = city;
        Bio = bio;
        MaxDistance = maxDistance;
        CreatedAt = DateTime.UtcNow;
    }

    public void UpdateBasicInfo(int age, string gender, string city, string bio)
    {
        Age = age;
        Gender = gender;
        City = city;
        Bio = bio;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateMaxDistance(int maxDistance)
    {
        MaxDistance = maxDistance;
        UpdatedAt = DateTime.UtcNow;
    }

    public void SetPhotoUrl(string photoUrl)
    {
        PhotoUrl = photoUrl;
        UpdatedAt = DateTime.UtcNow;
    }

    public void AddSport(Guid sportId, SportLevel level)
    {
        if (!_profileSports.Any(ps => ps.SportId == sportId))
        {
            var profileSport = new ProfileSport(Id, sportId, level);
            _profileSports.Add(profileSport);
            UpdatedAt = DateTime.UtcNow;
        }
    }

    public void UpdateSportLevel(Guid sportId, SportLevel level)
    {
        var profileSport = _profileSports.FirstOrDefault(ps => ps.SportId == sportId);
        if (profileSport != null)
        {
            profileSport.UpdateLevel(level);
            UpdatedAt = DateTime.UtcNow;
        }
    }

    public void RemoveSport(Guid sportId)
    {
        var profileSport = _profileSports.FirstOrDefault(ps => ps.SportId == sportId);
        if (profileSport != null)
        {
            _profileSports.Remove(profileSport);
            UpdatedAt = DateTime.UtcNow;
        }
    }

    public void ClearSports()
    {
        _profileSports.Clear();
        UpdatedAt = DateTime.UtcNow;
    }

    public bool IsComplete()
    {
        return Age >= 18
               && !string.IsNullOrWhiteSpace(Gender)
               && !string.IsNullOrWhiteSpace(City)
               && !string.IsNullOrWhiteSpace(Bio)
               && _profileSports.Any()
               && !string.IsNullOrWhiteSpace(PhotoUrl);
    }
}