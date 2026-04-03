namespace Firefly.Signal.EventBusRabbitMQ;

public sealed class EventBusOptions
{
    public string SubscriptionClientName { get; set; } = string.Empty;
    public string ExchangeName { get; set; } = "firefly_signal_event_bus";
    public int RetryCount { get; set; } = 5;
}
