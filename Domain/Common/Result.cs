namespace Domain.Common;

public class Result<T>
{
    public bool IsSuccess { get; private set; }
    public T? Value { get; private set; }
    public string? Error { get; private set; }
    public IReadOnlyCollection<string> Errors { get; private set; }

    private Result(bool isSuccess, T? value, string? error, IEnumerable<string>? errors)
    {
        IsSuccess = isSuccess;
        Value = value;
        Error = error;
        Errors = (errors ?? Array.Empty<string>()).ToList().AsReadOnly();
    }

    public static Result<T> Success(T value)
        => new Result<T>(true, value, null, null);

    public static Result<T> Failure(string error)
        => new Result<T>(false, default, error, new[] { error });

    public static Result<T> Failure(IEnumerable<string> errors)
        => new Result<T>(false, default, string.Join("; ", errors), errors);

    public TResult Match<TResult>(
        Func<T, TResult> onSuccess,
        Func<string, TResult> onFailure)
    {
        return IsSuccess ? onSuccess(Value!) : onFailure(Error!);
    }
}

public class Result
{
    public bool IsSuccess { get; private set; }
    public string? Error { get; private set; }
    public IReadOnlyCollection<string> Errors { get; private set; }

    private Result(bool isSuccess, string? error, IEnumerable<string>? errors)
    {
        IsSuccess = isSuccess;
        Error = error;
        Errors = (errors ?? Array.Empty<string>()).ToList().AsReadOnly();
    }

    public static Result Success()
        => new Result(true, null, null);

    public static Result Failure(string error)
        => new Result(false, error, new[] { error });

    public static Result Failure(IEnumerable<string> errors)
        => new Result(false, string.Join("; ", errors), errors);

    public TResult Match<TResult>(
        Func<TResult> onSuccess,
        Func<string, TResult> onFailure)
    {
        return IsSuccess ? onSuccess() : onFailure(Error!);
    }
}