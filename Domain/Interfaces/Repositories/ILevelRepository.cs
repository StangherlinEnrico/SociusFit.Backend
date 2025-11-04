using Domain.Entities;

namespace Domain.Interfaces.Repositories;

/// <summary>
/// Specialized repository interface for Level entity
/// </summary>
public interface ILevelRepository : IRepository<Level>
{
    Task<Level?> GetByNameAsync(string name, CancellationToken cancellationToken = default);

    Task<bool> NameExistsAsync(string name, int? excludeId = null, CancellationToken cancellationToken = default);
}