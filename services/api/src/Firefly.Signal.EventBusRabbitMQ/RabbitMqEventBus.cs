using System.Text;
using System.Text.Json;
using Firefly.Signal.EventBus;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Firefly.Signal.EventBusRabbitMQ;

internal sealed class RabbitMqEventBus(
    ILogger<RabbitMqEventBus> logger,
    IServiceProvider serviceProvider,
    IOptions<RabbitMqEventBusOptions> rabbitMqOptions,
    IOptions<EventBusOptions> eventBusOptions,
    IOptions<EventBusSubscriptionInfo> subscriptionOptions) : IEventBus, IHostedService, IDisposable
{
    private readonly RabbitMqEventBusOptions _rabbitMqOptions = rabbitMqOptions.Value;
    private readonly EventBusOptions _eventBusOptions = eventBusOptions.Value;
    private readonly EventBusSubscriptionInfo _subscriptionInfo = subscriptionOptions.Value;

    private IConnection? _connection;
    private IChannel? _consumerChannel;
    private CancellationToken _consumerStoppingToken = CancellationToken.None;

    public async Task PublishAsync<TEvent>(TEvent @event, CancellationToken cancellationToken = default)
        where TEvent : IntegrationEvent
    {
        await EnsureConnectionAsync(cancellationToken);

        if (_connection is null)
        {
            throw new InvalidOperationException("RabbitMQ connection is not available.");
        }

        await using var channel = await _connection.CreateChannelAsync(cancellationToken: cancellationToken);
        await channel.ExchangeDeclareAsync(
            exchange: _eventBusOptions.ExchangeName,
            type: ExchangeType.Direct,
            durable: true,
            autoDelete: false,
            cancellationToken: cancellationToken);

        var properties = new BasicProperties
        {
            DeliveryMode = DeliveryModes.Persistent
        };

        var body = JsonSerializer.SerializeToUtf8Bytes(@event, @event.GetType(), _subscriptionInfo.JsonSerializerOptions);
        await channel.BasicPublishAsync(
            exchange: _eventBusOptions.ExchangeName,
            routingKey: typeof(TEvent).Name,
            mandatory: false,
            basicProperties: properties,
            body: body,
            cancellationToken: cancellationToken);

        logger.LogInformation(
            "Published integration event {EventName} with id {EventId}",
            typeof(TEvent).Name,
            @event.Id);
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        _consumerStoppingToken = cancellationToken;
        await EnsureConnectionAsync(cancellationToken);

        if (_connection is null || !_subscriptionInfo.EventTypes.Any())
        {
            return;
        }

        _consumerChannel = await _connection.CreateChannelAsync(cancellationToken: cancellationToken);
        await _consumerChannel.ExchangeDeclareAsync(
            exchange: _eventBusOptions.ExchangeName,
            type: ExchangeType.Direct,
            durable: true,
            autoDelete: false,
            cancellationToken: cancellationToken);

        await _consumerChannel.QueueDeclareAsync(
            queue: _eventBusOptions.SubscriptionClientName,
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: null,
            cancellationToken: cancellationToken);

        foreach (var eventName in _subscriptionInfo.EventTypes.Keys)
        {
            await _consumerChannel.QueueBindAsync(
                queue: _eventBusOptions.SubscriptionClientName,
                exchange: _eventBusOptions.ExchangeName,
                routingKey: eventName,
                cancellationToken: cancellationToken);
        }

        var consumer = new AsyncEventingBasicConsumer(_consumerChannel);
        consumer.ReceivedAsync += OnMessageReceivedAsync;

        await _consumerChannel.BasicConsumeAsync(
            queue: _eventBusOptions.SubscriptionClientName,
            autoAck: false,
            consumer: consumer,
            cancellationToken: cancellationToken);

        logger.LogInformation(
            "RabbitMQ event bus consumer started for queue {QueueName}",
            _eventBusOptions.SubscriptionClientName);
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        if (_consumerChannel is not null)
        {
            await _consumerChannel.CloseAsync(cancellationToken);
            await _consumerChannel.DisposeAsync();
            _consumerChannel = null;
        }

        if (_connection is not null)
        {
            await _connection.CloseAsync(cancellationToken);
            await _connection.DisposeAsync();
            _connection = null;
        }
    }

    private async Task EnsureConnectionAsync(CancellationToken cancellationToken)
    {
        if (_connection is { IsOpen: true })
        {
            return;
        }

        var factory = new ConnectionFactory
        {
            HostName = _rabbitMqOptions.Host,
            Port = _rabbitMqOptions.Port,
            UserName = _rabbitMqOptions.Username,
            Password = _rabbitMqOptions.Password,
            VirtualHost = _rabbitMqOptions.VirtualHost,
            AutomaticRecoveryEnabled = true,
            TopologyRecoveryEnabled = true
        };

        _connection = await factory.CreateConnectionAsync(cancellationToken);
        logger.LogInformation(
            "RabbitMQ connection opened for host {Host}:{Port}",
            _rabbitMqOptions.Host,
            _rabbitMqOptions.Port);
    }

    private async Task OnMessageReceivedAsync(object sender, BasicDeliverEventArgs eventArgs)
    {
        if (_consumerChannel is null)
        {
            return;
        }

        var eventName = eventArgs.RoutingKey;
        var message = Encoding.UTF8.GetString(eventArgs.Body.ToArray());

        try
        {
            await ProcessEventAsync(eventName, message, _consumerStoppingToken);
            await _consumerChannel.BasicAckAsync(eventArgs.DeliveryTag, multiple: false);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error processing RabbitMQ event {EventName}", eventName);
            await _consumerChannel.BasicNackAsync(eventArgs.DeliveryTag, multiple: false, requeue: true);
        }
    }

    private async Task ProcessEventAsync(string eventName, string message, CancellationToken cancellationToken)
    {
        if (!_subscriptionInfo.EventTypes.TryGetValue(eventName, out var eventType))
        {
            logger.LogWarning("No event type registered for RabbitMQ event {EventName}", eventName);
            return;
        }

        var integrationEvent = JsonSerializer.Deserialize(message, eventType, _subscriptionInfo.JsonSerializerOptions) as IntegrationEvent;
        if (integrationEvent is null)
        {
            logger.LogWarning("Failed to deserialize RabbitMQ event {EventName}", eventName);
            return;
        }

        await using var scope = serviceProvider.CreateAsyncScope();
        var handlers = scope.ServiceProvider.GetKeyedServices<IIntegrationEventHandler>(eventType).ToArray();

        if (handlers.Length == 0)
        {
            logger.LogWarning("No handlers registered for RabbitMQ event {EventName}", eventName);
            return;
        }

        foreach (var handler in handlers)
        {
            await handler.HandleAsync(integrationEvent, cancellationToken);
        }
    }

    public void Dispose()
    {
        _consumerChannel?.Dispose();
        _connection?.Dispose();
    }
}
