using Domain.Events;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Events.Handlers;

public class ProfileCreatedEventHandler : IDomainEventHandler<ProfileCreatedEvent>
{
    private readonly ILogger<ProfileCreatedEventHandler> _logger;

    public ProfileCreatedEventHandler(ILogger<ProfileCreatedEventHandler> logger)
    {
        _logger = logger;
    }

    public Task HandleAsync(ProfileCreatedEvent domainEvent, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation(
            "Profile created: ProfileId={ProfileId}, UserId={UserId}, Age={Age}, City={City}",
            domainEvent.ProfileId,
            domainEvent.UserId,
            domainEvent.Age,
            domainEvent.City
        );

        return Task.CompletedTask;
    }
}