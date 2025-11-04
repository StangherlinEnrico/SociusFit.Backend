using Application.Common.Models;
using FluentValidation;
using MediatR;

namespace Application.Common.Behaviors;

/// <summary>
/// MediatR pipeline behavior for automatic validation using FluentValidation
/// </summary>
public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
    {
        _validators = validators;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        if (!_validators.Any())
        {
            return await next();
        }

        var context = new ValidationContext<TRequest>(request);

        var validationResults = await Task.WhenAll(
            _validators.Select(v => v.ValidateAsync(context, cancellationToken)));

        var failures = validationResults
            .SelectMany(r => r.Errors)
            .Where(f => f != null)
            .ToList();

        if (failures.Any())
        {
            var errors = failures.Select(f => f.ErrorMessage).ToList();

            // Check if TResponse is a Result type
            var responseType = typeof(TResponse);

            if (responseType.IsGenericType && responseType.GetGenericTypeDefinition() == typeof(Result<>))
            {
                var dataType = responseType.GetGenericArguments()[0];
                var resultType = typeof(Result<>).MakeGenericType(dataType);
                var failureMethod = resultType.GetMethod(nameof(Result<object>.FailureResult),
                    new[] { typeof(List<string>) });

                if (failureMethod != null)
                {
                    var result = failureMethod.Invoke(null, new object[] { errors });
                    return (TResponse)result!;
                }
            }
            else if (responseType == typeof(Result))
            {
                var result = Result.FailureResult(errors);
                return (TResponse)(object)result;
            }

            throw new ValidationException(failures);
        }

        return await next();
    }
}