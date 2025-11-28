using Domain.Events;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Events.Handlers;

public class UserCreatedEventHandler : IDomainEventHandler<UserCreatedEvent>
{
    private readonly ILogger<UserCreatedEventHandler> _logger;

    public UserCreatedEventHandler(ILogger<UserCreatedEventHandler> logger)
    {
        _logger = logger;
    }

    public Task HandleAsync(UserCreatedEvent domainEvent, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation(
            "User created: UserId={UserId}, Email={Email}, Name={FirstName} {LastName}",
            domainEvent.UserId,
            domainEvent.Email,
            domainEvent.FirstName,
            domainEvent.LastName
        );

        return Task.CompletedTask;
    }
}