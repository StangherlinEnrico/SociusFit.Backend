namespace Domain.Events;

public interface IDomainEventDispatcher
{
    Task DispatchAsync<TEvent>(TEvent domainEvent, CancellationToken cancellationToken = default)
        where TEvent : DomainEvent;

    Task DispatchManyAsync(IEnumerable<DomainEvent> domainEvents, CancellationToken cancellationToken = default);
}