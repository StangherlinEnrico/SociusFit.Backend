using Domain.Entities;

namespace Domain.Interfaces.Repositories;

/// <summary>
/// Specialized repository interface for Session entity
/// </summary>
public interface ISessionRepository : IRepository<Session>
{
    Task<Session?> GetByTokenAsync(string token, CancellationToken cancellationToken = default);

    Task<IEnumerable<Session>> GetActiveSessionsByUserIdAsync(int userId, CancellationToken cancellationToken = default);

    Task<IEnumerable<Session>> GetExpiredSessionsAsync(CancellationToken cancellationToken = default);

    Task RemoveExpiredSessionsAsync(CancellationToken cancellationToken = default);

    Task RemoveUserSessionsAsync(int userId, CancellationToken cancellationToken = default);
}