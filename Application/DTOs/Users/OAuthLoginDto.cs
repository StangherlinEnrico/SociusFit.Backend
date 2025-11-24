namespace Application.DTOs.Users;

/// <summary>
/// DTO for OAuth authentication
/// </summary>
public class OAuthLoginDto
{
    /// <summary>
    /// OAuth provider (google, apple)
    /// </summary>
    public string Provider { get; set; } = string.Empty;

    /// <summary>
    /// OAuth token (ID token for Google, identity token for Apple)
    /// </summary>
    public string Token { get; set; } = string.Empty;

    /// <summary>
    /// First name (optional, used for Apple Sign-In first login)
    /// </summary>
    public string? FirstName { get; set; }

    /// <summary>
    /// Last name (optional, used for Apple Sign-In first login)
    /// </summary>
    public string? LastName { get; set; }
}