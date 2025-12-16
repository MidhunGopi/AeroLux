using AeroLux.Domain.Common;

namespace AeroLux.Domain.Events;

public record FlightCancelledEvent(
    Guid FlightId,
    DateTime CancellationTime) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}
