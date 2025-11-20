namespace Domain.Exceptions;

/// <summary>
/// Base exception for domain-specific errors
/// </summary>
public abstract class DomainException : Exception
{
    protected DomainException(string message) : base(message) { }

    protected DomainException(string message, Exception innerException)
        : base(message, innerException) { }
}

/// <summary>
/// Exception thrown when an entity is not found
/// </summary>
public class EntityNotFoundException : DomainException
{
    public EntityNotFoundException(string entityName, object id)
        : base($"{entityName} with id '{id}' was not found.") { }

    public EntityNotFoundException(string message) : base(message) { }
}

/// <summary>
/// Exception thrown when an entity already exists
/// </summary>
public class EntityAlreadyExistsException : DomainException
{
    public EntityAlreadyExistsException(string entityName, string propertyName, object value)
        : base($"{entityName} with {propertyName} '{value}' already exists.") { }

    public EntityAlreadyExistsException(string message) : base(message) { }
}

/// <summary>
/// Exception thrown when a business rule is violated
/// </summary>
public class BusinessRuleValidationException : DomainException
{
    public BusinessRuleValidationException(string message) : base(message) { }

    public BusinessRuleValidationException(string message, Exception innerException)
        : base(message, innerException) { }
}

/// <summary>
/// Exception thrown when authentication fails
/// </summary>
public class AuthenticationException : DomainException
{
    public AuthenticationException(string message) : base(message) { }
}

/// <summary>
/// Exception thrown when authorization fails
/// </summary>
public class AuthorizationException : DomainException
{
    public AuthorizationException(string message) : base(message) { }
}
