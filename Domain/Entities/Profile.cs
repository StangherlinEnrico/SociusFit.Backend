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
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }

    private readonly List<Sport> _sports = new();
    public IReadOnlyCollection<Sport> Sports => _sports.AsReadOnly();

    private Profile() { }

    public Profile(Guid userId, int age, string gender, string city, string bio)
    {
        Id = Guid.NewGuid();
        UserId = userId;
        Age = age;
        Gender = gender;
        City = city;
        Bio = bio;
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

    public void SetPhotoUrl(string photoUrl)
    {
        PhotoUrl = photoUrl;
        UpdatedAt = DateTime.UtcNow;
    }

    public void AddSport(Sport sport)
    {
        if (!_sports.Any(s => s.Id == sport.Id))
        {
            _sports.Add(sport);
            UpdatedAt = DateTime.UtcNow;
        }
    }

    public void RemoveSport(Guid sportId)
    {
        var sport = _sports.FirstOrDefault(s => s.Id == sportId);
        if (sport != null)
        {
            _sports.Remove(sport);
            UpdatedAt = DateTime.UtcNow;
        }
    }

    public void ClearSports()
    {
        _sports.Clear();
        UpdatedAt = DateTime.UtcNow;
    }

    public bool IsComplete()
    {
        return Age >= 18
               && !string.IsNullOrWhiteSpace(Gender)
               && !string.IsNullOrWhiteSpace(City)
               && !string.IsNullOrWhiteSpace(Bio)
               && _sports.Any()
               && !string.IsNullOrWhiteSpace(PhotoUrl);
    }
}