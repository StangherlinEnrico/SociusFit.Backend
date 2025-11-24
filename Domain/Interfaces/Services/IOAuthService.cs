namespace Domain.Interfaces.Services;

/// <summary>
/// Service for OAuth authentication with external providers
/// </summary>
public interface IOAuthService
{
    /// <summary>
    /// Validates an OAuth token and returns user information
    /// </summary>
    /// <param name="provider">OAuth provider (google, apple)</param>
    /// <param name="token">The OAuth token to validate</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>User information if valid, null otherwise</returns>
    Task<OAuthUserInfo?> ValidateTokenAsync(
        string provider,
        string token,
        CancellationToken cancellationToken = default);
}

/// <summary>
/// User information returned from OAuth provider
/// </summary>
public sealed record OAuthUserInfo
{
    public required string Email { get; init; }
    public required string ProviderId { get; init; }
    public required string FirstName { get; init; }
    public required string LastName { get; init; }

    /// <summary>
    /// Some providers (Apple) may not return email on subsequent logins
    /// </summary>
    public bool HasEmail => !string.IsNullOrWhiteSpace(Email);
}