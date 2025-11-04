using Domain.Entities;

namespace Domain.Interfaces.Repositories;

/// <summary>
/// Specialized repository interface for Sport entity
/// </summary>
public interface ISportRepository : IRepository<Sport>
{
    Task<Sport?> GetByNameAsync(string name, CancellationToken cancellationToken = default);

    Task<bool> NameExistsAsync(string name, int? excludeId = null, CancellationToken cancellationToken = default);

    Task<IEnumerable<Sport>> GetPopularSportsAsync(int count, CancellationToken cancellationToken = default);
}