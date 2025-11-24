namespace Domain.Events;

public class UserPasswordResetRequestedEvent(int userId, string email) : DomainEvent
{
    public int UserId { get; } = userId;
    public string Email { get; } = email;
}