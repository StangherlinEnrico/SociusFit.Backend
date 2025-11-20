using Domain.Interfaces.Services;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Configuration;
using MimeKit;

namespace Infrastructure.Services;

/// <summary>
/// Email service implementation using MailKit
/// </summary>
public class EmailService : IEmailService
{
    private readonly IConfiguration _configuration;
    private readonly string _baseUrl;

    public EmailService(IConfiguration configuration)
    {
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        _baseUrl = _configuration["App:BaseUrl"] ?? "http://localhost:5000";
    }

    public async Task SendVerificationEmailAsync(
        string email,
        string firstName,
        string verificationToken,
        CancellationToken cancellationToken = default)
    {
        var verificationLink = $"{_baseUrl}/api/v1/users/verify-email?token={verificationToken}";

        var subject = "Verify Your Email Address - SociusFit";
        var body = GetEmailVerificationTemplate(firstName, verificationLink);

        await SendEmailAsync(email, subject, body, cancellationToken);
    }

    public async Task SendPasswordResetEmailAsync(
        string email,
        string resetLink,
        CancellationToken cancellationToken = default)
    {
        var subject = "Reset Your Password - SociusFit";
        var body = GetPasswordResetTemplate(resetLink);

        await SendEmailAsync(email, subject, body, cancellationToken);
    }

    public async Task SendWelcomeEmailAsync(
        string email,
        string firstName,
        CancellationToken cancellationToken = default)
    {
        var subject = "Welcome to SociusFit! 🎉";
        var body = GetWelcomeTemplate(firstName);

        await SendEmailAsync(email, subject, body, cancellationToken);
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
            // Log the exception in production
            throw new InvalidOperationException($"Failed to send email to {toEmail}", ex);
        }
    }

    private string GetEmailVerificationTemplate(string firstName, string verificationLink)
    {
        return $@"
<!DOCTYPE html>
<html>
<head>
    <meta charset='utf-8'>
    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
    <title>Verify Your Email</title>
</head>
<body style='margin: 0; padding: 0; font-family: Arial, sans-serif; background-color: #f4f4f4;'>
    <table width='100%' cellpadding='0' cellspacing='0' border='0' style='background-color: #f4f4f4; padding: 20px 0;'>
        <tr>
            <td align='center'>
                <table width='600' cellpadding='0' cellspacing='0' border='0' style='background-color: #ffffff; border-radius: 8px; box-shadow: 0 2px 4px rgba(0,0,0,0.1);'>
                    <!-- Header -->
                    <tr>
                        <td style='background: linear-gradient(135deg, #667eea 0%, #764ba2 100%); padding: 40px 30px; text-align: center; border-radius: 8px 8px 0 0;'>
                            <h1 style='color: #ffffff; margin: 0; font-size: 28px; font-weight: bold;'>SociusFit</h1>
                        </td>
                    </tr>
                    
                    <!-- Body -->
                    <tr>
                        <td style='padding: 40px 30px;'>
                            <h2 style='color: #333333; margin: 0 0 20px 0; font-size: 24px;'>Welcome, {firstName}! 👋</h2>
                            
                            <p style='color: #555555; line-height: 1.6; margin: 0 0 20px 0; font-size: 16px;'>
                                Thank you for joining <strong>SociusFit</strong>! We're excited to help you connect with others who share your passion for sports.
                            </p>
                            
                            <p style='color: #555555; line-height: 1.6; margin: 0 0 30px 0; font-size: 16px;'>
                                To get started, please verify your email address by clicking the button below:
                            </p>
                            
                            <table width='100%' cellpadding='0' cellspacing='0' border='0'>
                                <tr>
                                    <td align='center' style='padding: 10px 0 30px 0;'>
                                        <a href='{verificationLink}' style='display: inline-block; background: linear-gradient(135deg, #667eea 0%, #764ba2 100%); color: #ffffff; text-decoration: none; padding: 15px 40px; border-radius: 5px; font-size: 16px; font-weight: bold;'>
                                            Verify Email Address
                                        </a>
                                    </td>
                                </tr>
                            </table>
                            
                            <p style='color: #888888; line-height: 1.6; margin: 0; font-size: 14px;'>
                                Or copy and paste this link into your browser:<br>
                                <a href='{verificationLink}' style='color: #667eea; word-break: break-all;'>{verificationLink}</a>
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
                    
                    <!-- Footer -->
                    <tr>
                        <td style='background-color: #f8f8f8; padding: 20px 30px; text-align: center; border-radius: 0 0 8px 8px;'>
                            <p style='color: #888888; margin: 0; font-size: 14px;'>
                                © 2025 SociusFit. All rights reserved.
                            </p>
                            <p style='color: #888888; margin: 10px 0 0 0; font-size: 12px;'>
                                Find your sport partner, play better together!
                            </p>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>
</body>
</html>";
    }

    private string GetPasswordResetTemplate(string resetLink)
    {
        return $@"
<!DOCTYPE html>
<html>
<head>
    <meta charset='utf-8'>
    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
    <title>Reset Your Password</title>
</head>
<body style='margin: 0; padding: 0; font-family: Arial, sans-serif; background-color: #f4f4f4;'>
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
                                We received a request to reset your password. Click the button below to create a new password:
                            </p>
                            
                            <table width='100%' cellpadding='0' cellspacing='0' border='0'>
                                <tr>
                                    <td align='center' style='padding: 10px 0 30px 0;'>
                                        <a href='{resetLink}' style='display: inline-block; background: linear-gradient(135deg, #667eea 0%, #764ba2 100%); color: #ffffff; text-decoration: none; padding: 15px 40px; border-radius: 5px; font-size: 16px; font-weight: bold;'>
                                            Reset Password
                                        </a>
                                    </td>
                                </tr>
                            </table>
                            
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
                            <p style='color: #888888; margin: 0; font-size: 14px;'>
                                © 2025 SociusFit. All rights reserved.
                            </p>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>
</body>
</html>";
    }

    private string GetWelcomeTemplate(string firstName)
    {
        return $@"
<!DOCTYPE html>
<html>
<head>
    <meta charset='utf-8'>
    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
    <title>Welcome to SociusFit</title>
</head>
<body style='margin: 0; padding: 0; font-family: Arial, sans-serif; background-color: #f4f4f4;'>
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
                                <ul style='color: #555555; line-height: 1.8; margin: 0; padding-left: 20px;'>
                                    <li>🏃 Add your favorite sports and skill levels</li>
                                    <li>📍 Set your location to find nearby partners</li>
                                    <li>🤝 Connect with people who share your passion</li>
                                    <li>📅 Organize your first sports session</li>
                                </ul>
                            </div>
                            
                            <p style='color: #555555; line-height: 1.6; margin: 20px 0; font-size: 16px;'>
                                Ready to play? Log in now and start connecting!
                            </p>
                        </td>
                    </tr>
                    
                    <tr>
                        <td style='background-color: #f8f8f8; padding: 20px 30px; text-align: center; border-radius: 0 0 8px 8px;'>
                            <p style='color: #888888; margin: 0; font-size: 14px;'>
                                © 2025 SociusFit. All rights reserved.
                            </p>
                            <p style='color: #888888; margin: 10px 0 0 0; font-size: 12px;'>
                                Find your sport partner, play better together!
                            </p>
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