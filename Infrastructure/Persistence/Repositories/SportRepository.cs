using Domain.Entities;
using Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

public class SportRepository : ISportRepository
{
    private readonly SociusFitDbContext _context;

    public SportRepository(SociusFitDbContext context)
    {
        _context = context;
    }

    public async Task<Sport?> GetByIdAsync(Guid sportId, CancellationToken cancellationToken = default)
    {
        return await _context.Sports
            .FirstOrDefaultAsync(s => s.Id == sportId, cancellationToken);
    }

    public async Task<Sport?> GetByNameAsync(string sportName, CancellationToken cancellationToken = default)
    {
        return await _context.Sports
            .FirstOrDefaultAsync(s => s.Name.ToLower() == sportName.ToLower(), cancellationToken);
    }

    public async Task<IEnumerable<Sport>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Sports
            .OrderBy(s => s.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Sport>> GetByIdsAsync(
        IEnumerable<Guid> sportIds,
        CancellationToken cancellationToken = default)
    {
        return await _context.Sports
            .Where(s => sportIds.Contains(s.Id))
            .ToListAsync(cancellationToken);
    }

    public async Task<Sport> CreateAsync(Sport sport, CancellationToken cancellationToken = default)
    {
        await _context.Sports.AddAsync(sport, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return sport;
    }

    public async Task<IEnumerable<Sport>> GetByProfileIdAsync(
        Guid profileId,
        CancellationToken cancellationToken = default)
    {
        var profile = await _context.Profiles
            .Include(p => p.Sports)
            .FirstOrDefaultAsync(p => p.Id == profileId, cancellationToken);

        return profile?.Sports ?? new List<Sport>();
    }
}