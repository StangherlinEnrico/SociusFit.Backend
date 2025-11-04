using Domain.Entities;

namespace Domain.Interfaces.Repositories;

/// <summary>
/// Specialized repository interface for User entity
/// </summary>
public interface IUserRepository : IRepository<User>
{
    Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);

    Task<User?> GetByProviderAsync(string provider, string providerId, CancellationToken cancellationToken = default);

    Task<bool> EmailExistsAsync(string email, CancellationToken cancellationToken = default);

    Task<IEnumerable<User>> GetUsersWithinDistanceAsync(
        decimal latitude,
        decimal longitude,
        int maxDistanceKm,
        CancellationToken cancellationToken = default);

    Task<IEnumerable<User>> GetUsersBySportAsync(int sportId, CancellationToken cancellationToken = default);

    Task<IEnumerable<User>> GetUsersBySportAndLevelAsync(
        int sportId,
        int levelId,
        CancellationToken cancellationToken = default);

    Task<User?> GetByIdWithSportsAsync(int id, CancellationToken cancellationToken = default);

    Task<User?> GetByIdWithSessionsAsync(int id, CancellationToken cancellationToken = default);

    Task<User?> GetByIdWithConsentsAsync(int id, CancellationToken cancellationToken = default);
}