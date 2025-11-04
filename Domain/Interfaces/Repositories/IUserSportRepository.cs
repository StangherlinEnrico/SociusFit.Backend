using Domain.Entities;

namespace Domain.Interfaces.Repositories;

/// <summary>
/// Specialized repository interface for UserSport entity
/// </summary>
public interface IUserSportRepository : IRepository<UserSport>
{
    Task<IEnumerable<UserSport>> GetByUserIdAsync(int userId, CancellationToken cancellationToken = default);

    Task<UserSport?> GetByUserAndSportAsync(
        int userId,
        int sportId,
        CancellationToken cancellationToken = default);

    Task<bool> UserHasSportAsync(int userId, int sportId, CancellationToken cancellationToken = default);

    Task<IEnumerable<UserSport>> GetBySportIdAsync(int sportId, CancellationToken cancellationToken = default);

    Task<IEnumerable<UserSport>> GetBySportAndLevelAsync(
        int sportId,
        int levelId,
        CancellationToken cancellationToken = default);
}