namespace Application.DTOs.Consents;

/// <summary>
/// DTO for user consent response
/// </summary>
public class UserConsentDto
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string ConsentType { get; set; } = string.Empty;
    public bool IsGranted { get; set; }
    public DateTime? GrantedAt { get; set; }
    public DateTime? RevokedAt { get; set; }
    public DateTime CreatedAt { get; set; }
}

/// <summary>
/// DTO for granting consent
/// </summary>
public class GrantConsentDto
{
    public string ConsentType { get; set; } = string.Empty;
}

/// <summary>
/// DTO for revoking consent
/// </summary>
public class RevokeConsentDto
{
    public string ConsentType { get; set; } = string.Empty;
}