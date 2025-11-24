namespace Application.DTOs.Users;

/// <summary>
/// DTO for forgot password request
/// </summary>
public class ForgotPasswordDto
{
    /// <summary>
    /// Email address of the account
    /// </summary>
    public string Email { get; set; } = string.Empty;
}