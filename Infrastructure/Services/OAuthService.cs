using Domain.Interfaces.Services;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.Services;

/// <summary>
/// OAuth service for validating third-party authentication tokens
/// NOTE: This is a stub implementation. In production, you would integrate with
/// actual OAuth providers (Google, Facebook, Apple, Microsoft)
/// </summary>
public class OAuthService : IOAuthService
{
    private readonly IConfiguration _configuration;
    private readonly HttpClient _httpClient;

    public OAuthService(IConfiguration configuration, HttpClient httpClient)
    {
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
    }

    public async Task<(string Email, string ProviderId, string FirstName, string LastName)?> ValidateProviderTokenAsync(
        string provider,
        string token,
        CancellationToken cancellationToken = default)
    {
        return provider.ToLowerInvariant() switch
        {
            "google" => await ValidateGoogleTokenAsync(token, cancellationToken),
            "facebook" => await ValidateFacebookTokenAsync(token, cancellationToken),
            "apple" => await ValidateAppleTokenAsync(token, cancellationToken),
            "microsoft" => await ValidateMicrosoftTokenAsync(token, cancellationToken),
            _ => throw new NotSupportedException($"OAuth provider '{provider}' is not supported")
        };
    }

    private async Task<(string Email, string ProviderId, string FirstName, string LastName)?> ValidateGoogleTokenAsync(
        string token,
        CancellationToken cancellationToken)
    {
        try
        {
            // Google Token Validation Endpoint
            var response = await _httpClient.GetAsync(
                $"https://oauth2.googleapis.com/tokeninfo?id_token={token}",
                cancellationToken);

            if (!response.IsSuccessStatusCode)
                return null;

            var content = await response.Content.ReadAsStringAsync(cancellationToken);
            var tokenInfo = System.Text.Json.JsonDocument.Parse(content);
            var root = tokenInfo.RootElement;

            // Verify the token is for this app
            var clientId = _configuration["OAuth:Google:ClientId"];
            if (root.GetProperty("aud").GetString() != clientId)
                return null;

            var email = root.GetProperty("email").GetString();
            var providerId = root.GetProperty("sub").GetString();
            var givenName = root.TryGetProperty("given_name", out var gn) ? gn.GetString() : "";
            var familyName = root.TryGetProperty("family_name", out var fn) ? fn.GetString() : "";

            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(providerId))
                return null;

            return (email, providerId, givenName ?? "", familyName ?? "");
        }
        catch
        {
            return null;
        }
    }

    private async Task<(string Email, string ProviderId, string FirstName, string LastName)?> ValidateFacebookTokenAsync(
        string token,
        CancellationToken cancellationToken)
    {
        try
        {
            // Facebook Graph API
            var response = await _httpClient.GetAsync(
                $"https://graph.facebook.com/me?fields=id,email,first_name,last_name&access_token={token}",
                cancellationToken);

            if (!response.IsSuccessStatusCode)
                return null;

            var content = await response.Content.ReadAsStringAsync(cancellationToken);
            var userInfo = System.Text.Json.JsonDocument.Parse(content);
            var root = userInfo.RootElement;

            var email = root.TryGetProperty("email", out var e) ? e.GetString() : null;
            var providerId = root.GetProperty("id").GetString();
            var firstName = root.TryGetProperty("first_name", out var fn) ? fn.GetString() : "";
            var lastName = root.TryGetProperty("last_name", out var ln) ? ln.GetString() : "";

            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(providerId))
                return null;

            return (email, providerId, firstName ?? "", lastName ?? "");
        }
        catch
        {
            return null;
        }
    }

    private Task<(string Email, string ProviderId, string FirstName, string LastName)?> ValidateAppleTokenAsync(
        string token,
        CancellationToken cancellationToken)
    {
        // Apple Sign In requires JWT validation with Apple's public keys
        // This is more complex and requires additional libraries
        // Placeholder implementation - integrate Apple Sign In SDK in production
        throw new NotImplementedException("Apple Sign In validation not implemented. Use Apple Sign In SDK.");
    }

    private async Task<(string Email, string ProviderId, string FirstName, string LastName)?> ValidateMicrosoftTokenAsync(
        string token,
        CancellationToken cancellationToken)
    {
        try
        {
            // Microsoft Graph API
            var request = new HttpRequestMessage(HttpMethod.Get, "https://graph.microsoft.com/v1.0/me");
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var response = await _httpClient.SendAsync(request, cancellationToken);

            if (!response.IsSuccessStatusCode)
                return null;

            var content = await response.Content.ReadAsStringAsync(cancellationToken);
            var userInfo = System.Text.Json.JsonDocument.Parse(content);
            var root = userInfo.RootElement;

            var email = root.TryGetProperty("mail", out var m) ? m.GetString() :
                        root.TryGetProperty("userPrincipalName", out var upn) ? upn.GetString() : null;
            var providerId = root.GetProperty("id").GetString();
            var givenName = root.TryGetProperty("givenName", out var gn) ? gn.GetString() : "";
            var surname = root.TryGetProperty("surname", out var sn) ? sn.GetString() : "";

            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(providerId))
                return null;

            return (email, providerId, givenName ?? "", surname ?? "");
        }
        catch
        {
            return null;
        }
    }
}