using System.IdentityModel.Tokens.Jwt;
using System.Text.Json;
using Domain.Constants;
using Domain.Interfaces.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Services;

/// <summary>
/// OAuth service for validating third-party authentication tokens
/// Supports Google and Apple Sign-In for mobile apps
/// </summary>
public class OAuthService : IOAuthService
{
    private readonly IConfiguration _configuration;
    private readonly HttpClient _httpClient;
    private readonly ILogger<OAuthService> _logger;

    public OAuthService(
        IConfiguration configuration,
        HttpClient httpClient,
        ILogger<OAuthService> logger)
    {
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<OAuthUserInfo?> ValidateTokenAsync(
        string provider,
        string token,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(provider))
            return null;

        if (string.IsNullOrWhiteSpace(token))
            return null;

        var normalizedProvider = provider.ToLowerInvariant();

        if (!AuthConstants.OAuthProviders.IsSupported(normalizedProvider))
        {
            _logger.LogWarning("Unsupported OAuth provider: {Provider}", provider);
            return null;
        }

        return normalizedProvider switch
        {
            AuthConstants.OAuthProviders.Google => await ValidateGoogleTokenAsync(token, cancellationToken),
            AuthConstants.OAuthProviders.Apple => await ValidateAppleTokenAsync(token, cancellationToken),
            _ => null
        };
    }

    private async Task<OAuthUserInfo?> ValidateGoogleTokenAsync(
        string idToken,
        CancellationToken cancellationToken)
    {
        try
        {
            // Google ID Token validation endpoint
            var response = await _httpClient.GetAsync(
                $"https://oauth2.googleapis.com/tokeninfo?id_token={idToken}",
                cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("Google token validation failed with status: {StatusCode}", response.StatusCode);
                return null;
            }

            var content = await response.Content.ReadAsStringAsync(cancellationToken);
            using var tokenInfo = JsonDocument.Parse(content);
            var root = tokenInfo.RootElement;

            // Verify the token is for this app
            var clientId = _configuration["OAuth:Google:ClientId"];
            if (!string.IsNullOrEmpty(clientId))
            {
                var audience = root.TryGetProperty("aud", out var aud) ? aud.GetString() : null;
                if (audience != clientId)
                {
                    _logger.LogWarning("Google token audience mismatch. Expected: {Expected}, Got: {Got}", clientId, audience);
                    return null;
                }
            }

            var email = root.TryGetProperty("email", out var e) ? e.GetString() : null;
            var providerId = root.TryGetProperty("sub", out var s) ? s.GetString() : null;
            var givenName = root.TryGetProperty("given_name", out var gn) ? gn.GetString() ?? "" : "";
            var familyName = root.TryGetProperty("family_name", out var fn) ? fn.GetString() ?? "" : "";

            // Email verified check
            var emailVerified = root.TryGetProperty("email_verified", out var ev) && ev.GetString() == "true";
            if (!emailVerified)
            {
                _logger.LogWarning("Google account email not verified");
                return null;
            }

            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(providerId))
            {
                _logger.LogWarning("Google token missing required fields (email or sub)");
                return null;
            }

            return new OAuthUserInfo
            {
                Email = email,
                ProviderId = providerId,
                FirstName = givenName,
                LastName = familyName
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error validating Google token");
            return null;
        }
    }

    private async Task<OAuthUserInfo?> ValidateAppleTokenAsync(
        string identityToken,
        CancellationToken cancellationToken)
    {
        try
        {
            // Apple uses JWT tokens that need to be validated
            // For mobile apps, the identity token is a JWT signed by Apple
            var handler = new JwtSecurityTokenHandler();

            if (!handler.CanReadToken(identityToken))
            {
                _logger.LogWarning("Invalid Apple identity token format");
                return null;
            }

            var jwt = handler.ReadJwtToken(identityToken);

            // Verify issuer
            if (jwt.Issuer != "https://appleid.apple.com")
            {
                _logger.LogWarning("Invalid Apple token issuer: {Issuer}", jwt.Issuer);
                return null;
            }

            // Verify audience (your app's bundle ID)
            var clientId = _configuration["OAuth:Apple:ClientId"];
            if (!string.IsNullOrEmpty(clientId) && !jwt.Audiences.Contains(clientId))
            {
                _logger.LogWarning("Apple token audience mismatch");
                return null;
            }

            // Check expiration
            if (jwt.ValidTo < DateTime.UtcNow)
            {
                _logger.LogWarning("Apple token has expired");
                return null;
            }

            // Extract claims
            var providerId = jwt.Subject;
            var email = jwt.Claims.FirstOrDefault(c => c.Type == "email")?.Value;

            // Apple may not always return email (only on first login or if user shared it)
            // For subsequent logins, we rely on the providerId (sub claim)

            if (string.IsNullOrEmpty(providerId))
            {
                _logger.LogWarning("Apple token missing subject claim");
                return null;
            }

            // Note: Apple doesn't provide first/last name in the JWT
            // These come separately in the authorization response on first sign-in
            // The mobile app should send these separately if available

            return new OAuthUserInfo
            {
                Email = email ?? "",
                ProviderId = providerId,
                FirstName = "",
                LastName = ""
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error validating Apple token");
            return null;
        }
    }
}