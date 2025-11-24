using Domain.Constants;
using Domain.Interfaces.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text.Json;

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
            var handler = new JwtSecurityTokenHandler();

            if (!handler.CanReadToken(identityToken))
            {
                _logger.LogWarning("Invalid Apple identity token format");
                return null;
            }

            // Per SVILUPPO: legge solo il token senza validare firma
            // var jwt = handler.ReadJwtToken(identityToken);

            // Per PRODUZIONE: valida firma con chiavi pubbliche Apple
            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                ValidateIssuer = true,
                ValidIssuer = "https://appleid.apple.com",
                ValidateAudience = true,
                ValidAudience = _configuration["OAuth:Apple:ClientId"],
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero,
                // Apple usa chiavi pubbliche RSA recuperate da:
                // https://appleid.apple.com/auth/keys
                // Per semplicità in sviluppo, si può disabilitare la validazione
                // In produzione DEVE essere abilitata
                RequireSignedTokens = true
            };

            ClaimsPrincipal? principal;
            SecurityToken? validatedToken;

            // OPZIONE 1: Sviluppo (salta validazione firma)
            // Decommenta per sviluppo
            /*
            validationParameters.SignatureValidator = (token, parameters) =>
            {
                var jwt = new JwtSecurityToken(token);
                return jwt;
            };
            */

            // OPZIONE 2: Produzione (valida firma con chiavi Apple)
            // Richiede implementazione di fetch delle chiavi pubbliche Apple
            // Vedi: https://developer.apple.com/documentation/sign_in_with_apple/sign_in_with_apple_rest_api/verifying_a_user

            try
            {
                principal = handler.ValidateToken(identityToken, validationParameters, out validatedToken);
            }
            catch (SecurityTokenException ex)
            {
                _logger.LogWarning(ex, "Apple token validation failed");
                return null;
            }

            var jwt = validatedToken as JwtSecurityToken;
            if (jwt == null)
            {
                _logger.LogWarning("Invalid Apple JWT token");
                return null;
            }

            var providerId = jwt.Subject;
            var email = jwt.Claims.FirstOrDefault(c => c.Type == "email")?.Value;

            if (string.IsNullOrEmpty(providerId))
            {
                _logger.LogWarning("Apple token missing subject claim");
                return null;
            }

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