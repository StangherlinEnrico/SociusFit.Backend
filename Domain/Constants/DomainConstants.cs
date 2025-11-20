namespace Domain.Constants;

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
}

/// <summary>
/// Constants for OAuth providers
/// </summary>
public static class OAuthProviders
{
    public const string Google = "google";
    public const string Apple = "apple";
}

/// <summary>
/// Constants for table names
/// </summary>
public static class TableNames
{
    public const string AuditLogs = "audit_logs";
    public const string Users = "users";
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
