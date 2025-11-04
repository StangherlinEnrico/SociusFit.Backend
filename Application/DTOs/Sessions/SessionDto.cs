namespace Application.DTOs.Sessions;

/// <summary>
/// DTO for session response
/// </summary>
public class SessionDto
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string Token { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
    public DateTime CreatedAt { get; set; }
    public bool IsExpired => DateTime.UtcNow > ExpiresAt;
}
