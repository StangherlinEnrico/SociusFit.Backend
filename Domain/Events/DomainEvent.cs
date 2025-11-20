namespace Domain.Events;

/// <summary>
/// Base interface for domain events
/// </summary>
public interface IDomainEvent
{
    DateTime OccurredOn { get; }
}

/// <summary>
/// Base class for domain events
/// </summary>
public abstract class DomainEvent : IDomainEvent
{
    public DateTime OccurredOn { get; }

    protected DomainEvent()
    {
        OccurredOn = DateTime.UtcNow;
    }
}
