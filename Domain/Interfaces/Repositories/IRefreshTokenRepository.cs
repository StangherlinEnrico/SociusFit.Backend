using Domain.Entities;

namespace Domain.Interfaces.Repositories;

/// <summary>
/// Repository interface for RefreshToken entity
/// </summary>
public interface IRefreshTokenRepository : IRepository<RefreshToken>
{
    Task<RefreshToken?> GetByTokenAsync(string token, CancellationToken cancellationToken = default);

    Task<IEnumerable<RefreshToken>> GetActiveTokensByUserIdAsync(int userId, CancellationToken cancellationToken = default);

    Task RevokeAllUserTokensAsync(int userId, string revokedByIp, CancellationToken cancellationToken = default);

    Task DeleteExpiredTokensAsync(CancellationToken cancellationToken = default);
}