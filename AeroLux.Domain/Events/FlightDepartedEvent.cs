using AeroLux.Domain.Common;

namespace AeroLux.Domain.Events;

public record FlightDepartedEvent(
    Guid FlightId,
    DateTime ActualDepartureTime) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}
