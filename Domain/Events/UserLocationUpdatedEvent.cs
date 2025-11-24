namespace Domain.Events;

/// <summary>
/// Domain event raised when user updates their location settings
/// </summary>
public class UserLocationUpdatedEvent(int userId, string? location, int? maxDistance) : DomainEvent
{
    public int UserId { get; } = userId;
    public string? Location { get; } = location;
    public int? MaxDistance { get; } = maxDistance;
}