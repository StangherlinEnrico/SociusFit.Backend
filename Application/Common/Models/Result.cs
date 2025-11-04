namespace Application.Common.Models;

/// <summary>
/// Generic response wrapper for API responses
/// </summary>
public class Result<T>
{
    public bool Success { get; set; }
    public T? Data { get; set; }
    public string? Message { get; set; }
    public List<string> Errors { get; set; } = new();

    public static Result<T> SuccessResult(T data, string? message = null)
    {
        return new Result<T>
        {
            Success = true,
            Data = data,
            Message = message
        };
    }

    public static Result<T> FailureResult(string message, List<string>? errors = null)
    {
        return new Result<T>
        {
            Success = false,
            Message = message,
            Errors = errors ?? new List<string>()
        };
    }

    public static Result<T> FailureResult(List<string> errors)
    {
        return new Result<T>
        {
            Success = false,
            Errors = errors
        };
    }
}

/// <summary>
/// Result without data
/// </summary>
public class Result
{
    public bool Success { get; set; }
    public string? Message { get; set; }
    public List<string> Errors { get; set; } = new();

    public static Result SuccessResult(string? message = null)
    {
        return new Result
        {
            Success = true,
            Message = message
        };
    }

    public static Result FailureResult(string message, List<string>? errors = null)
    {
        return new Result
        {
            Success = false,
            Message = message,
            Errors = errors ?? new List<string>()
        };
    }

    public static Result FailureResult(List<string> errors)
    {
        return new Result
        {
            Success = false,
            Errors = errors
        };
    }
}

/// <summary>
/// Paginated result
/// </summary>
public class PaginatedResult<T>
{
    public List<T> Items { get; set; } = new();
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public int TotalPages { get; set; }
    public int TotalCount { get; set; }
    public bool HasPreviousPage => PageNumber > 1;
    public bool HasNextPage => PageNumber < TotalPages;

    public PaginatedResult(List<T> items, int count, int pageNumber, int pageSize)
    {
        Items = items;
        TotalCount = count;
        PageNumber = pageNumber;
        PageSize = pageSize;
        TotalPages = (int)Math.Ceiling(count / (double)pageSize);
    }
}