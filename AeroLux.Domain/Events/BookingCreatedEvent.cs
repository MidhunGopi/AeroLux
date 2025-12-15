using AeroLux.Domain.Common;

namespace AeroLux.Domain.Events;

public record BookingCreatedEvent(
    Guid BookingId,
    Guid CustomerId,
    Guid FlightId,
    decimal TotalAmount,
    DateTime BookingDate) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}
