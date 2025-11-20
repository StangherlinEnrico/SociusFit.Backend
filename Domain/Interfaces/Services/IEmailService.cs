namespace Domain.Interfaces.Services;

/// <summary>
/// Service for sending emails
/// </summary>
public interface IEmailService
{
    /// <summary>
    /// Send email verification email with token
    /// </summary>
    Task SendVerificationEmailAsync(
        string email,
        string firstName,
        string verificationToken,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Send password reset email
    /// </summary>
    Task SendPasswordResetEmailAsync(
        string email,
        string resetLink,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Send welcome email after successful verification
    /// </summary>
    Task SendWelcomeEmailAsync(
        string email,
        string firstName,
        CancellationToken cancellationToken = default);
}