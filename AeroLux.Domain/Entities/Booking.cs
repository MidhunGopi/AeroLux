using AeroLux.Domain.Common;
using AeroLux.Domain.Enums;
using AeroLux.Domain.Events;
using AeroLux.Domain.ValueObjects;

namespace AeroLux.Domain.Entities;

/// <summary>
/// Booking aggregate root representing charter bookings
/// </summary>
public class Booking : Entity
{
    public Guid CustomerId { get; private set; }
    public Guid FlightId { get; private set; }
    public BookingStatus Status { get; private set; }
    public Money TotalPrice { get; private set; }
    public int PassengerCount { get; private set; }
    public DateTime BookingDate { get; private set; }
    public string? SpecialRequests { get; private set; }
    public bool RequiresCatering { get; private set; }

    private Booking() { } // EF Core

    public Booking(Guid customerId, Guid flightId, Money totalPrice, int passengerCount, 
        string? specialRequests = null, bool requiresCatering = false)
    {
        if (passengerCount <= 0)
            throw new ArgumentException("Passenger count must be greater than zero", nameof(passengerCount));

        CustomerId = customerId;
        FlightId = flightId;
        TotalPrice = totalPrice ?? throw new ArgumentNullException(nameof(totalPrice));
        PassengerCount = passengerCount;
        Status = BookingStatus.Pending;
        BookingDate = DateTime.UtcNow;
        SpecialRequests = specialRequests;
        RequiresCatering = requiresCatering;

        AddDomainEvent(new BookingCreatedEvent(Id, customerId, flightId, totalPrice.Amount, BookingDate));
    }

    public void Confirm()
    {
        if (Status != BookingStatus.Pending)
            throw new InvalidOperationException("Only pending bookings can be confirmed");

        Status = BookingStatus.Confirmed;
        MarkAsUpdated();
        AddDomainEvent(new BookingConfirmedEvent(Id, CustomerId, FlightId, DateTime.UtcNow));
    }

    public void Cancel(string reason)
    {
        if (Status == BookingStatus.Completed || Status == BookingStatus.Cancelled)
            throw new InvalidOperationException($"Cannot cancel a {Status} booking");

        Status = BookingStatus.Cancelled;
        MarkAsUpdated();
        AddDomainEvent(new BookingCancelledEvent(Id, CustomerId, FlightId, reason, DateTime.UtcNow));
    }

    public void Complete()
    {
        if (Status != BookingStatus.Confirmed)
            throw new InvalidOperationException("Only confirmed bookings can be completed");

        Status = BookingStatus.Completed;
        MarkAsUpdated();
    }

    public void UpdateSpecialRequests(string specialRequests)
    {
        SpecialRequests = specialRequests;
        MarkAsUpdated();
    }
}
