using System.Security.Claims;

namespace Domain.Interfaces.Services;

/// <summary>
/// Service for JWT token operations
/// </summary>
public interface IJwtService
{
    /// <summary>
    /// Generate access token (short-lived JWT)
    /// </summary>
    string GenerateAccessToken(int userId, string email, string firstName, string lastName);

    /// <summary>
    /// Generate refresh token (long-lived random token)
    /// </summary>
    string GenerateRefreshToken();

    /// <summary>
    /// Validate JWT token and return claims principal
    /// </summary>
    ClaimsPrincipal? ValidateToken(string token);

    /// <summary>
    /// Extract user ID from JWT token
    /// </summary>
    int? GetUserIdFromToken(string token);

    /// <summary>
    /// Get access token expiration time
    /// </summary>
    DateTime GetAccessTokenExpiration();

    /// <summary>
    /// Get refresh token expiration time
    /// </summary>
    DateTime GetRefreshTokenExpiration();
}