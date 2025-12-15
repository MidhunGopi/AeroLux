using AeroLux.Booking.Domain.Events;
using AeroLux.Booking.Domain.ValueObjects;
using AeroLux.Shared.Kernel.DDD;

namespace AeroLux.Booking.Domain.Entities;

/// <summary>
/// Booking aggregate root for private jet charter bookings
/// </summary>
public sealed class Booking : AggregateRoot<Guid>
{
    private readonly List<FlightLeg> _flightLegs = [];

    public string BookingNumber { get; private set; } = string.Empty;
    public Guid PassengerId { get; private set; }
    public Guid? AircraftId { get; private set; }
    public BookingStatus Status { get; private set; } = null!;
    public Money TotalPrice { get; private set; } = null!;
    public int PassengerCount { get; private set; }
    public string? SpecialRequests { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? ConfirmedAt { get; private set; }
    public DateTime? CancelledAt { get; private set; }
    public string? CancellationReason { get; private set; }

    public IReadOnlyList<FlightLeg> FlightLegs => _flightLegs.AsReadOnly();

    private Booking() { }

    public static Booking Create(
        Guid passengerId,
        int passengerCount,
        IEnumerable<FlightLeg> flightLegs,
        Money totalPrice,
        string? specialRequests = null)
    {
        var booking = new Booking
        {
            Id = Guid.NewGuid(),
            BookingNumber = GenerateBookingNumber(),
            PassengerId = passengerId,
            PassengerCount = passengerCount,
            TotalPrice = totalPrice,
            SpecialRequests = specialRequests,
            Status = BookingStatus.Draft,
            CreatedAt = DateTime.UtcNow
        };

        booking._flightLegs.AddRange(flightLegs.OrderBy(l => l.Sequence));
        booking.RaiseDomainEvent(new BookingCreatedEvent(
            booking.Id, 
            booking.BookingNumber, 
            passengerId));

        return booking;
    }

    public void SubmitForConfirmation()
    {
        if (Status != BookingStatus.Draft)
            throw new InvalidOperationException("Only draft bookings can be submitted for confirmation.");

        if (!_flightLegs.Any())
            throw new InvalidOperationException("Booking must have at least one flight leg.");

        Status = BookingStatus.Pending;
        RaiseDomainEvent(new BookingSubmittedEvent(Id, BookingNumber));
    }

    public void AssignAircraft(Guid aircraftId)
    {
        if (Status != BookingStatus.Pending && Status != BookingStatus.Confirmed)
            throw new InvalidOperationException("Cannot assign aircraft to this booking.");

        AircraftId = aircraftId;
    }

    public void Confirm()
    {
        if (Status != BookingStatus.Pending)
            throw new InvalidOperationException("Only pending bookings can be confirmed.");

        Status = BookingStatus.Confirmed;
        ConfirmedAt = DateTime.UtcNow;
        RaiseDomainEvent(new BookingConfirmedEvent(Id, BookingNumber, PassengerId));
    }

    public void StartFlight()
    {
        if (Status != BookingStatus.Confirmed)
            throw new InvalidOperationException("Only confirmed bookings can start.");

        Status = BookingStatus.InProgress;
    }

    public void Complete()
    {
        if (Status != BookingStatus.InProgress)
            throw new InvalidOperationException("Only in-progress bookings can be completed.");

        Status = BookingStatus.Completed;
        RaiseDomainEvent(new BookingCompletedEvent(Id, BookingNumber, PassengerId));
    }

    public void Cancel(string reason)
    {
        if (Status == BookingStatus.Completed || Status == BookingStatus.Cancelled)
            throw new InvalidOperationException("Cannot cancel this booking.");

        Status = BookingStatus.Cancelled;
        CancelledAt = DateTime.UtcNow;
        CancellationReason = reason;
        RaiseDomainEvent(new BookingCancelledEvent(Id, BookingNumber, PassengerId, reason));
    }

    public void UpdatePrice(Money newPrice)
    {
        if (Status != BookingStatus.Draft && Status != BookingStatus.Pending)
            throw new InvalidOperationException("Cannot update price for this booking.");

        TotalPrice = newPrice;
    }

    private static string GenerateBookingNumber()
    {
        return $"ALX-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString()[..8].ToUpperInvariant()}";
    }
}
