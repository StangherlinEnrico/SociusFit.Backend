using Domain.Entities;
using Domain.Repositories;
using Domain.Services;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace Infrastructure.Services;

public class NotificationService : INotificationService
{
    private readonly IDeviceTokenRepository _deviceTokenRepository;
    private readonly INotificationRepository _notificationRepository;
    private readonly ILogger<NotificationService> _logger;

    public NotificationService(
        IDeviceTokenRepository deviceTokenRepository,
        INotificationRepository notificationRepository,
        ILogger<NotificationService> logger)
    {
        _deviceTokenRepository = deviceTokenRepository;
        _notificationRepository = notificationRepository;
        _logger = logger;
    }

    public async Task SendMatchNotificationAsync(
        Guid userId,
        string matchedUserName,
        Guid matchId,
        CancellationToken cancellationToken = default)
    {
        var deviceToken = await _deviceTokenRepository.GetByUserIdAsync(userId, cancellationToken);

        if (deviceToken == null)
        {
            _logger.LogWarning("No device token found for user {UserId}", userId);
            return;
        }

        var data = JsonSerializer.Serialize(new
        {
            type = "new_match",
            match_id = matchId.ToString(),
            user_name = matchedUserName
        });

        var notification = new Notification(
            userId,
            "new_match",
            "Nuovo Match!",
            $"Hai un nuovo match con {matchedUserName}",
            data
        );

        await _notificationRepository.CreateAsync(notification, cancellationToken);

        notification.MarkAsSent();
        await _notificationRepository.CreateAsync(notification, cancellationToken);

        _logger.LogInformation(
            "Match notification sent to user {UserId} for match {MatchId}",
            userId,
            matchId);
    }

    public async Task SendMessageNotificationAsync(
        Guid userId,
        string senderName,
        Guid matchId,
        string messagePreview,
        CancellationToken cancellationToken = default)
    {
        var deviceToken = await _deviceTokenRepository.GetByUserIdAsync(userId, cancellationToken);

        if (deviceToken == null)
        {
            _logger.LogWarning("No device token found for user {UserId}", userId);
            return;
        }

        var data = JsonSerializer.Serialize(new
        {
            type = "new_message",
            match_id = matchId.ToString(),
            sender_name = senderName
        });

        var notification = new Notification(
            userId,
            "new_message",
            $"Nuovo messaggio da {senderName}",
            messagePreview.Length > 100 ? messagePreview.Substring(0, 100) + "..." : messagePreview,
            data
        );

        await _notificationRepository.CreateAsync(notification, cancellationToken);

        notification.MarkAsSent();
        await _notificationRepository.CreateAsync(notification, cancellationToken);

        _logger.LogInformation(
            "Message notification sent to user {UserId} for match {MatchId}",
            userId,
            matchId);
    }
}