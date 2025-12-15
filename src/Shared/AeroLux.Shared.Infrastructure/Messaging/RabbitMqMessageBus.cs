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
    private IConnection? _connection;
    private IChannel? _channel;
    private readonly string _exchangeName;
    private readonly string _connectionString;
    private readonly SemaphoreSlim _initLock = new(1, 1);
    private bool _initialized;

    private RabbitMqMessageBus(string connectionString, string exchangeName)
    {
        _connectionString = connectionString;
        _exchangeName = exchangeName;
    }

    /// <summary>
    /// Creates a new instance of RabbitMqMessageBus
    /// </summary>
    public static async Task<RabbitMqMessageBus> CreateAsync(
        string connectionString, 
        string exchangeName = "aerolux.events",
        CancellationToken cancellationToken = default)
    {
        var bus = new RabbitMqMessageBus(connectionString, exchangeName);
        await bus.InitializeAsync(cancellationToken);
        return bus;
    }

    /// <summary>
    /// Creates an uninitialized instance that will connect lazily on first use
    /// </summary>
    public static RabbitMqMessageBus CreateLazy(string connectionString, string exchangeName = "aerolux.events")
    {
        return new RabbitMqMessageBus(connectionString, exchangeName);
    }

    private async Task InitializeAsync(CancellationToken cancellationToken = default)
    {
        if (_initialized)
            return;

        await _initLock.WaitAsync(cancellationToken);
        try
        {
            if (_initialized)
                return;

            var factory = new ConnectionFactory
            {
                Uri = new Uri(_connectionString)
            };

            _connection = await factory.CreateConnectionAsync(cancellationToken);
            _channel = await _connection.CreateChannelAsync(cancellationToken: cancellationToken);
            await _channel.ExchangeDeclareAsync(_exchangeName, ExchangeType.Topic, durable: true, cancellationToken: cancellationToken);
            _initialized = true;
        }
        catch
        {
            // Connection may fail if RabbitMQ is not available - service should still start
        }
        finally
        {
            _initLock.Release();
        }
    }

    public async Task PublishAsync<TIntegrationEvent>(TIntegrationEvent integrationEvent, CancellationToken cancellationToken = default)
        where TIntegrationEvent : IIntegrationEvent
    {
        await PublishAsync((IIntegrationEvent)integrationEvent, cancellationToken);
    }

    public async Task PublishAsync(IIntegrationEvent integrationEvent, CancellationToken cancellationToken = default)
    {
        if (!_initialized)
            await InitializeAsync(cancellationToken);

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

        _initLock.Dispose();
    }
}
