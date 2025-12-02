using Domain.Entities;

namespace Domain.Repositories;

public interface IRevokedTokenRepository
{
    /// <summary>
    /// Add a token to the blacklist
    /// </summary>
    Task AddAsync(RevokedToken revokedToken, CancellationToken cancellationToken = default);

    /// <summary>
    /// Check if a token is revoked (in blacklist)
    /// </summary>
    Task<bool> IsTokenRevokedAsync(string tokenId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Remove expired tokens from blacklist (cleanup)
    /// </summary>
    Task RemoveExpiredTokensAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Get all revoked tokens for a specific user
    /// </summary>
    Task<List<RevokedToken>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Revoke all tokens for a user (e.g., security breach, force logout all devices)
    /// </summary>
    Task RevokeAllUserTokensAsync(Guid userId, string reason, CancellationToken cancellationToken = default);
}