namespace Application.DTOs;

public class DiscoveryCardDto
{
    public Guid UserId { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public int Age { get; set; }
    public string City { get; set; } = string.Empty;
    public string? PhotoUrl { get; set; }
    public string Bio { get; set; } = string.Empty;
    public double DistanceKm { get; set; }
    public List<SportInfoDto> Sports { get; set; } = new();
}

public class SportInfoDto
{
    public Guid SportId { get; set; }
    public string SportName { get; set; } = string.Empty;
    public string Level { get; set; } = string.Empty;
}