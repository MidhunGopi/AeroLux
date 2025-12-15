using AeroLux.Shared.Kernel.DDD;

namespace AeroLux.Fleet.Domain.Entities;

/// <summary>
/// Aircraft status enumeration
/// </summary>
public sealed class AircraftStatus : Enumeration<AircraftStatus>
{
    public static readonly AircraftStatus Available = new(1, nameof(Available));
    public static readonly AircraftStatus InFlight = new(2, nameof(InFlight));
    public static readonly AircraftStatus Maintenance = new(3, nameof(Maintenance));
    public static readonly AircraftStatus Grounded = new(4, nameof(Grounded));
    public static readonly AircraftStatus Reserved = new(5, nameof(Reserved));

    private AircraftStatus(int value, string name) : base(value, name) { }
}

/// <summary>
/// Aircraft aggregate root representing a private jet in the fleet
/// </summary>
public sealed class Aircraft : AggregateRoot<Guid>
{
    public string TailNumber { get; private set; } = string.Empty;
    public string Model { get; private set; } = string.Empty;
    public string Manufacturer { get; private set; } = string.Empty;
    public int MaxPassengers { get; private set; }
    public int RangeNauticalMiles { get; private set; }
    public AircraftStatus Status { get; private set; } = null!;
    public DateTime? LastMaintenanceDate { get; private set; }
    public DateTime? NextMaintenanceDate { get; private set; }
    public decimal HourlyRate { get; private set; }
    public bool IsAirworthy { get; private set; }

    private Aircraft() { }

    public static Aircraft Create(
        string tailNumber,
        string model,
        string manufacturer,
        int maxPassengers,
        int rangeNauticalMiles,
        decimal hourlyRate)
    {
        return new Aircraft
        {
            Id = Guid.NewGuid(),
            TailNumber = tailNumber.ToUpperInvariant(),
            Model = model,
            Manufacturer = manufacturer,
            MaxPassengers = maxPassengers,
            RangeNauticalMiles = rangeNauticalMiles,
            HourlyRate = hourlyRate,
            Status = AircraftStatus.Available,
            IsAirworthy = true
        };
    }

    public void UpdateMaintenanceSchedule(DateTime lastMaintenance, DateTime nextMaintenance)
    {
        LastMaintenanceDate = lastMaintenance;
        NextMaintenanceDate = nextMaintenance;
        CheckAirworthiness();
    }

    public void StartMaintenance()
    {
        Status = AircraftStatus.Maintenance;
        IsAirworthy = false;
    }

    public void CompleteMaintenance()
    {
        Status = AircraftStatus.Available;
        LastMaintenanceDate = DateTime.UtcNow;
        NextMaintenanceDate = DateTime.UtcNow.AddMonths(6);
        IsAirworthy = true;
    }

    public void Reserve()
    {
        if (Status != AircraftStatus.Available)
            throw new InvalidOperationException("Aircraft is not available for reservation.");

        Status = AircraftStatus.Reserved;
    }

    public void StartFlight()
    {
        if (Status != AircraftStatus.Reserved && Status != AircraftStatus.Available)
            throw new InvalidOperationException("Aircraft cannot start flight.");

        Status = AircraftStatus.InFlight;
    }

    public void EndFlight()
    {
        if (Status != AircraftStatus.InFlight)
            throw new InvalidOperationException("Aircraft is not in flight.");

        Status = AircraftStatus.Available;
        CheckAirworthiness();
    }

    public void Ground(string reason)
    {
        Status = AircraftStatus.Grounded;
        IsAirworthy = false;
    }

    private void CheckAirworthiness()
    {
        if (NextMaintenanceDate.HasValue && NextMaintenanceDate.Value <= DateTime.UtcNow)
        {
            IsAirworthy = false;
        }
    }
}
