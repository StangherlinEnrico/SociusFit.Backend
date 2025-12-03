using Domain.Entities;
using Domain.Repositories;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class DeviceTokenRepository : IDeviceTokenRepository
{
    private readonly SociusFitDbContext _context;

    public DeviceTokenRepository(SociusFitDbContext context)
    {
        _context = context;
    }

    public async Task<DeviceToken?> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await _context.DeviceTokens
            .Where(dt => dt.UserId == userId && dt.IsActive)
            .OrderByDescending(dt => dt.UpdatedAt)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<DeviceToken?> GetByTokenAsync(string token, CancellationToken cancellationToken = default)
    {
        return await _context.DeviceTokens
            .FirstOrDefaultAsync(dt => dt.Token == token, cancellationToken);
    }

    public async Task<List<DeviceToken>> GetActiveByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await _context.DeviceTokens
            .Where(dt => dt.UserId == userId && dt.IsActive)
            .ToListAsync(cancellationToken);
    }

    public async Task<DeviceToken> CreateAsync(DeviceToken deviceToken, CancellationToken cancellationToken = default)
    {
        await _context.DeviceTokens.AddAsync(deviceToken, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return deviceToken;
    }

    public async Task<DeviceToken> UpdateAsync(DeviceToken deviceToken, CancellationToken cancellationToken = default)
    {
        _context.DeviceTokens.Update(deviceToken);
        await _context.SaveChangesAsync(cancellationToken);
        return deviceToken;
    }

    public async Task DeactivateAllByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var tokens = await _context.DeviceTokens
            .Where(dt => dt.UserId == userId && dt.IsActive)
            .ToListAsync(cancellationToken);

        foreach (var token in tokens)
        {
            token.Deactivate();
        }

        await _context.SaveChangesAsync(cancellationToken);
    }
}