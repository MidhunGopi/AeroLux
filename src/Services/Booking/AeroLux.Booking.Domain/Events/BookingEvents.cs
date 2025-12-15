using AeroLux.Shared.Kernel.Events;

namespace AeroLux.Booking.Domain.Events;

public sealed class BookingCreatedEvent : DomainEvent
{
    public Guid BookingId { get; }
    public string BookingNumber { get; }
    public Guid PassengerId { get; }

    public BookingCreatedEvent(Guid bookingId, string bookingNumber, Guid passengerId)
    {
        BookingId = bookingId;
        BookingNumber = bookingNumber;
        PassengerId = passengerId;
    }
}

public sealed class BookingSubmittedEvent : DomainEvent
{
    public Guid BookingId { get; }
    public string BookingNumber { get; }

    public BookingSubmittedEvent(Guid bookingId, string bookingNumber)
    {
        BookingId = bookingId;
        BookingNumber = bookingNumber;
    }
}

public sealed class BookingConfirmedEvent : DomainEvent
{
    public Guid BookingId { get; }
    public string BookingNumber { get; }
    public Guid PassengerId { get; }

    public BookingConfirmedEvent(Guid bookingId, string bookingNumber, Guid passengerId)
    {
        BookingId = bookingId;
        BookingNumber = bookingNumber;
        PassengerId = passengerId;
    }
}

public sealed class BookingCompletedEvent : DomainEvent
{
    public Guid BookingId { get; }
    public string BookingNumber { get; }
    public Guid PassengerId { get; }

    public BookingCompletedEvent(Guid bookingId, string bookingNumber, Guid passengerId)
    {
        BookingId = bookingId;
        BookingNumber = bookingNumber;
        PassengerId = passengerId;
    }
}

public sealed class BookingCancelledEvent : DomainEvent
{
    public Guid BookingId { get; }
    public string BookingNumber { get; }
    public Guid PassengerId { get; }
    public string Reason { get; }

    public BookingCancelledEvent(Guid bookingId, string bookingNumber, Guid passengerId, string reason)
    {
        BookingId = bookingId;
        BookingNumber = bookingNumber;
        PassengerId = passengerId;
        Reason = reason;
    }
}
