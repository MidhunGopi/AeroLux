using AeroLux.Domain.Common;
using AeroLux.Domain.Enums;
using AeroLux.Domain.Events;

namespace AeroLux.Domain.Entities;

/// <summary>
/// Flight aggregate root representing scheduled flights
/// </summary>
public class Flight : Entity
{
    public Guid AircraftId { get; private set; }
    public string DepartureAirport { get; private set; }
    public string ArrivalAirport { get; private set; }
    public DateTime ScheduledDepartureTime { get; private set; }
    public DateTime ScheduledArrivalTime { get; private set; }
    public DateTime? ActualDepartureTime { get; private set; }
    public DateTime? ActualArrivalTime { get; private set; }
    public FlightStatus Status { get; private set; }
    public string? FlightNumber { get; private set; }

    private Flight() { } // EF Core

    public Flight(Guid aircraftId, string departureAirport, string arrivalAirport, 
        DateTime scheduledDepartureTime, DateTime scheduledArrivalTime, string? flightNumber = null)
    {
        AircraftId = aircraftId;
        DepartureAirport = departureAirport ?? throw new ArgumentNullException(nameof(departureAirport));
        ArrivalAirport = arrivalAirport ?? throw new ArgumentNullException(nameof(arrivalAirport));
        
        if (scheduledDepartureTime >= scheduledArrivalTime)
            throw new ArgumentException("Departure time must be before arrival time");
        
        ScheduledDepartureTime = scheduledDepartureTime;
        ScheduledArrivalTime = scheduledArrivalTime;
        Status = FlightStatus.Scheduled;
        FlightNumber = flightNumber;

        AddDomainEvent(new FlightScheduledEvent(Id, aircraftId, departureAirport, arrivalAirport, scheduledDepartureTime));
    }

    public void UpdateStatus(FlightStatus newStatus)
    {
        var oldStatus = Status;
        Status = newStatus;
        MarkAsUpdated();

        if (newStatus == FlightStatus.InFlight && ActualDepartureTime == null)
        {
            ActualDepartureTime = DateTime.UtcNow;
            AddDomainEvent(new FlightDepartedEvent(Id, ActualDepartureTime.Value));
        }
        else if (newStatus == FlightStatus.Landed && ActualArrivalTime == null)
        {
            ActualArrivalTime = DateTime.UtcNow;
            AddDomainEvent(new FlightCompletedEvent(Id, ActualArrivalTime.Value));
        }
        else if (newStatus == FlightStatus.Cancelled)
        {
            AddDomainEvent(new FlightCancelledEvent(Id, DateTime.UtcNow));
        }
    }

    public void Delay(TimeSpan delayDuration)
    {
        ScheduledDepartureTime = ScheduledDepartureTime.Add(delayDuration);
        ScheduledArrivalTime = ScheduledArrivalTime.Add(delayDuration);
        Status = FlightStatus.Delayed;
        MarkAsUpdated();
    }
}
