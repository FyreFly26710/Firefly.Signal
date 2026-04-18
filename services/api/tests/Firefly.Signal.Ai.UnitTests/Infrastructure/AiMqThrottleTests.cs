using Firefly.Signal.Ai.Infrastructure.Concurrency;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Firefly.Signal.Ai.UnitTests.Infrastructure;

[TestClass]
public sealed class AiMqThrottleTests
{
    [TestMethod]
    public async Task AcquireAsync_WithMoreThan5Concurrent_LimitsToFiveAtATime()
    {
        using var throttle = new AiMqThrottle();
        var currentConcurrency = 0;
        var maxObservedConcurrency = 0;
        var lockObj = new object();

        async Task SimulateWorkAsync()
        {
            using var lease = await throttle.AcquireAsync(CancellationToken.None);

            var current = Interlocked.Increment(ref currentConcurrency);
            lock (lockObj)
            {
                if (current > maxObservedConcurrency)
                    maxObservedConcurrency = current;
            }

            await Task.Delay(20);
            Interlocked.Decrement(ref currentConcurrency);
        }

        var tasks = Enumerable.Range(0, 12).Select(_ => SimulateWorkAsync());
        await Task.WhenAll(tasks);

        Assert.IsLessThanOrEqualTo(maxObservedConcurrency, 5,
            $"Expected max concurrency <= 5 but observed {maxObservedConcurrency}");
    }

    [TestMethod]
    public async Task AcquireAsync_After5Complete_AllowsRemainingToComplete()
    {
        using var throttle = new AiMqThrottle();
        var completed = 0;

        async Task WorkAsync()
        {
            using var lease = await throttle.AcquireAsync(CancellationToken.None);
            await Task.Delay(5);
            Interlocked.Increment(ref completed);
        }

        await Task.WhenAll(Enumerable.Range(0, 10).Select(_ => WorkAsync()));

        Assert.AreEqual(10, completed);
    }

    [TestMethod]
    public async Task AcquireAsync_WhenCancelled_ThrowsOperationCanceledException()
    {
        using var throttle = new AiMqThrottle();

        // Exhaust all 5 slots
        var leases = new List<IDisposable>();
        for (var i = 0; i < 5; i++)
            leases.Add(await throttle.AcquireAsync(CancellationToken.None));

        using var cts = new CancellationTokenSource(millisecondsDelay: 50);

        await Assert.ThrowsExactlyAsync<OperationCanceledException>(
            () => throttle.AcquireAsync(cts.Token));

        foreach (var lease in leases)
            lease.Dispose();
    }
}
