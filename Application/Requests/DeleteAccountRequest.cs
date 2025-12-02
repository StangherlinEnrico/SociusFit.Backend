namespace Application.Requests;

/// <summary>
/// Request to delete user account
/// Requires password confirmation for security
/// </summary>
public class DeleteAccountRequest
{
    /// <summary>
    /// Current password for confirmation
    /// REQUIRED for security - prevents accidental deletion
    /// </summary>
    public string Password { get; set; } = string.Empty;

    /// <summary>
    /// Optional reason for deletion (for analytics/feedback)
    /// </summary>
    public string? Reason { get; set; }

    /// <summary>
    /// Confirmation flag - user must explicitly confirm
    /// </summary>
    public bool ConfirmDeletion { get; set; }
}