using MediatR;
using Microsoft.Extensions.Logging;

namespace Firefly.Signal.SharedKernel.Behaviors;

public sealed class TransactionBehavior<TRequest, TResponse>(
    IMediatorTransaction transaction,
    ILogger<TransactionBehavior<TRequest, TResponse>> logger) : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Running transactional request {RequestName}", typeof(TRequest).Name);

        return await transaction.ExecuteAsync<TResponse>(
            typeof(TRequest).Name,
            () => next(),
            cancellationToken);
    }
}
