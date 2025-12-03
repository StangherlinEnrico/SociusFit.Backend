using Domain.Entities;

namespace Domain.Repositories;

public interface IMatchRepository
{
    Task<Match?> GetByIdAsync(Guid matchId, CancellationToken cancellationToken = default);
    Task<Match?> GetByUsersAsync(Guid user1Id, Guid user2Id, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(Guid user1Id, Guid user2Id, CancellationToken cancellationToken = default);
    Task<Match> CreateAsync(Match match, CancellationToken cancellationToken = default);
    Task<List<Match>> GetUserMatchesAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<List<Guid>> GetMatchedUserIdsAsync(Guid userId, CancellationToken cancellationToken = default);
}