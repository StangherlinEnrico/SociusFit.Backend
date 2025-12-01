using Domain.Entities;

namespace Domain.Repositories;

public interface ISportRepository
{
    Task<Sport?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Sport?> GetByNameAsync(string name, CancellationToken cancellationToken = default);
    Task<List<Sport>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Sport> CreateAsync(Sport sport, CancellationToken cancellationToken = default);
    Task<Sport> UpdateAsync(Sport sport, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}