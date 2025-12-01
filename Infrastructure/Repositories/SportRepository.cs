using Domain.Entities;
using Domain.Repositories;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class SportRepository : ISportRepository
{
    private readonly SociusFitDbContext _context;

    public SportRepository(SociusFitDbContext context)
    {
        _context = context;
    }

    public async Task<Sport?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Sports
            .FirstOrDefaultAsync(s => s.Id == id, cancellationToken);
    }

    public async Task<Sport?> GetByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        return await _context.Sports
            .FirstOrDefaultAsync(s => s.Name == name, cancellationToken);
    }

    public async Task<List<Sport>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Sports
            .OrderBy(s => s.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<Sport> CreateAsync(Sport sport, CancellationToken cancellationToken = default)
    {
        _context.Sports.Add(sport);
        await _context.SaveChangesAsync(cancellationToken);
        return sport;
    }

    public async Task<Sport> UpdateAsync(Sport sport, CancellationToken cancellationToken = default)
    {
        _context.Sports.Update(sport);
        await _context.SaveChangesAsync(cancellationToken);
        return sport;
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var sport = await GetByIdAsync(id, cancellationToken);
        if (sport != null)
        {
            _context.Sports.Remove(sport);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}