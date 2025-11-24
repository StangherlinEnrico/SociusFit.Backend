namespace Application.DTOs.Users;

/// <summary>
/// DTO for password reset
/// </summary>
public class ResetPasswordDto
{
    /// <summary>
    /// Password reset token received via email
    /// </summary>
    public string Token { get; set; } = string.Empty;

    /// <summary>
    /// New password
    /// </summary>
    public string NewPassword { get; set; } = string.Empty;

    /// <summary>
    /// Confirm new password
    /// </summary>
    public string ConfirmPassword { get; set; } = string.Empty;
}