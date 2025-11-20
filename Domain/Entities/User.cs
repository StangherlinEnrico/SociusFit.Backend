namespace Domain.Entities;

/// <summary>
/// Represents a user in the system
/// </summary>
public class User
{
    public int Id { get; private set; }
    public string FirstName { get; private set; }
    public string LastName { get; private set; }
    public string Email { get; private set; }
    public DateTime? EmailVerifiedAt { get; private set; }
    public string? PasswordHash { get; private set; }
    public string? Provider { get; private set; }
    public string? ProviderId { get; private set; }
    public string? Location { get; private set; }
    public decimal? Latitude { get; private set; }
    public decimal? Longitude { get; private set; }
    public int? MaxDistanceKm { get; private set; }

    // Email verification tokens
    public string? EmailVerificationToken { get; private set; }
    public DateTime? EmailVerificationTokenExpiresAt { get; private set; }

    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }
    public DateTime? DeletedAt { get; private set; }

    // Navigation properties
    private readonly List<AuditLog> _auditLogs = new();
    public IReadOnlyCollection<AuditLog> AuditLogs => _auditLogs.AsReadOnly();

    // Private constructor for EF Core
    private User() { }

    public User(string firstName, string lastName, string email)
    {
        if (string.IsNullOrWhiteSpace(firstName))
            throw new ArgumentException("First name cannot be empty", nameof(firstName));

        if (string.IsNullOrWhiteSpace(lastName))
            throw new ArgumentException("Last name cannot be empty", nameof(lastName));

        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("Email cannot be empty", nameof(email));

        FirstName = firstName;
        LastName = lastName;
        Email = email;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateProfile(string firstName, string lastName, string? location = null)
    {
        if (string.IsNullOrWhiteSpace(firstName))
            throw new ArgumentException("First name cannot be empty", nameof(firstName));

        if (string.IsNullOrWhiteSpace(lastName))
            throw new ArgumentException("Last name cannot be empty", nameof(lastName));

        FirstName = firstName;
        LastName = lastName;
        Location = location;
        UpdatedAt = DateTime.UtcNow;
    }

    public void SetPassword(string passwordHash)
    {
        if (string.IsNullOrWhiteSpace(passwordHash))
            throw new ArgumentException("Password hash cannot be empty", nameof(passwordHash));

        PasswordHash = passwordHash;
        UpdatedAt = DateTime.UtcNow;
    }

    public void SetOAuthProvider(string provider, string providerId)
    {
        if (string.IsNullOrWhiteSpace(provider))
            throw new ArgumentException("Provider cannot be empty", nameof(provider));

        if (string.IsNullOrWhiteSpace(providerId))
            throw new ArgumentException("Provider ID cannot be empty", nameof(providerId));

        Provider = provider;
        ProviderId = providerId;
        UpdatedAt = DateTime.UtcNow;
    }

    public void VerifyEmail()
    {
        EmailVerifiedAt = DateTime.UtcNow;
        EmailVerificationToken = null;
        EmailVerificationTokenExpiresAt = null;
        UpdatedAt = DateTime.UtcNow;
    }

    public void SetEmailVerificationToken(string token, DateTime expiresAt)
    {
        if (string.IsNullOrWhiteSpace(token))
            throw new ArgumentException("Token cannot be empty", nameof(token));

        if (expiresAt <= DateTime.UtcNow)
            throw new ArgumentException("Expiration date must be in the future", nameof(expiresAt));

        EmailVerificationToken = token;
        EmailVerificationTokenExpiresAt = expiresAt;
        UpdatedAt = DateTime.UtcNow;
    }

    public bool IsEmailVerificationTokenValid(string token)
    {
        if (string.IsNullOrWhiteSpace(token) || string.IsNullOrWhiteSpace(EmailVerificationToken))
            return false;

        if (EmailVerificationTokenExpiresAt == null || EmailVerificationTokenExpiresAt <= DateTime.UtcNow)
            return false;

        return EmailVerificationToken == token;
    }

    public void SetLocation(decimal latitude, decimal longitude, int maxDistanceKm)
    {
        if (latitude < -90 || latitude > 90)
            throw new ArgumentException("Latitude must be between -90 and 90", nameof(latitude));

        if (longitude < -180 || longitude > 180)
            throw new ArgumentException("Longitude must be between -180 and 180", nameof(longitude));

        if (maxDistanceKm <= 0)
            throw new ArgumentException("Max distance must be positive", nameof(maxDistanceKm));

        Latitude = latitude;
        Longitude = longitude;
        MaxDistanceKm = maxDistanceKm;
        UpdatedAt = DateTime.UtcNow;
    }

    public void SoftDelete()
    {
        DeletedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Restore()
    {
        DeletedAt = null;
        UpdatedAt = DateTime.UtcNow;
    }

    public bool IsDeleted() => DeletedAt.HasValue;

    public bool IsEmailVerified() => EmailVerifiedAt.HasValue;

    public bool HasOAuthProvider() => !string.IsNullOrWhiteSpace(Provider);
}