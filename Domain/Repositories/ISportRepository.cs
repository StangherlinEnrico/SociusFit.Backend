using Domain.Entities;

namespace Domain.Repositories;

public interface ISportRepository
{
    Task<Sport?> GetByIdAsync(Guid sportId, CancellationToken cancellationToken = default);
    Task<Sport?> GetByNameAsync(string sportName, CancellationToken cancellationToken = default);
    Task<IEnumerable<Sport>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<Sport>> GetByIdsAsync(IEnumerable<Guid> sportIds, CancellationToken cancellationToken = default);
    Task<Sport> CreateAsync(Sport sport, CancellationToken cancellationToken = default);
    Task<IEnumerable<Sport>> GetByProfileIdAsync(Guid profileId, CancellationToken cancellationToken = default);
}