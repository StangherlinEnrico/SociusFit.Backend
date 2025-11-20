namespace Domain.Events;

public class UserDeletedEvent(int userId) : DomainEvent
{
    public int UserId { get; } = userId;
}
