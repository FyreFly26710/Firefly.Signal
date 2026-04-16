namespace Firefly.Signal.SharedKernel.Behaviors;

public interface IMediatorTransaction
{
    Task<TResponse> ExecuteAsync<TResponse>(
        string requestName,
        Func<Task<TResponse>> handler,
        CancellationToken cancellationToken = default);
}
