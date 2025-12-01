using Domain.Enums;

namespace Application.DTOs;

public class ProfileSportDto
{
    public Guid SportId { get; set; }
    public string SportName { get; set; } = string.Empty;
    public SportLevel Level { get; set; }
}