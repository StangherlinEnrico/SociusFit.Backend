using Domain.Entities;
using Domain.Repositories;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class MatchRepository : IMatchRepository
{
    private readonly SociusFitDbContext _context;

    public MatchRepository(SociusFitDbContext context)
    {
        _context = context;
    }

    public async Task<Match?> GetByIdAsync(Guid matchId, CancellationToken cancellationToken = default)
    {
        return await _context.Matches
            .FirstOrDefaultAsync(m => m.Id == matchId, cancellationToken);
    }

    public async Task<Match?> GetByUsersAsync(Guid user1Id, Guid user2Id, CancellationToken cancellationToken = default)
    {
        return await _context.Matches
            .FirstOrDefaultAsync(m =>
                (m.User1Id == user1Id && m.User2Id == user2Id) ||
                (m.User1Id == user2Id && m.User2Id == user1Id),
                cancellationToken);
    }

    public async Task<bool> ExistsAsync(Guid user1Id, Guid user2Id, CancellationToken cancellationToken = default)
    {
        return await _context.Matches
            .AnyAsync(m =>
                (m.User1Id == user1Id && m.User2Id == user2Id) ||
                (m.User1Id == user2Id && m.User2Id == user1Id),
                cancellationToken);
    }

    public async Task<Match> CreateAsync(Match match, CancellationToken cancellationToken = default)
    {
        await _context.Matches.AddAsync(match, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return match;
    }

    public async Task<List<Match>> GetUserMatchesAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await _context.Matches
            .Where(m => m.User1Id == userId || m.User2Id == userId)
            .OrderByDescending(m => m.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<Guid>> GetMatchedUserIdsAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var matches = await GetUserMatchesAsync(userId, cancellationToken);
        return matches.Select(m => m.GetOtherUserId(userId)).ToList();
    }
}