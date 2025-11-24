namespace Domain.Constants;

/// <summary>
/// Validation rules constants
/// </summary>
public static class ValidationConstants
{
    public static class User
    {
        public const int FirstNameMaxLength = 100;
        public const int LastNameMaxLength = 100;
        public const int EmailMaxLength = 255;
        public const int PasswordMinLength = 8;
        public const int PasswordMaxLength = 128;
    }

    public static class Token
    {
        public const int RefreshTokenMaxLength = 500;
        public const int VerificationTokenMaxLength = 255;
    }

    public static class IpAddress
    {
        public const int MaxLength = 45; // IPv6 max length
    }
}