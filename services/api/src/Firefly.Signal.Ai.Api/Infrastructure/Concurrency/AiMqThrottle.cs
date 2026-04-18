namespace Firefly.Signal.Ai.Infrastructure.Concurrency;

/// <summary>
/// Singleton semaphore that limits concurrent MQ event processing to 5 at a time.
/// </summary>
public sealed class AiMqThrottle : IDisposable
{
    private readonly SemaphoreSlim _semaphore = new(initialCount: 5, maxCount: 5);

    public async Task<IDisposable> AcquireAsync(CancellationToken ct)
    {
        await _semaphore.WaitAsync(ct);
        return new Lease(_semaphore);
    }

    public void Dispose() => _semaphore.Dispose();

    private sealed class Lease(SemaphoreSlim semaphore) : IDisposable
    {
        public void Dispose() => semaphore.Release();
    }
}
