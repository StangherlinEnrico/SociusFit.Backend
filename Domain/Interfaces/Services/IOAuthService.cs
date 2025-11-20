namespace Domain.Interfaces.Services;

/// <summary>
/// Service for OAuth authentication
/// </summary>
public interface IOAuthService
{
    Task<(string Email, string ProviderId, string FirstName, string LastName)?> ValidateProviderTokenAsync(
        string provider,
        string token,
        CancellationToken cancellationToken = default);
}