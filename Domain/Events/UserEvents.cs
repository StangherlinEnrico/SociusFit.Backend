namespace Domain.Events;

public class UserCreatedEvent : DomainEvent
{
    public Guid UserId { get; private set; }
    public string Email { get; private set; }
    public string FirstName { get; private set; }
    public string LastName { get; private set; }

    public UserCreatedEvent(Guid userId, string email, string firstName, string lastName)
    {
        UserId = userId;
        Email = email;
        FirstName = firstName;
        LastName = lastName;
    }
}

public class UserUpdatedEvent : DomainEvent
{
    public Guid UserId { get; private set; }
    public string FirstName { get; private set; }
    public string LastName { get; private set; }

    public UserUpdatedEvent(Guid userId, string firstName, string lastName)
    {
        UserId = userId;
        FirstName = firstName;
        LastName = lastName;
    }
}

public class UserDeletedEvent : DomainEvent
{
    public Guid UserId { get; private set; }
    public string Email { get; private set; }

    public UserDeletedEvent(Guid userId, string email)
    {
        UserId = userId;
        Email = email;
    }
}