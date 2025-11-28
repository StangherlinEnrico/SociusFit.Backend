using Domain.Events;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Events;

public class DomainEventDispatcher : IDomainEventDispatcher
{
    private readonly IServiceProvider _serviceProvider;

    public DomainEventDispatcher(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task DispatchAsync<TEvent>(TEvent domainEvent, CancellationToken cancellationToken = default)
        where TEvent : DomainEvent
    {
        var handlerType = typeof(IDomainEventHandler<>).MakeGenericType(domainEvent.GetType());

        using var scope = _serviceProvider.CreateScope();
        var handlers = scope.ServiceProvider.GetServices(handlerType);

        foreach (var handler in handlers)
        {
            var handleMethod = handlerType.GetMethod(nameof(IDomainEventHandler<DomainEvent>.HandleAsync));
            if (handleMethod != null)
            {
                var task = (Task)handleMethod.Invoke(handler, new object[] { domainEvent, cancellationToken })!;
                await task;
            }
        }
    }

    public async Task DispatchManyAsync(IEnumerable<DomainEvent> domainEvents, CancellationToken cancellationToken = default)
    {
        foreach (var domainEvent in domainEvents)
        {
            await DispatchAsync((dynamic)domainEvent, cancellationToken);
        }
    }
}