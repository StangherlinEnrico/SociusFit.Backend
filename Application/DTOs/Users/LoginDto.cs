namespace Application.DTOs.Users;

/// <summary>
/// DTO for user authentication
/// </summary>
public class LoginDto
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}
