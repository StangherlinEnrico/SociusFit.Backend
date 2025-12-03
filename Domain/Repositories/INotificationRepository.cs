using Domain.Entities;

namespace Domain.Repositories;

public interface INotificationRepository
{
    Task<Notification> CreateAsync(Notification notification, CancellationToken cancellationToken = default);
    Task<List<Notification>> GetByUserIdAsync(Guid userId, int pageSize = 50, int pageNumber = 1, CancellationToken cancellationToken = default);
}