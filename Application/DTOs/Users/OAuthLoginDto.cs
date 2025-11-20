namespace Application.DTOs.Users;

/// <summary>
/// DTO for OAuth authentication
/// </summary>
public class OAuthLoginDto
{
    public string Provider { get; set; } = string.Empty;
    public string Token { get; set; } = string.Empty;
}
