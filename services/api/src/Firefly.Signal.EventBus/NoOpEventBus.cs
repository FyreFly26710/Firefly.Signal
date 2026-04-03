namespace Firefly.Signal.EventBus;

public sealed class NoOpEventBus : IEventBus
{
    public Task PublishAsync<TEvent>(TEvent @event, CancellationToken cancellationToken = default)
        where TEvent : IntegrationEvent
        => Task.CompletedTask;
}
