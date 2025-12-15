using System.Text;
using System.Text.Json;
using AeroLux.Shared.Kernel.Messaging;
using RabbitMQ.Client;

namespace AeroLux.Shared.Infrastructure.Messaging;

/// <summary>
/// RabbitMQ implementation of the message bus
/// </summary>
public class RabbitMqMessageBus : IMessageBus, IAsyncDisposable
{
    private readonly IConnection? _connection;
    private readonly IChannel? _channel;
    private readonly string _exchangeName;

    public RabbitMqMessageBus(string connectionString, string exchangeName = "aerolux.events")
    {
        _exchangeName = exchangeName;

        var factory = new ConnectionFactory
        {
            Uri = new Uri(connectionString)
        };

        try
        {
            _connection = factory.CreateConnectionAsync().GetAwaiter().GetResult();
            _channel = _connection.CreateChannelAsync().GetAwaiter().GetResult();
            _channel.ExchangeDeclareAsync(_exchangeName, ExchangeType.Topic, durable: true).GetAwaiter().GetResult();
        }
        catch
        {
            // Connection may fail if RabbitMQ is not available - service should still start
        }
    }

    public async Task PublishAsync<TIntegrationEvent>(TIntegrationEvent integrationEvent, CancellationToken cancellationToken = default)
        where TIntegrationEvent : IIntegrationEvent
    {
        await PublishAsync((IIntegrationEvent)integrationEvent, cancellationToken);
    }

    public async Task PublishAsync(IIntegrationEvent integrationEvent, CancellationToken cancellationToken = default)
    {
        if (_channel is null)
            return;

        var routingKey = integrationEvent.EventType;
        var message = JsonSerializer.Serialize(integrationEvent, integrationEvent.GetType());
        var body = Encoding.UTF8.GetBytes(message);

        var properties = new BasicProperties
        {
            Persistent = true,
            MessageId = integrationEvent.EventId.ToString(),
            Timestamp = new AmqpTimestamp(DateTimeOffset.UtcNow.ToUnixTimeSeconds()),
            ContentType = "application/json"
        };

        await _channel.BasicPublishAsync(
            exchange: _exchangeName,
            routingKey: routingKey,
            mandatory: false,
            basicProperties: properties,
            body: body,
            cancellationToken: cancellationToken);
    }

    public async ValueTask DisposeAsync()
    {
        if (_channel is not null)
            await _channel.CloseAsync();

        if (_connection is not null)
            await _connection.CloseAsync();
    }
}
