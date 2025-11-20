namespace Domain.Entities;

/// <summary>
/// Represents a refresh token for user authentication
/// </summary>
public class RefreshToken
{
    public int Id { get; private set; }
    public int UserId { get; private set; }
    public string Token { get; private set; }
    public DateTime ExpiresAt { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? RevokedAt { get; private set; }
    public string? RevokedByIp { get; private set; }
    public string? ReplacedByToken { get; private set; }
    public string CreatedByIp { get; private set; }

    // Navigation property
    public User User { get; private set; } = null!;

    // Private constructor for EF Core
    private RefreshToken() { }

    public RefreshToken(int userId, string token, DateTime expiresAt, string createdByIp)
    {
        if (userId <= 0)
            throw new ArgumentException("User ID must be positive", nameof(userId));

        if (string.IsNullOrWhiteSpace(token))
            throw new ArgumentException("Token cannot be empty", nameof(token));

        if (expiresAt <= DateTime.UtcNow)
            throw new ArgumentException("Expiration must be in the future", nameof(expiresAt));

        if (string.IsNullOrWhiteSpace(createdByIp))
            throw new ArgumentException("IP address cannot be empty", nameof(createdByIp));

        UserId = userId;
        Token = token;
        ExpiresAt = expiresAt;
        CreatedByIp = createdByIp;
        CreatedAt = DateTime.UtcNow;
    }

    public bool IsActive => RevokedAt == null && !IsExpired;

    public bool IsExpired => DateTime.UtcNow >= ExpiresAt;

    public void Revoke(string revokedByIp, string? replacedByToken = null)
    {
        if (string.IsNullOrWhiteSpace(revokedByIp))
            throw new ArgumentException("IP address cannot be empty", nameof(revokedByIp));

        RevokedAt = DateTime.UtcNow;
        RevokedByIp = revokedByIp;
        ReplacedByToken = replacedByToken;
    }
}