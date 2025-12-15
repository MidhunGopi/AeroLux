namespace AeroLux.Shared.Kernel.Messaging;

/// <summary>
/// Base interface for integration events (cross-service events)
/// </summary>
public interface IIntegrationEvent
{
    Guid EventId { get; }
    DateTime OccurredOn { get; }
    string EventType { get; }
    int Version { get; }
}
