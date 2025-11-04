using Domain.Entities;

namespace Domain.Interfaces.Services;

/// <summary>
/// Service for password hashing and verification
/// </summary>
public interface IPasswordHasher
{
    string HashPassword(string password);
    bool VerifyPassword(string password, string hash);
}

/// <summary>
/// Service for generating and validating authentication tokens
/// </summary>
public interface ITokenGenerator
{
    string GenerateToken();
    string GenerateRefreshToken();
}

/// <summary>
/// Service for sending emails
/// </summary>
public interface IEmailService
{
    Task SendVerificationEmailAsync(string email, string verificationLink, CancellationToken cancellationToken = default);
    Task SendPasswordResetEmailAsync(string email, string resetLink, CancellationToken cancellationToken = default);
    Task SendWelcomeEmailAsync(string email, string firstName, CancellationToken cancellationToken = default);
}

/// <summary>
/// Service for audit logging
/// </summary>
public interface IAuditService
{
    Task LogAsync(
        string action,
        string tableName,
        int? recordId = null,
        int? userId = null,
        object? oldValues = null,
        object? newValues = null,
        string? ipAddress = null,
        CancellationToken cancellationToken = default);
}

/// <summary>
/// Service for location-based operations
/// </summary>
public interface ILocationService
{
    double CalculateDistance(decimal lat1, decimal lon1, decimal lat2, decimal lon2);
    Task<IEnumerable<User>> FindUsersNearbyAsync(
        decimal latitude,
        decimal longitude,
        int maxDistanceKm,
        CancellationToken cancellationToken = default);
}

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