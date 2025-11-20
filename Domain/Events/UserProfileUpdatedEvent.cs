namespace Domain.Events;

public class UserProfileUpdatedEvent(int userId) : DomainEvent
{
    public int UserId { get; } = userId;
}
