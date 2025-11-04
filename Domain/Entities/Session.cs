namespace Domain.Entities;

/// <summary>
/// Represents an authentication session
/// </summary>
public class Session
{
    public int Id { get; private set; }
    public int UserId { get; private set; }
    public string Token { get; private set; }
    public DateTime ExpiresAt { get; private set; }
    public DateTime CreatedAt { get; private set; }

    // Navigation property
    public User User { get; private set; } = null!;

    // Private constructor for EF Core
    private Session() { }

    public Session(int userId, string token, DateTime expiresAt)
    {
        if (userId <= 0)
            throw new ArgumentException("User ID must be positive", nameof(userId));

        if (string.IsNullOrWhiteSpace(token))
            throw new ArgumentException("Token cannot be empty", nameof(token));

        if (expiresAt <= DateTime.UtcNow)
            throw new ArgumentException("Expiration date must be in the future", nameof(expiresAt));

        UserId = userId;
        Token = token;
        ExpiresAt = expiresAt;
        CreatedAt = DateTime.UtcNow;
    }

    public bool IsExpired() => DateTime.UtcNow > ExpiresAt;

    public bool IsValid() => !IsExpired();
}