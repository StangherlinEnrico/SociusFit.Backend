using Domain.Entities;

namespace Domain.Repositories;

public interface IMessageRepository
{
    Task<Message?> GetByIdAsync(Guid messageId, CancellationToken cancellationToken = default);
    Task<List<Message>> GetByMatchIdAsync(Guid matchId, int pageSize = 50, int pageNumber = 1, CancellationToken cancellationToken = default);
    Task<Message> CreateAsync(Message message, CancellationToken cancellationToken = default);
    Task<int> GetUnreadCountAsync(Guid matchId, Guid userId, CancellationToken cancellationToken = default);
    Task MarkMessagesAsReadAsync(Guid matchId, Guid userId, CancellationToken cancellationToken = default);
}