namespace Domain.Constants;

/// <summary>
/// Authentication and authorization constants
/// </summary>
public static class AuthConstants
{
    /// <summary>
    /// Email verification token expiration in hours
    /// </summary>
    public const int EmailVerificationTokenExpirationHours = 24;

    /// <summary>
    /// Password reset token expiration in hours
    /// </summary>
    public const int PasswordResetTokenExpirationHours = 1;

    /// <summary>
    /// Supported OAuth providers
    /// </summary>
    public static class OAuthProviders
    {
        public const string Google = "google";
        public const string Apple = "apple";

        public static readonly string[] Supported = [Google, Apple];

        public static bool IsSupported(string provider) =>
            Supported.Contains(provider.ToLowerInvariant());
    }
}