using Domain.Entities;
using Domain.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

/// <summary>
/// Repository implementation for Session entity
/// </summary>
public class SessionRepository : Repository<Session>, ISessionRepository
{
    public SessionRepository(DbContext context) : base(context)
    {
    }

    public async Task<Session?> GetByTokenAsync(string token, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .FirstOrDefaultAsync(s => s.Token == token, cancellationToken);
    }

    public async Task<IEnumerable<Session>> GetActiveSessionsByUserIdAsync(int userId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(s => s.UserId == userId && s.ExpiresAt > DateTime.UtcNow)
            .OrderByDescending(s => s.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Session>> GetExpiredSessionsAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(s => s.ExpiresAt <= DateTime.UtcNow)
            .ToListAsync(cancellationToken);
    }

    public async Task RemoveExpiredSessionsAsync(CancellationToken cancellationToken = default)
    {
        var expiredSessions = await GetExpiredSessionsAsync(cancellationToken);
        _dbSet.RemoveRange(expiredSessions);
    }

    public async Task RemoveUserSessionsAsync(int userId, CancellationToken cancellationToken = default)
    {
        var userSessions = await _dbSet
            .Where(s => s.UserId == userId)
            .ToListAsync(cancellationToken);

        _dbSet.RemoveRange(userSessions);
    }
}
