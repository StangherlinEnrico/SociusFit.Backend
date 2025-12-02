namespace Application.DTOs;

public class ProfileDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public int Age { get; set; }
    public string Gender { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public double Latitude { get; set; } // Calcolato dal backend
    public double Longitude { get; set; } // Calcolato dal backend
    public string Bio { get; set; } = string.Empty;
    public string? PhotoUrl { get; set; }
    public int MaxDistance { get; set; }
    public List<ProfileSportDto> Sports { get; set; } = new();
    public DateTime CreatedAt { get; set; }
    public bool IsComplete { get; set; }
}