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

// User Events

public class UserRegisteredEvent : DomainEvent
{
    public int UserId { get; }
    public string Email { get; }
    public string FirstName { get; }
    public string LastName { get; }
    public bool IsOAuthRegistration { get; }

    public UserRegisteredEvent(int userId, string email, string firstName, string lastName, bool isOAuthRegistration = false)
    {
        UserId = userId;
        Email = email;
        FirstName = firstName;
        LastName = lastName;
        IsOAuthRegistration = isOAuthRegistration;
    }
}

public class UserEmailVerifiedEvent : DomainEvent
{
    public int UserId { get; }
    public string Email { get; }

    public UserEmailVerifiedEvent(int userId, string email)
    {
        UserId = userId;
        Email = email;
    }
}

public class UserProfileUpdatedEvent : DomainEvent
{
    public int UserId { get; }

    public UserProfileUpdatedEvent(int userId)
    {
        UserId = userId;
    }
}

public class UserDeletedEvent : DomainEvent
{
    public int UserId { get; }

    public UserDeletedEvent(int userId)
    {
        UserId = userId;
    }
}

// Session Events

public class SessionCreatedEvent : DomainEvent
{
    public int SessionId { get; }
    public int UserId { get; }
    public DateTime ExpiresAt { get; }

    public SessionCreatedEvent(int sessionId, int userId, DateTime expiresAt)
    {
        SessionId = sessionId;
        UserId = userId;
        ExpiresAt = expiresAt;
    }
}

public class SessionExpiredEvent : DomainEvent
{
    public int SessionId { get; }
    public int UserId { get; }

    public SessionExpiredEvent(int sessionId, int userId)
    {
        SessionId = sessionId;
        UserId = userId;
    }
}

// Consent Events

public class ConsentGrantedEvent : DomainEvent
{
    public int UserId { get; }
    public string ConsentType { get; }

    public ConsentGrantedEvent(int userId, string consentType)
    {
        UserId = userId;
        ConsentType = consentType;
    }
}

public class ConsentRevokedEvent : DomainEvent
{
    public int UserId { get; }
    public string ConsentType { get; }

    public ConsentRevokedEvent(int userId, string consentType)
    {
        UserId = userId;
        ConsentType = consentType;
    }
}

// Sport Events

public class UserSportAddedEvent : DomainEvent
{
    public int UserId { get; }
    public int SportId { get; }
    public int LevelId { get; }

    public UserSportAddedEvent(int userId, int sportId, int levelId)
    {
        UserId = userId;
        SportId = sportId;
        LevelId = levelId;
    }
}

public class UserSportLevelUpdatedEvent : DomainEvent
{
    public int UserId { get; }
    public int SportId { get; }
    public int OldLevelId { get; }
    public int NewLevelId { get; }

    public UserSportLevelUpdatedEvent(int userId, int sportId, int oldLevelId, int newLevelId)
    {
        UserId = userId;
        SportId = sportId;
        OldLevelId = oldLevelId;
        NewLevelId = newLevelId;
    }
}

public class UserSportRemovedEvent : DomainEvent
{
    public int UserId { get; }
    public int SportId { get; }

    public UserSportRemovedEvent(int userId, int sportId)
    {
        UserId = userId;
        SportId = sportId;
    }
}