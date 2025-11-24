namespace Domain.Events;

public class UserPasswordChangedEvent(int userId) : DomainEvent
{
    public int UserId { get; } = userId;
}