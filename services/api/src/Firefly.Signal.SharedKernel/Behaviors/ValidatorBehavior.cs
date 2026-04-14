using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Firefly.Signal.SharedKernel.Behaviors;

public sealed class ValidatorBehavior<TRequest, TResponse>(
    IEnumerable<IValidator<TRequest>> validators,
    ILogger<ValidatorBehavior<TRequest, TResponse>> logger) : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        if (!validators.Any())
        {
            return await next();
        }

        var validationResults = await Task.WhenAll(validators.Select(x => x.ValidateAsync(request, cancellationToken)));
        var failures = validationResults
            .SelectMany(x => x.Errors)
            .Where(x => x is not null)
            .ToList();

        if (failures.Count == 0)
        {
            return await next();
        }

        logger.LogWarning("Validation failed for {RequestName}: {@Failures}", typeof(TRequest).Name, failures);
        throw new ValidationException(failures);
    }
}
