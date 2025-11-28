namespace Domain.Exceptions;

public class UserNotFoundException : DomainException
{
    public UserNotFoundException(Guid userId)
        : base($"User not found with ID: {userId}")
    {
    }

    public UserNotFoundException(string email)
        : base($"User not found with email: {email}")
    {
    }
}

public class UserAlreadyExistsException : DomainException
{
    public UserAlreadyExistsException(string email)
        : base($"User already exists with email: {email}")
    {
    }
}

public class InvalidUserDataException : DomainException
{
    public InvalidUserDataException(string message)
        : base(message)
    {
    }
}