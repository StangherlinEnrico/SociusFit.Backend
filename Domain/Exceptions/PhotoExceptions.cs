namespace Domain.Exceptions;

public class PhotoUploadFailedException : DomainException
{
    public PhotoUploadFailedException(string message)
        : base(message)
    {
    }

    public PhotoUploadFailedException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}

public class InvalidPhotoFormatException : DomainException
{
    public InvalidPhotoFormatException(string message)
        : base(message)
    {
    }
}

public class PhotoTooLargeException : DomainException
{
    public PhotoTooLargeException(int maxSizeInBytes)
        : base($"Photo size exceeds maximum allowed size of {maxSizeInBytes / (1024 * 1024)}MB")
    {
    }
}