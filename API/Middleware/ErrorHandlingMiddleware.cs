using System.Net;
using System.Text.Json;
using Domain.Exceptions;
using FluentValidation;

namespace API.Middleware;

/// <summary>
/// Global error handling middleware
/// </summary>
public class ErrorHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ErrorHandlingMiddleware> _logger;

    public ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unhandled exception occurred");
            await HandleExceptionAsync(context, ex);
        }
    }

    private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";

        var response = exception switch
        {
            ValidationException validationException => new
            {
                StatusCode = (int)HttpStatusCode.BadRequest,
                Success = false,
                Message = "Validation error",
                Errors = validationException.Errors.Select(e => e.ErrorMessage).ToList()
            },
            EntityNotFoundException notFoundException => new
            {
                StatusCode = (int)HttpStatusCode.NotFound,
                Success = false,
                Message = notFoundException.Message,
                Errors = new List<string>()
            },
            EntityAlreadyExistsException alreadyExistsException => new
            {
                StatusCode = (int)HttpStatusCode.Conflict,
                Success = false,
                Message = alreadyExistsException.Message,
                Errors = new List<string>()
            },
            BusinessRuleValidationException businessRuleException => new
            {
                StatusCode = (int)HttpStatusCode.BadRequest,
                Success = false,
                Message = businessRuleException.Message,
                Errors = new List<string>()
            },
            AuthenticationException authException => new
            {
                StatusCode = (int)HttpStatusCode.Unauthorized,
                Success = false,
                Message = authException.Message,
                Errors = new List<string>()
            },
            AuthorizationException authzException => new
            {
                StatusCode = (int)HttpStatusCode.Forbidden,
                Success = false,
                Message = authzException.Message,
                Errors = new List<string>()
            },
            InvalidSessionException sessionException => new
            {
                StatusCode = (int)HttpStatusCode.Unauthorized,
                Success = false,
                Message = sessionException.Message,
                Errors = new List<string>()
            },
            _ => new
            {
                StatusCode = (int)HttpStatusCode.InternalServerError,
                Success = false,
                Message = "An internal server error occurred",
                Errors = new List<string> { exception.Message }
            }
        };

        context.Response.StatusCode = response.StatusCode;

        var jsonResponse = JsonSerializer.Serialize(response, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        await context.Response.WriteAsync(jsonResponse);
    }
}