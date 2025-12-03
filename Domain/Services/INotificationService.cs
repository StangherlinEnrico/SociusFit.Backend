namespace Domain.Services;

public interface INotificationService
{
    Task SendMatchNotificationAsync(Guid userId, string matchedUserName, Guid matchId, CancellationToken cancellationToken = default);
    Task SendMessageNotificationAsync(Guid userId, string senderName, Guid matchId, string messagePreview, CancellationToken cancellationToken = default);
}