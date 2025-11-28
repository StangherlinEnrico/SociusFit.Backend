using Domain.Entities;

namespace Domain.Repositories;

public interface IProfileRepository
{
    Task<Profile?> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<Profile?> GetByIdAsync(Guid profileId, CancellationToken cancellationToken = default);
    Task<bool> ExistsByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<Profile> CreateAsync(Profile profile, CancellationToken cancellationToken = default);
    Task<Profile> UpdateAsync(Profile profile, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid profileId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Profile>> GetProfilesByIdsAsync(IEnumerable<Guid> profileIds, CancellationToken cancellationToken = default);
}