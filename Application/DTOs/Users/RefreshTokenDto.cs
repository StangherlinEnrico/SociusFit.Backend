namespace Application.DTOs.Users;

/// <summary>
/// DTO for refresh token requests
/// </summary>
public class RefreshTokenDto
{
    public string RefreshToken { get; set; } = string.Empty;
}