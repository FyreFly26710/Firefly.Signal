using MediatR;
using Microsoft.Extensions.Logging;

namespace Firefly.Signal.SharedKernel.Behaviors;

public sealed class LoggingBehavior<TRequest, TResponse>(
    ILogger<LoggingBehavior<TRequest, TResponse>> logger) : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Handling request {RequestName} ({@Request})", typeof(TRequest).Name, request);

        var response = await next();

        logger.LogInformation("Handled request {RequestName} ({@Response})", typeof(TRequest).Name, response);
        return response;
    }
}
