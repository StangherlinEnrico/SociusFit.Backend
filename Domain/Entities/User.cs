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

    // Email verification tokens
    public string? EmailVerificationToken { get; private set; }
    public DateTime? EmailVerificationTokenExpiresAt { get; private set; }

    // Password reset tokens
    public string? PasswordResetToken { get; private set; }
    public DateTime? PasswordResetTokenExpiresAt { get; private set; }

    // Tracking
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }
    public DateTime? DeletedAt { get; private set; }
    public DateTime? LastLoginAt { get; private set; }

    // Navigation properties
    private readonly List<AuditLog> _auditLogs = [];
    public IReadOnlyCollection<AuditLog> AuditLogs => _auditLogs.AsReadOnly();

    // Private constructor for EF Core
    private User()
    {
        FirstName = string.Empty;
        LastName = string.Empty;
        Email = string.Empty;
    }

    public User(string firstName, string lastName, string email)
    {
        ValidateFirstName(firstName);
        ValidateLastName(lastName);
        ValidateEmail(email);

        FirstName = firstName.Trim();
        LastName = lastName.Trim();
        Email = email.ToLowerInvariant().Trim();
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Creates a user from OAuth provider (auto-verified)
    /// </summary>
    public static User CreateFromOAuth(
        string firstName,
        string lastName,
        string email,
        string provider,
        string providerId)
    {
        var user = new User(firstName, lastName, email);
        user.SetOAuthProvider(provider, providerId);
        user.VerifyEmail();
        return user;
    }

    public void UpdateProfile(string firstName, string lastName)
    {
        ValidateFirstName(firstName);
        ValidateLastName(lastName);

        FirstName = firstName.Trim();
        LastName = lastName.Trim();
        UpdatedAt = DateTime.UtcNow;
    }

    public void SetPassword(string passwordHash)
    {
        if (string.IsNullOrWhiteSpace(passwordHash))
            throw new ArgumentException("Password hash cannot be empty", nameof(passwordHash));

        PasswordHash = passwordHash;
        ClearPasswordResetToken();
        UpdatedAt = DateTime.UtcNow;
    }

    public void SetOAuthProvider(string provider, string providerId)
    {
        if (string.IsNullOrWhiteSpace(provider))
            throw new ArgumentException("Provider cannot be empty", nameof(provider));

        if (string.IsNullOrWhiteSpace(providerId))
            throw new ArgumentException("Provider ID cannot be empty", nameof(providerId));

        Provider = provider.ToLowerInvariant();
        ProviderId = providerId;
        UpdatedAt = DateTime.UtcNow;
    }

    public void VerifyEmail()
    {
        EmailVerifiedAt = DateTime.UtcNow;
        ClearEmailVerificationToken();
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

    public void SetPasswordResetToken(string token, DateTime expiresAt)
    {
        if (string.IsNullOrWhiteSpace(token))
            throw new ArgumentException("Token cannot be empty", nameof(token));

        if (expiresAt <= DateTime.UtcNow)
            throw new ArgumentException("Expiration date must be in the future", nameof(expiresAt));

        PasswordResetToken = token;
        PasswordResetTokenExpiresAt = expiresAt;
        UpdatedAt = DateTime.UtcNow;
    }

    public void RecordLogin()
    {
        LastLoginAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public bool IsEmailVerificationTokenValid(string token)
    {
        if (string.IsNullOrWhiteSpace(token) || string.IsNullOrWhiteSpace(EmailVerificationToken))
            return false;

        if (EmailVerificationTokenExpiresAt == null || EmailVerificationTokenExpiresAt <= DateTime.UtcNow)
            return false;

        return string.Equals(EmailVerificationToken, token, StringComparison.Ordinal);
    }

    public bool IsPasswordResetTokenValid(string token)
    {
        if (string.IsNullOrWhiteSpace(token) || string.IsNullOrWhiteSpace(PasswordResetToken))
            return false;

        if (PasswordResetTokenExpiresAt == null || PasswordResetTokenExpiresAt <= DateTime.UtcNow)
            return false;

        return string.Equals(PasswordResetToken, token, StringComparison.Ordinal);
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

    public bool HasPassword() => !string.IsNullOrWhiteSpace(PasswordHash);

    public bool CanLoginWithPassword() => HasPassword() && !HasOAuthProvider();

    private void ClearEmailVerificationToken()
    {
        EmailVerificationToken = null;
        EmailVerificationTokenExpiresAt = null;
    }

    private void ClearPasswordResetToken()
    {
        PasswordResetToken = null;
        PasswordResetTokenExpiresAt = null;
    }

    private static void ValidateFirstName(string firstName)
    {
        if (string.IsNullOrWhiteSpace(firstName))
            throw new ArgumentException("First name cannot be empty", nameof(firstName));
    }

    private static void ValidateLastName(string lastName)
    {
        if (string.IsNullOrWhiteSpace(lastName))
            throw new ArgumentException("Last name cannot be empty", nameof(lastName));
    }

    private static void ValidateEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("Email cannot be empty", nameof(email));
    }
}