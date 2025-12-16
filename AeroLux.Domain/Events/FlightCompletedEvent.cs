using AeroLux.Domain.Common;

namespace AeroLux.Domain.Events;

public record FlightCompletedEvent(
    Guid FlightId,
    DateTime ActualArrivalTime) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}
