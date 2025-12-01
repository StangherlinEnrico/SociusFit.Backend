using Domain.Entities;
using Domain.Repositories;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class ProfileRepository : IProfileRepository
{
    private readonly SociusFitDbContext _context;

    public ProfileRepository(SociusFitDbContext context)
    {
        _context = context;
    }

    public async Task<Profile?> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await _context.Profiles
            .Include(p => p.ProfileSports)
            .FirstOrDefaultAsync(p => p.UserId == userId, cancellationToken);
    }

    public async Task<Profile?> GetByIdAsync(Guid profileId, CancellationToken cancellationToken = default)
    {
        return await _context.Profiles
            .Include(p => p.ProfileSports)
            .FirstOrDefaultAsync(p => p.Id == profileId, cancellationToken);
    }

    public async Task<bool> ExistsByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await _context.Profiles
            .AnyAsync(p => p.UserId == userId, cancellationToken);
    }

    public async Task<Profile> CreateAsync(Profile profile, CancellationToken cancellationToken = default)
    {
        await _context.Profiles.AddAsync(profile, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return profile;
    }

    public async Task<Profile> UpdateAsync(Profile profile, CancellationToken cancellationToken = default)
    {
        _context.Profiles.Update(profile);
        await _context.SaveChangesAsync(cancellationToken);
        return profile;
    }

    public async Task DeleteAsync(Guid profileId, CancellationToken cancellationToken = default)
    {
        var profile = await GetByIdAsync(profileId, cancellationToken);
        if (profile != null)
        {
            _context.Profiles.Remove(profile);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }

    public async Task<IEnumerable<Profile>> GetProfilesByIdsAsync(
        IEnumerable<Guid> profileIds,
        CancellationToken cancellationToken = default)
    {
        return await _context.Profiles
            .Include(p => p.ProfileSports)
            .Where(p => profileIds.Contains(p.Id))
            .ToListAsync(cancellationToken);
    }
}