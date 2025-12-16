using AeroLux.Domain.Common;

namespace AeroLux.Domain.Events;

public record FlightScheduledEvent(
    Guid FlightId,
    Guid AircraftId,
    string DepartureAirport,
    string ArrivalAirport,
    DateTime ScheduledDepartureTime) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}
