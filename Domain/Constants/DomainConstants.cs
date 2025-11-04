namespace Domain.Constants;

/// <summary>
/// Constants for consent types
/// </summary>
public static class ConsentTypes
{
    public const string TermsOfService = "terms_of_service";
    public const string PrivacyPolicy = "privacy_policy";
    public const string MarketingEmails = "marketing_emails";
    public const string DataProcessing = "data_processing";
    public const string LocationSharing = "location_sharing";
}

/// <summary>
/// Constants for audit log actions
/// </summary>
public static class AuditActions
{
    public const string Create = "CREATE";
    public const string Update = "UPDATE";
    public const string Delete = "DELETE";
    public const string Login = "LOGIN";
    public const string Logout = "LOGOUT";
    public const string PasswordChange = "PASSWORD_CHANGE";
    public const string EmailVerification = "EMAIL_VERIFICATION";
    public const string ConsentGranted = "CONSENT_GRANTED";
    public const string ConsentRevoked = "CONSENT_REVOKED";
}

/// <summary>
/// Constants for OAuth providers
/// </summary>
public static class OAuthProviders
{
    public const string Google = "google";
    public const string Facebook = "facebook";
    public const string Apple = "apple";
    public const string Microsoft = "microsoft";
}

/// <summary>
/// Constants for table names
/// </summary>
public static class TableNames
{
    public const string Users = "users";
    public const string Sessions = "sessions";
    public const string UserConsents = "user_consents";
    public const string AuditLogs = "audit_logs";
    public const string Sports = "sports";
    public const string Levels = "levels";
    public const string UserSports = "user_sports";
}

/// <summary>
/// Constants for validation
/// </summary>
public static class ValidationConstants
{
    public const int MinPasswordLength = 8;
    public const int MaxPasswordLength = 128;
    public const int MaxNameLength = 100;
    public const int MaxEmailLength = 255;
    public const int MaxLocationLength = 255;
    public const int TokenLength = 64;
    public const int DefaultMaxDistanceKm = 50;
    public const int MinMaxDistanceKm = 1;
    public const int MaxMaxDistanceKm = 500;
}

/// <summary>
/// Constants for session configuration
/// </summary>
public static class SessionConstants
{
    public static readonly TimeSpan DefaultSessionDuration = TimeSpan.FromDays(7);
    public static readonly TimeSpan RefreshTokenDuration = TimeSpan.FromDays(30);
}