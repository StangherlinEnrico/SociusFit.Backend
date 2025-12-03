using Domain.Entities;

namespace Domain.Repositories;

public interface ILikeRepository
{
    Task<Like?> GetLikeAsync(Guid likerUserId, Guid likedUserId, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(Guid likerUserId, Guid likedUserId, CancellationToken cancellationToken = default);
    Task<Like> CreateAsync(Like like, CancellationToken cancellationToken = default);
    Task<List<Guid>> GetLikedUserIdsAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<bool> HasMutualLikeAsync(Guid user1Id, Guid user2Id, CancellationToken cancellationToken = default);
}