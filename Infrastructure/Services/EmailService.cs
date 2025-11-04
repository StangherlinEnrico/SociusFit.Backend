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

    public EmailService(IConfiguration configuration)
    {
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
    }

    public async Task SendVerificationEmailAsync(
        string email,
        string verificationLink,
        CancellationToken cancellationToken = default)
    {
        var subject = "Verify Your Email Address";
        var body = $@"
            <h2>Email Verification</h2>
            <p>Thank you for registering! Please verify your email address by clicking the link below:</p>
            <p><a href='{verificationLink}'>Verify Email</a></p>
            <p>If you didn't create this account, you can safely ignore this email.</p>
        ";

        await SendEmailAsync(email, subject, body, cancellationToken);
    }

    public async Task SendPasswordResetEmailAsync(
        string email,
        string resetLink,
        CancellationToken cancellationToken = default)
    {
        var subject = "Reset Your Password";
        var body = $@"
            <h2>Password Reset Request</h2>
            <p>We received a request to reset your password. Click the link below to reset it:</p>
            <p><a href='{resetLink}'>Reset Password</a></p>
            <p>This link will expire in 24 hours.</p>
            <p>If you didn't request a password reset, you can safely ignore this email.</p>
        ";

        await SendEmailAsync(email, subject, body, cancellationToken);
    }

    public async Task SendWelcomeEmailAsync(
        string email,
        string firstName,
        CancellationToken cancellationToken = default)
    {
        var subject = "Welcome!";
        var body = $@"
            <h2>Welcome, {firstName}!</h2>
            <p>Thank you for joining us. We're excited to have you on board!</p>
            <p>Get started by exploring our platform and connecting with other users.</p>
        ";

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
        var fromName = _configuration["Email:FromName"] ?? "Your App";

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
}