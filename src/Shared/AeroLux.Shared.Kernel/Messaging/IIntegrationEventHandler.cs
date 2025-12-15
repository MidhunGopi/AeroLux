namespace AeroLux.Shared.Kernel.Messaging;

/// <summary>
/// Interface for integration event handlers
/// </summary>
/// <typeparam name="TIntegrationEvent">The integration event type</typeparam>
public interface IIntegrationEventHandler<in TIntegrationEvent>
    where TIntegrationEvent : IIntegrationEvent
{
    Task HandleAsync(TIntegrationEvent integrationEvent, CancellationToken cancellationToken = default);
}
