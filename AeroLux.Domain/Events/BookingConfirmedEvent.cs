using AeroLux.Domain.Common;

namespace AeroLux.Domain.Events;

public record BookingConfirmedEvent(
    Guid BookingId,
    Guid CustomerId,
    Guid FlightId,
    DateTime ConfirmationTime) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}
