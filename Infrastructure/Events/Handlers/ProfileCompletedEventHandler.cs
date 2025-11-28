using Domain.Events;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Events.Handlers;

public class ProfileCompletedEventHandler : IDomainEventHandler<ProfileCompletedEvent>
{
    private readonly ILogger<ProfileCompletedEventHandler> _logger;

    public ProfileCompletedEventHandler(ILogger<ProfileCompletedEventHandler> logger)
    {
        _logger = logger;
    }

    public Task HandleAsync(ProfileCompletedEvent domainEvent, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation(
            "Profile completed: ProfileId={ProfileId}, UserId={UserId}",
            domainEvent.ProfileId,
            domainEvent.UserId
        );

        return Task.CompletedTask;
    }
}