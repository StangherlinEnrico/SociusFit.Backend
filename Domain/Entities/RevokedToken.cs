using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities;

/// <summary>
/// Represents a JWT token that has been revoked/blacklisted
/// Used for logout functionality to invalidate tokens server-side
/// </summary>
public class RevokedToken
{
    [Key]
    public Guid Id { get; private set; }

    /// <summary>
    /// The JTI (JWT ID) claim from the token - unique identifier for this token
    /// </summary>
    [Required]
    [MaxLength(100)]
    public string TokenId { get; private set; }

    /// <summary>
    /// User ID who owned this token
    /// </summary>
    [Required]
    public Guid UserId { get; private set; }

    /// <summary>
    /// When the token was revoked (logout time)
    /// </summary>
    [Required]
    public DateTime RevokedAt { get; private set; }

    /// <summary>
    /// When the token expires naturally (after this, can be removed from blacklist)
    /// </summary>
    [Required]
    public DateTime ExpiresAt { get; private set; }

    /// <summary>
    /// Reason for revocation (e.g., "User logout", "Security breach", "Admin action")
    /// </summary>
    [MaxLength(200)]
    public string? Reason { get; private set; }

    private RevokedToken() { } // EF Core

    public RevokedToken(
        string tokenId,
        Guid userId,
        DateTime expiresAt,
        string? reason = null)
    {
        Id = Guid.NewGuid();
        TokenId = tokenId;
        UserId = userId;
        RevokedAt = DateTime.UtcNow;
        ExpiresAt = expiresAt;
        Reason = reason ?? "User logout";
    }

    /// <summary>
    /// Check if this revoked token can be safely removed from blacklist
    /// (token has expired naturally)
    /// </summary>
    public bool CanBeRemoved() => DateTime.UtcNow > ExpiresAt;
}