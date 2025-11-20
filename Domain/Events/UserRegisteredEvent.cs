namespace Domain.Events;

public class UserRegisteredEvent(int userId, string email, string firstName, string lastName, bool isOAuthRegistration = false) : DomainEvent
{
    public int UserId { get; } = userId;
    public string Email { get; } = email;
    public string FirstName { get; } = firstName;
    public string LastName { get; } = lastName;
    public bool IsOAuthRegistration { get; } = isOAuthRegistration;
}
