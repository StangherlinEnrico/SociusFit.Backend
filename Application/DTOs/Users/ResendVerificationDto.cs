namespace Application.DTOs.Users;

/// <summary>
/// DTO for resend verification email
/// </summary>
public class ResendVerificationDto
{
    public string Email { get; set; } = string.Empty;
}