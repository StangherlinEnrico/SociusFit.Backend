using Domain.Interfaces.Services;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MimeKit;

namespace Infrastructure.Services;

/// <summary>
/// Email service implementation using MailKit
/// </summary>
public class EmailService : IEmailService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<EmailService> _logger;
    private readonly string _baseUrl;
    private readonly string _appBaseUrl;

    public EmailService(IConfiguration configuration, ILogger<EmailService> logger)
    {
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _baseUrl = _configuration["App:BaseUrl"] ?? "http://localhost:5000";
        _appBaseUrl = _configuration["App:MobileDeepLinkUrl"] ?? "sociusfit://";
    }

    public async Task SendVerificationEmailAsync(
        string email,
        string firstName,
        string verificationToken,
        CancellationToken cancellationToken = default)
    {
        // Web link for fallback
        var webVerificationLink = $"{_baseUrl}/api/v1/auth/verify-email?token={verificationToken}";

        // Deep link for mobile app
        var appVerificationLink = $"{_appBaseUrl}verify-email?token={verificationToken}";

        var subject = "Verify Your Email Address - SociusFit";
        var body = GetEmailVerificationTemplate(firstName, webVerificationLink, appVerificationLink);

        await SendEmailAsync(email, subject, body, cancellationToken);

        _logger.LogInformation("Verification email sent to {Email}", email);
    }

    public async Task SendPasswordResetEmailAsync(
        string email,
        string resetToken,
        CancellationToken cancellationToken = default)
    {
        // Deep link for mobile app
        var appResetLink = $"{_appBaseUrl}reset-password?token={resetToken}";

        // Web link for fallback (if you have a web app)
        var webResetLink = $"{_baseUrl}/reset-password?token={resetToken}";

        var subject = "Reset Your Password - SociusFit";
        var body = GetPasswordResetTemplate(appResetLink, webResetLink);

        await SendEmailAsync(email, subject, body, cancellationToken);

        _logger.LogInformation("Password reset email sent to {Email}", email);
    }

    public async Task SendWelcomeEmailAsync(
        string email,
        string firstName,
        CancellationToken cancellationToken = default)
    {
        var subject = "Welcome to SociusFit! 🎉";
        var body = GetWelcomeTemplate(firstName);

        await SendEmailAsync(email, subject, body, cancellationToken);

        _logger.LogInformation("Welcome email sent to {Email}", email);
    }

    public async Task SendPasswordChangedEmailAsync(
        string email,
        string firstName,
        CancellationToken cancellationToken = default)
    {
        var subject = "Your Password Has Been Changed - SociusFit";
        var body = GetPasswordChangedTemplate(firstName);

        await SendEmailAsync(email, subject, body, cancellationToken);

        _logger.LogInformation("Password changed email sent to {Email}", email);
    }

    private async Task SendEmailAsync(
        string toEmail,
        string subject,
        string htmlBody,
        CancellationToken cancellationToken = default)
    {
        var smtpHost = _configuration["Email:SmtpHost"] ?? "smtp.gmail.com";
        var smtpPort = int.Parse(_configuration["Email:SmtpPort"] ?? "587");
        var smtpUsername = _configuration["Email:SmtpUsername"] ?? "";
        var smtpPassword = _configuration["Email:SmtpPassword"] ?? "";
        var fromEmail = _configuration["Email:FromEmail"] ?? smtpUsername;
        var fromName = _configuration["Email:FromName"] ?? "SociusFit";

        var message = new MimeMessage();
        message.From.Add(new MailboxAddress(fromName, fromEmail));
        message.To.Add(new MailboxAddress("", toEmail));
        message.Subject = subject;

        var bodyBuilder = new BodyBuilder
        {
            HtmlBody = htmlBody
        };
        message.Body = bodyBuilder.ToMessageBody();

        using var client = new SmtpClient();
        try
        {
            await client.ConnectAsync(smtpHost, smtpPort, SecureSocketOptions.StartTls, cancellationToken);
            await client.AuthenticateAsync(smtpUsername, smtpPassword, cancellationToken);
            await client.SendAsync(message, cancellationToken);
            await client.DisconnectAsync(true, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send email to {Email}", toEmail);
            throw new InvalidOperationException($"Failed to send email to {toEmail}", ex);
        }
    }

    private static string GetEmailVerificationTemplate(string firstName, string webLink, string appLink)
    {
        return $@"
<!DOCTYPE html>
<html>
<head>
    <meta charset='utf-8'>
    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
    <title>Verify Your Email</title>
</head>
<body style='margin: 0; padding: 0; font-family: -apple-system, BlinkMacSystemFont, ""Segoe UI"", Roboto, sans-serif; background-color: #f4f4f4;'>
    <table width='100%' cellpadding='0' cellspacing='0' border='0' style='background-color: #f4f4f4; padding: 20px 0;'>
        <tr>
            <td align='center'>
                <table width='600' cellpadding='0' cellspacing='0' border='0' style='background-color: #ffffff; border-radius: 8px; box-shadow: 0 2px 4px rgba(0,0,0,0.1);'>
                    <tr>
                        <td style='background: linear-gradient(135deg, #667eea 0%, #764ba2 100%); padding: 40px 30px; text-align: center; border-radius: 8px 8px 0 0;'>
                            <h1 style='color: #ffffff; margin: 0; font-size: 28px; font-weight: bold;'>SociusFit</h1>
                        </td>
                    </tr>
                    <tr>
                        <td style='padding: 40px 30px;'>
                            <h2 style='color: #333333; margin: 0 0 20px 0; font-size: 24px;'>Welcome, {firstName}! 👋</h2>
                            <p style='color: #555555; line-height: 1.6; margin: 0 0 20px 0; font-size: 16px;'>
                                Thank you for joining <strong>SociusFit</strong>! We're excited to help you connect with others who share your passion for sports.
                            </p>
                            <p style='color: #555555; line-height: 1.6; margin: 0 0 30px 0; font-size: 16px;'>
                                To get started, please verify your email address by tapping the button below:
                            </p>
                            <table width='100%' cellpadding='0' cellspacing='0' border='0'>
                                <tr>
                                    <td align='center' style='padding: 10px 0 30px 0;'>
                                        <a href='{appLink}' style='display: inline-block; background: linear-gradient(135deg, #667eea 0%, #764ba2 100%); color: #ffffff; text-decoration: none; padding: 15px 40px; border-radius: 5px; font-size: 16px; font-weight: bold;'>
                                            Verify Email Address
                                        </a>
                                    </td>
                                </tr>
                            </table>
                            <p style='color: #888888; line-height: 1.6; margin: 0; font-size: 14px;'>
                                If the button doesn't work, <a href='{webLink}' style='color: #667eea;'>click here</a> to verify in your browser.
                            </p>
                            <div style='border-top: 1px solid #eeeeee; margin: 30px 0; padding-top: 20px;'>
                                <p style='color: #888888; line-height: 1.6; margin: 0; font-size: 14px;'>
                                    <strong>⏰ This link expires in 24 hours.</strong>
                                </p>
                                <p style='color: #888888; line-height: 1.6; margin: 10px 0 0 0; font-size: 14px;'>
                                    If you didn't create an account, you can safely ignore this email.
                                </p>
                            </div>
                        </td>
                    </tr>
                    <tr>
                        <td style='background-color: #f8f8f8; padding: 20px 30px; text-align: center; border-radius: 0 0 8px 8px;'>
                            <p style='color: #888888; margin: 0; font-size: 14px;'>© 2025 SociusFit. All rights reserved.</p>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>
</body>
</html>";
    }

    private static string GetPasswordResetTemplate(string appLink, string webLink)
    {
        return $@"
<!DOCTYPE html>
<html>
<head>
    <meta charset='utf-8'>
    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
    <title>Reset Your Password</title>
</head>
<body style='margin: 0; padding: 0; font-family: -apple-system, BlinkMacSystemFont, ""Segoe UI"", Roboto, sans-serif; background-color: #f4f4f4;'>
    <table width='100%' cellpadding='0' cellspacing='0' border='0' style='background-color: #f4f4f4; padding: 20px 0;'>
        <tr>
            <td align='center'>
                <table width='600' cellpadding='0' cellspacing='0' border='0' style='background-color: #ffffff; border-radius: 8px; box-shadow: 0 2px 4px rgba(0,0,0,0.1);'>
                    <tr>
                        <td style='background: linear-gradient(135deg, #667eea 0%, #764ba2 100%); padding: 40px 30px; text-align: center; border-radius: 8px 8px 0 0;'>
                            <h1 style='color: #ffffff; margin: 0; font-size: 28px; font-weight: bold;'>SociusFit</h1>
                        </td>
                    </tr>
                    <tr>
                        <td style='padding: 40px 30px;'>
                            <h2 style='color: #333333; margin: 0 0 20px 0; font-size: 24px;'>Password Reset Request 🔐</h2>
                            <p style='color: #555555; line-height: 1.6; margin: 0 0 20px 0; font-size: 16px;'>
                                We received a request to reset your password. Tap the button below to create a new password:
                            </p>
                            <table width='100%' cellpadding='0' cellspacing='0' border='0'>
                                <tr>
                                    <td align='center' style='padding: 10px 0 30px 0;'>
                                        <a href='{appLink}' style='display: inline-block; background: linear-gradient(135deg, #667eea 0%, #764ba2 100%); color: #ffffff; text-decoration: none; padding: 15px 40px; border-radius: 5px; font-size: 16px; font-weight: bold;'>
                                            Reset Password
                                        </a>
                                    </td>
                                </tr>
                            </table>
                            <p style='color: #888888; line-height: 1.6; margin: 0; font-size: 14px;'>
                                If the button doesn't work, <a href='{webLink}' style='color: #667eea;'>click here</a> to reset in your browser.
                            </p>
                            <div style='border-top: 1px solid #eeeeee; margin: 30px 0; padding-top: 20px;'>
                                <p style='color: #888888; line-height: 1.6; margin: 0; font-size: 14px;'>
                                    <strong>⏰ This link expires in 1 hour.</strong>
                                </p>
                                <p style='color: #888888; line-height: 1.6; margin: 10px 0 0 0; font-size: 14px;'>
                                    If you didn't request a password reset, please ignore this email or contact support if you have concerns.
                                </p>
                            </div>
                        </td>
                    </tr>
                    <tr>
                        <td style='background-color: #f8f8f8; padding: 20px 30px; text-align: center; border-radius: 0 0 8px 8px;'>
                            <p style='color: #888888; margin: 0; font-size: 14px;'>© 2025 SociusFit. All rights reserved.</p>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>
</body>
</html>";
    }

    private static string GetWelcomeTemplate(string firstName)
    {
        return $@"
<!DOCTYPE html>
<html>
<head>
    <meta charset='utf-8'>
    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
    <title>Welcome to SociusFit</title>
</head>
<body style='margin: 0; padding: 0; font-family: -apple-system, BlinkMacSystemFont, ""Segoe UI"", Roboto, sans-serif; background-color: #f4f4f4;'>
    <table width='100%' cellpadding='0' cellspacing='0' border='0' style='background-color: #f4f4f4; padding: 20px 0;'>
        <tr>
            <td align='center'>
                <table width='600' cellpadding='0' cellspacing='0' border='0' style='background-color: #ffffff; border-radius: 8px; box-shadow: 0 2px 4px rgba(0,0,0,0.1);'>
                    <tr>
                        <td style='background: linear-gradient(135deg, #667eea 0%, #764ba2 100%); padding: 40px 30px; text-align: center; border-radius: 8px 8px 0 0;'>
                            <h1 style='color: #ffffff; margin: 0; font-size: 32px; font-weight: bold;'>🎉 Welcome!</h1>
                        </td>
                    </tr>
                    <tr>
                        <td style='padding: 40px 30px;'>
                            <h2 style='color: #333333; margin: 0 0 20px 0; font-size: 24px;'>You're All Set, {firstName}! 🚀</h2>
                            <p style='color: #555555; line-height: 1.6; margin: 0 0 20px 0; font-size: 16px;'>
                                Your email has been successfully verified! Welcome to the <strong>SociusFit</strong> community.
                            </p>
                            <div style='background-color: #f8f9fa; border-left: 4px solid #667eea; padding: 20px; margin: 20px 0;'>
                                <h3 style='color: #333333; margin: 0 0 15px 0; font-size: 18px;'>Get Started:</h3>
                                <p style='color: #555555; line-height: 1.8; margin: 0;'>
                                    🏃 Add your favorite sports and skill levels<br>
                                    📍 Set your location to find nearby partners<br>
                                    🤝 Connect with people who share your passion<br>
                                    📅 Organize your first sports session
                                </p>
                            </div>
                            <p style='color: #555555; line-height: 1.6; margin: 20px 0; font-size: 16px;'>
                                Ready to play? Open the app and start connecting!
                            </p>
                        </td>
                    </tr>
                    <tr>
                        <td style='background-color: #f8f8f8; padding: 20px 30px; text-align: center; border-radius: 0 0 8px 8px;'>
                            <p style='color: #888888; margin: 0; font-size: 14px;'>© 2025 SociusFit. All rights reserved.</p>
                            <p style='color: #888888; margin: 10px 0 0 0; font-size: 12px;'>Find your sport partner, play better together!</p>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>
</body>
</html>";
    }

    private static string GetPasswordChangedTemplate(string firstName)
    {
        return $@"
<!DOCTYPE html>
<html>
<head>
    <meta charset='utf-8'>
    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
    <title>Password Changed</title>
</head>
<body style='margin: 0; padding: 0; font-family: -apple-system, BlinkMacSystemFont, ""Segoe UI"", Roboto, sans-serif; background-color: #f4f4f4;'>
    <table width='100%' cellpadding='0' cellspacing='0' border='0' style='background-color: #f4f4f4; padding: 20px 0;'>
        <tr>
            <td align='center'>
                <table width='600' cellpadding='0' cellspacing='0' border='0' style='background-color: #ffffff; border-radius: 8px; box-shadow: 0 2px 4px rgba(0,0,0,0.1);'>
                    <tr>
                        <td style='background: linear-gradient(135deg, #667eea 0%, #764ba2 100%); padding: 40px 30px; text-align: center; border-radius: 8px 8px 0 0;'>
                            <h1 style='color: #ffffff; margin: 0; font-size: 28px; font-weight: bold;'>SociusFit</h1>
                        </td>
                    </tr>
                    <tr>
                        <td style='padding: 40px 30px;'>
                            <h2 style='color: #333333; margin: 0 0 20px 0; font-size: 24px;'>Password Changed ✅</h2>
                            <p style='color: #555555; line-height: 1.6; margin: 0 0 20px 0; font-size: 16px;'>
                                Hi {firstName},
                            </p>
                            <p style='color: #555555; line-height: 1.6; margin: 0 0 20px 0; font-size: 16px;'>
                                Your password has been successfully changed. You can now use your new password to log in to your account.
                            </p>
                            <div style='background-color: #fff3cd; border-left: 4px solid #ffc107; padding: 20px; margin: 20px 0;'>
                                <p style='color: #856404; line-height: 1.6; margin: 0; font-size: 14px;'>
                                    <strong>⚠️ Didn't make this change?</strong><br>
                                    If you didn't change your password, please secure your account immediately by resetting your password and contacting our support team.
                                </p>
                            </div>
                        </td>
                    </tr>
                    <tr>
                        <td style='background-color: #f8f8f8; padding: 20px 30px; text-align: center; border-radius: 0 0 8px 8px;'>
                            <p style='color: #888888; margin: 0; font-size: 14px;'>© 2025 SociusFit. All rights reserved.</p>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>
</body>
</html>";
    }
}