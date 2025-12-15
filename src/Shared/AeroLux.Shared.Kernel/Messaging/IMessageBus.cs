namespace AeroLux.Shared.Kernel.Messaging;

/// <summary>
/// Interface for the message bus (event publishing)
/// </summary>
public interface IMessageBus
{
    Task PublishAsync<TIntegrationEvent>(TIntegrationEvent integrationEvent, CancellationToken cancellationToken = default)
        where TIntegrationEvent : IIntegrationEvent;

    Task PublishAsync(IIntegrationEvent integrationEvent, CancellationToken cancellationToken = default);
}
