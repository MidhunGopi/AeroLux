using AeroLux.Domain.Common;

namespace AeroLux.Domain.Events;

public record BookingCancelledEvent(
    Guid BookingId,
    Guid CustomerId,
    Guid FlightId,
    string Reason,
    DateTime CancellationTime) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}
