using Domain.Entities;

namespace Domain.Repositories;

public interface IDeviceTokenRepository
{
    Task<DeviceToken?> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<DeviceToken?> GetByTokenAsync(string token, CancellationToken cancellationToken = default);
    Task<List<DeviceToken>> GetActiveByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<DeviceToken> CreateAsync(DeviceToken deviceToken, CancellationToken cancellationToken = default);
    Task<DeviceToken> UpdateAsync(DeviceToken deviceToken, CancellationToken cancellationToken = default);
    Task DeactivateAllByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
}