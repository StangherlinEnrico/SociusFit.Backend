namespace Domain.Entities;

/// <summary>
/// Represents user consent for data processing (GDPR compliance)
/// </summary>
public class UserConsent
{
    public int Id { get; private set; }
    public int UserId { get; private set; }
    public string ConsentType { get; private set; }
    public bool IsGranted { get; private set; }
    public string? IpAddress { get; private set; }
    public DateTime? GrantedAt { get; private set; }
    public DateTime? RevokedAt { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    // Navigation property
    public User User { get; private set; } = null!;

    // Private constructor for EF Core
    private UserConsent() { }

    public UserConsent(int userId, string consentType, string? ipAddress = null)
    {
        if (userId <= 0)
            throw new ArgumentException("User ID must be positive", nameof(userId));

        if (string.IsNullOrWhiteSpace(consentType))
            throw new ArgumentException("Consent type cannot be empty", nameof(consentType));

        UserId = userId;
        ConsentType = consentType;
        IsGranted = false;
        IpAddress = ipAddress;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Grant(string? ipAddress = null)
    {
        IsGranted = true;
        GrantedAt = DateTime.UtcNow;
        RevokedAt = null;
        IpAddress = ipAddress ?? IpAddress;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Revoke(string? ipAddress = null)
    {
        IsGranted = false;
        RevokedAt = DateTime.UtcNow;
        IpAddress = ipAddress ?? IpAddress;
        UpdatedAt = DateTime.UtcNow;
    }

    public bool IsActive() => IsGranted && !RevokedAt.HasValue;
}