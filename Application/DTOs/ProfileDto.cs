namespace Application.DTOs;

public class ProfileDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public int Age { get; set; }
    public string Gender { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string Bio { get; set; } = string.Empty;
    public string? PhotoUrl { get; set; }
    public List<SportDto> Sports { get; set; } = new();
    public DateTime CreatedAt { get; set; }
    public bool IsComplete { get; set; }
}