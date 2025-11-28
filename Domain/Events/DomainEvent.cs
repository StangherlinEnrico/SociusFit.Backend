namespace Domain.Events;

public abstract class DomainEvent
{
    public Guid EventId { get; private set; }
    public DateTime OccurredOn { get; private set; }

    protected DomainEvent()
    {
        EventId = Guid.NewGuid();
        OccurredOn = DateTime.UtcNow;
    }
}