using MediatR;

namespace AeroLux.Domain.Common;

/// <summary>
/// Marker interface for domain events in event-driven architecture
/// </summary>
public interface IDomainEvent : INotification
{
    Guid EventId { get; }
    DateTime OccurredOn { get; }
}
