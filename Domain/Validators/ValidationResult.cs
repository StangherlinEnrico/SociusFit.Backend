namespace Domain.Validators;

public class ValidationResult
{
    public bool IsSuccess { get; private set; }
    public IReadOnlyCollection<string> Errors { get; private set; }

    private ValidationResult(bool isSuccess, IEnumerable<string> errors)
    {
        IsSuccess = isSuccess;
        Errors = errors.ToList().AsReadOnly();
    }

    public static ValidationResult Success()
        => new ValidationResult(true, Array.Empty<string>());

    public static ValidationResult Failure(IEnumerable<string> errors)
        => new ValidationResult(false, errors);

    public static ValidationResult Failure(string error)
        => new ValidationResult(false, new[] { error });

    public string GetErrorMessage()
        => string.Join("; ", Errors);
}