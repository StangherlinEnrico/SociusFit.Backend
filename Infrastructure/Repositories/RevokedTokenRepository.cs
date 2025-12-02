using Domain.Entities;
using Domain.Repositories;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class RevokedTokenRepository : IRevokedTokenRepository
{
    private readonly SociusFitDbContext _context;

    public RevokedTokenRepository(SociusFitDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(RevokedToken revokedToken, CancellationToken cancellationToken = default)
    {
        await _context.RevokedTokens.AddAsync(revokedToken, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> IsTokenRevokedAsync(string tokenId, CancellationToken cancellationToken = default)
    {
        return await _context.RevokedTokens
            .AnyAsync(rt => rt.TokenId == tokenId, cancellationToken);
    }

    public async Task RemoveExpiredTokensAsync(CancellationToken cancellationToken = default)
    {
        var expiredTokens = await _context.RevokedTokens
            .Where(rt => rt.ExpiresAt < DateTime.UtcNow)
            .ToListAsync(cancellationToken);

        if (expiredTokens.Any())
        {
            _context.RevokedTokens.RemoveRange(expiredTokens);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }

    public async Task<List<RevokedToken>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await _context.RevokedTokens
            .Where(rt => rt.UserId == userId)
            .OrderByDescending(rt => rt.RevokedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task RevokeAllUserTokensAsync(Guid userId, string reason, CancellationToken cancellationToken = default)
    {
        // Note: This requires knowing all active tokens for a user
        // In a real system, you might need a separate UserSessions table
        // For now, we just mark reason for existing revoked tokens

        var userTokens = await GetByUserIdAsync(userId, cancellationToken);

        foreach (var token in userTokens)
        {
            // Update reason if not already set
            if (string.IsNullOrEmpty(token.Reason))
            {
                _context.Entry(token).Property(nameof(RevokedToken.Reason)).CurrentValue = reason;
            }
        }

        await _context.SaveChangesAsync(cancellationToken);
    }
}