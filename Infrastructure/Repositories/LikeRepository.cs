using Domain.Entities;
using Domain.Repositories;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class LikeRepository : ILikeRepository
{
    private readonly SociusFitDbContext _context;

    public LikeRepository(SociusFitDbContext context)
    {
        _context = context;
    }

    public async Task<Like?> GetLikeAsync(Guid likerUserId, Guid likedUserId, CancellationToken cancellationToken = default)
    {
        return await _context.Likes
            .FirstOrDefaultAsync(l => l.LikerUserId == likerUserId && l.LikedUserId == likedUserId, cancellationToken);
    }

    public async Task<bool> ExistsAsync(Guid likerUserId, Guid likedUserId, CancellationToken cancellationToken = default)
    {
        return await _context.Likes
            .AnyAsync(l => l.LikerUserId == likerUserId && l.LikedUserId == likedUserId, cancellationToken);
    }

    public async Task<Like> CreateAsync(Like like, CancellationToken cancellationToken = default)
    {
        await _context.Likes.AddAsync(like, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return like;
    }

    public async Task<List<Guid>> GetLikedUserIdsAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await _context.Likes
            .Where(l => l.LikerUserId == userId)
            .Select(l => l.LikedUserId)
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> HasMutualLikeAsync(Guid user1Id, Guid user2Id, CancellationToken cancellationToken = default)
    {
        var like1 = await ExistsAsync(user1Id, user2Id, cancellationToken);
        var like2 = await ExistsAsync(user2Id, user1Id, cancellationToken);
        return like1 && like2;
    }
}