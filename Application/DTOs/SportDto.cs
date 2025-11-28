using Domain.Enums;

namespace Application.DTOs;

public class SportDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public SportLevel Level { get; set; }
}