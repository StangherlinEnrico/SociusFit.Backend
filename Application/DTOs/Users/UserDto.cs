namespace Application.DTOs.Users;

/// <summary>
/// DTO for user response
/// </summary>
public class UserDto
{
    public int Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string FullName => $"{FirstName} {LastName}";
    public string Email { get; set; } = string.Empty;
    public bool IsEmailVerified { get; set; }
    public string? Provider { get; set; }
    public string? Location { get; set; }
    public int? MaxDistance { get; set; }
    public DateTime CreatedAt { get; set; }
}