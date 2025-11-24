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
    /// Send password reset email with token
    /// </summary>
    Task SendPasswordResetEmailAsync(
        string email,
        string resetToken,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Send welcome email after successful verification
    /// </summary>
    Task SendWelcomeEmailAsync(
        string email,
        string firstName,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Send password changed confirmation email
    /// </summary>
    Task SendPasswordChangedEmailAsync(
        string email,
        string firstName,
        CancellationToken cancellationToken = default);
}