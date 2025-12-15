using AeroLux.Shared.Kernel.DDD;

namespace AeroLux.FlightOperations.Domain.Entities;

/// <summary>
/// Flight status enumeration (state machine states)
/// </summary>
public sealed class FlightStatus : Enumeration<FlightStatus>
{
    public static readonly FlightStatus Scheduled = new(1, nameof(Scheduled));
    public static readonly FlightStatus CrewAssigned = new(2, nameof(CrewAssigned));
    public static readonly FlightStatus AircraftAssigned = new(3, nameof(AircraftAssigned));
    public static readonly FlightStatus ClearanceRequested = new(4, nameof(ClearanceRequested));
    public static readonly FlightStatus ClearanceApproved = new(5, nameof(ClearanceApproved));
    public static readonly FlightStatus ReadyForDeparture = new(6, nameof(ReadyForDeparture));
    public static readonly FlightStatus Departed = new(7, nameof(Departed));
    public static readonly FlightStatus Airborne = new(8, nameof(Airborne));
    public static readonly FlightStatus Landed = new(9, nameof(Landed));
    public static readonly FlightStatus Completed = new(10, nameof(Completed));
    public static readonly FlightStatus Cancelled = new(11, nameof(Cancelled));
    public static readonly FlightStatus Delayed = new(12, nameof(Delayed));

    private FlightStatus(int value, string name) : base(value, name) { }
}

/// <summary>
/// Flight aggregate root - orchestrates the full flight lifecycle using saga pattern
/// </summary>
public sealed class Flight : AggregateRoot<Guid>
{
    private readonly List<Guid> _crewMemberIds = [];

    public Guid BookingId { get; private set; }
    public string FlightNumber { get; private set; } = string.Empty;
    public Guid? AircraftId { get; private set; }
    public FlightStatus Status { get; private set; } = null!;
    public string DepartureAirport { get; private set; } = string.Empty;
    public string ArrivalAirport { get; private set; } = string.Empty;
    public DateTime ScheduledDepartureTime { get; private set; }
    public DateTime ScheduledArrivalTime { get; private set; }
    public DateTime? ActualDepartureTime { get; private set; }
    public DateTime? ActualArrivalTime { get; private set; }
    public Guid? CaptainId { get; private set; }
    public Guid? ClearanceId { get; private set; }
    public string? CancellationReason { get; private set; }

    public IReadOnlyList<Guid> CrewMemberIds => _crewMemberIds.AsReadOnly();

    private Flight() { }

    public static Flight Create(
        Guid bookingId,
        string departureAirport,
        string arrivalAirport,
        DateTime scheduledDepartureTime,
        DateTime scheduledArrivalTime)
    {
        return new Flight
        {
            Id = Guid.NewGuid(),
            BookingId = bookingId,
            FlightNumber = GenerateFlightNumber(),
            Status = FlightStatus.Scheduled,
            DepartureAirport = departureAirport.ToUpperInvariant(),
            ArrivalAirport = arrivalAirport.ToUpperInvariant(),
            ScheduledDepartureTime = scheduledDepartureTime,
            ScheduledArrivalTime = scheduledArrivalTime
        };
    }

    public void AssignAircraft(Guid aircraftId)
    {
        if (Status != FlightStatus.Scheduled && Status != FlightStatus.CrewAssigned)
            throw new InvalidOperationException("Cannot assign aircraft in current state.");

        AircraftId = aircraftId;
        if (_crewMemberIds.Count > 0 && CaptainId.HasValue)
            Status = FlightStatus.AircraftAssigned;
    }

    public void AssignCrew(Guid captainId, IEnumerable<Guid> crewMemberIds)
    {
        if (Status != FlightStatus.Scheduled && Status != FlightStatus.AircraftAssigned)
            throw new InvalidOperationException("Cannot assign crew in current state.");

        CaptainId = captainId;
        _crewMemberIds.Clear();
        _crewMemberIds.AddRange(crewMemberIds);

        if (AircraftId.HasValue)
            Status = FlightStatus.AircraftAssigned;
        else
            Status = FlightStatus.CrewAssigned;
    }

    public void RequestClearance()
    {
        if (Status != FlightStatus.AircraftAssigned)
            throw new InvalidOperationException("Cannot request clearance in current state.");

        Status = FlightStatus.ClearanceRequested;
    }

    public void ApproveClearance(Guid clearanceId)
    {
        if (Status != FlightStatus.ClearanceRequested)
            throw new InvalidOperationException("Cannot approve clearance in current state.");

        ClearanceId = clearanceId;
        Status = FlightStatus.ClearanceApproved;
    }

    public void MarkReadyForDeparture()
    {
        if (Status != FlightStatus.ClearanceApproved)
            throw new InvalidOperationException("Cannot mark ready for departure in current state.");

        Status = FlightStatus.ReadyForDeparture;
    }

    public void Depart()
    {
        if (Status != FlightStatus.ReadyForDeparture)
            throw new InvalidOperationException("Cannot depart in current state.");

        ActualDepartureTime = DateTime.UtcNow;
        Status = FlightStatus.Departed;
    }

    public void MarkAirborne()
    {
        if (Status != FlightStatus.Departed)
            throw new InvalidOperationException("Cannot mark airborne in current state.");

        Status = FlightStatus.Airborne;
    }

    public void Land()
    {
        if (Status != FlightStatus.Airborne)
            throw new InvalidOperationException("Cannot land in current state.");

        ActualArrivalTime = DateTime.UtcNow;
        Status = FlightStatus.Landed;
    }

    public void Complete()
    {
        if (Status != FlightStatus.Landed)
            throw new InvalidOperationException("Cannot complete in current state.");

        Status = FlightStatus.Completed;
    }

    public void Cancel(string reason)
    {
        if (Status == FlightStatus.Completed || Status == FlightStatus.Cancelled)
            throw new InvalidOperationException("Cannot cancel in current state.");

        CancellationReason = reason;
        Status = FlightStatus.Cancelled;
    }

    public void Delay(DateTime newDepartureTime, DateTime newArrivalTime)
    {
        if (Status == FlightStatus.Completed || Status == FlightStatus.Cancelled || 
            Status == FlightStatus.Airborne || Status == FlightStatus.Landed)
            throw new InvalidOperationException("Cannot delay in current state.");

        ScheduledDepartureTime = newDepartureTime;
        ScheduledArrivalTime = newArrivalTime;
        Status = FlightStatus.Delayed;
    }

    private static string GenerateFlightNumber()
    {
        return $"ALX{DateTime.UtcNow:yyyyMMdd}{Random.Shared.Next(1000, 9999)}";
    }
}
