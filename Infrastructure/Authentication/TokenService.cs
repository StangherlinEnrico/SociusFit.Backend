using Domain.Entities;
using Domain.Repositories;
using Microsoft.Extensions.Logging;
using System.IdentityModel.Tokens.Jwt;

namespace Infrastructure.Authentication;

public interface ITokenService
{
    /// <summary>
    /// Extract token ID (JTI claim) from JWT token
    /// </summary>
    string? GetTokenId(string token);

    /// <summary>
    /// Extract user ID from JWT token
    /// </summary>
    Guid? GetUserId(string token);

    /// <summary>
    /// Get token expiration date
    /// </summary>
    DateTime? GetTokenExpiration(string token);

    /// <summary>
    /// Revoke a token (add to blacklist)
    /// </summary>
    Task RevokeTokenAsync(string token, string? reason = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Check if a token is revoked
    /// </summary>
    Task<bool> IsTokenRevokedAsync(string token, CancellationToken cancellationToken = default);

    /// <summary>
    /// Clean up expired tokens from blacklist (should be run periodically)
    /// </summary>
    Task CleanupExpiredTokensAsync(CancellationToken cancellationToken = default);
}

public class TokenService : ITokenService
{
    private readonly IRevokedTokenRepository _revokedTokenRepository;
    private readonly ILogger<TokenService> _logger;

    public TokenService(
        IRevokedTokenRepository revokedTokenRepository,
        ILogger<TokenService> logger)
    {
        _revokedTokenRepository = revokedTokenRepository;
        _logger = logger;
    }

    public string? GetTokenId(string token)
    {
        try
        {
            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(token);
            return jwtToken.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Jti)?.Value;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to extract token ID from JWT");
            return null;
        }
    }

    public Guid? GetUserId(string token)
    {
        try
        {
            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(token);
            var userIdClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Sub)?.Value;

            if (Guid.TryParse(userIdClaim, out var userId))
            {
                return userId;
            }

            return null;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to extract user ID from JWT");
            return null;
        }
    }

    public DateTime? GetTokenExpiration(string token)
    {
        try
        {
            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(token);
            return jwtToken.ValidTo;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to extract expiration from JWT");
            return null;
        }
    }

    public async Task RevokeTokenAsync(string token, string? reason = null, CancellationToken cancellationToken = default)
    {
        var tokenId = GetTokenId(token);
        var userId = GetUserId(token);
        var expiration = GetTokenExpiration(token);

        if (tokenId == null || userId == null || expiration == null)
        {
            _logger.LogWarning("Cannot revoke token: missing token ID, user ID, or expiration");
            return;
        }

        // Check if already revoked
        if (await _revokedTokenRepository.IsTokenRevokedAsync(tokenId, cancellationToken))
        {
            _logger.LogInformation("Token {TokenId} is already revoked", tokenId);
            return;
        }

        var revokedToken = new RevokedToken(
            tokenId,
            userId.Value,
            expiration.Value,
            reason
        );

        await _revokedTokenRepository.AddAsync(revokedToken, cancellationToken);

        _logger.LogInformation(
            "Token {TokenId} revoked for user {UserId}. Reason: {Reason}",
            tokenId,
            userId,
            reason ?? "User logout"
        );
    }

    public async Task<bool> IsTokenRevokedAsync(string token, CancellationToken cancellationToken = default)
    {
        var tokenId = GetTokenId(token);

        if (tokenId == null)
        {
            return false; // If we can't extract token ID, let JWT validation handle it
        }

        return await _revokedTokenRepository.IsTokenRevokedAsync(tokenId, cancellationToken);
    }

    public async Task CleanupExpiredTokensAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Starting cleanup of expired revoked tokens");
        await _revokedTokenRepository.RemoveExpiredTokensAsync(cancellationToken);
        _logger.LogInformation("Cleanup of expired revoked tokens completed");
    }
}