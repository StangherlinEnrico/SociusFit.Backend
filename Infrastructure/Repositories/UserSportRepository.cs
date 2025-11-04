using Domain.Entities;
using Domain.Interfaces.Repositories;
using Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

/// <summary>
/// Repository implementation for UserSport entity
/// </summary>
public class UserSportRepository : Repository<UserSport>, IUserSportRepository
{
    public UserSportRepository(DbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<UserSport>> GetByUserIdAsync(int userId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(us => us.Sport)
            .Include(us => us.Level)
            .Where(us => us.UserId == userId)
            .ToListAsync(cancellationToken);
    }

    public async Task<UserSport?> GetByUserAndSportAsync(
        int userId,
        int sportId,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(us => us.Sport)
            .Include(us => us.Level)
            .FirstOrDefaultAsync(us => us.UserId == userId && us.SportId == sportId, cancellationToken);
    }

    public async Task<bool> UserHasSportAsync(int userId, int sportId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AnyAsync(us => us.UserId == userId && us.SportId == sportId, cancellationToken);
    }

    public async Task<IEnumerable<UserSport>> GetBySportIdAsync(int sportId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(us => us.User)
            .Include(us => us.Level)
            .Where(us => us.SportId == sportId)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<UserSport>> GetBySportAndLevelAsync(
        int sportId,
        int levelId,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(us => us.User)
            .Where(us => us.SportId == sportId && us.LevelId == levelId)
            .ToListAsync(cancellationToken);
    }
}