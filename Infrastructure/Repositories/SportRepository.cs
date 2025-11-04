using Domain.Entities;
using Domain.Interfaces.Repositories;
using Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

/// <summary>
/// Repository implementation for Sport entity
/// </summary>
public class SportRepository : Repository<Sport>, ISportRepository
{
    public SportRepository(DbContext context) : base(context)
    {
    }

    public async Task<Sport?> GetByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .FirstOrDefaultAsync(s => s.Name == name, cancellationToken);
    }

    public async Task<bool> NameExistsAsync(string name, int? excludeId = null, CancellationToken cancellationToken = default)
    {
        var query = _dbSet.Where(s => s.Name == name);

        if (excludeId.HasValue)
        {
            query = query.Where(s => s.Id != excludeId.Value);
        }

        return await query.AnyAsync(cancellationToken);
    }

    public async Task<IEnumerable<Sport>> GetPopularSportsAsync(int count, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(s => s.UserSports)
            .OrderByDescending(s => s.UserSports.Count)
            .Take(count)
            .ToListAsync(cancellationToken);
    }
}