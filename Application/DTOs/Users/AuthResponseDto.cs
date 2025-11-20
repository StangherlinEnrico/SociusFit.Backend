namespace Application.DTOs.Users;

/// <summary>
/// DTO for authentication response
/// </summary>
public class AuthResponseDto
{
    public string Token { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
    public UserDto User { get; set; } = null!;
    public string? Message { get; set; }
}
