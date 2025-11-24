namespace Domain.Events;

public class UserLoggedInEvent(int userId, string provider, string? ipAddress) : DomainEvent
{
    public int UserId { get; } = userId;
    public string Provider { get; } = provider; // "email", "google", "apple"
    public string? IpAddress { get; } = ipAddress;
}