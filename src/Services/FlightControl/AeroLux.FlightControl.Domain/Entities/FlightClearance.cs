using AeroLux.Shared.Kernel.DDD;

namespace AeroLux.FlightControl.Domain.Entities;

/// <summary>
/// Clearance status enumeration
/// </summary>
public sealed class ClearanceStatus : Enumeration<ClearanceStatus>
{
    public static readonly ClearanceStatus Pending = new(1, nameof(Pending));
    public static readonly ClearanceStatus Approved = new(2, nameof(Approved));
    public static readonly ClearanceStatus Denied = new(3, nameof(Denied));
    public static readonly ClearanceStatus Expired = new(4, nameof(Expired));

    private ClearanceStatus(int value, string name) : base(value, name) { }
}

/// <summary>
/// Weather condition assessment
/// </summary>
public sealed class WeatherCondition : Enumeration<WeatherCondition>
{
    public static readonly WeatherCondition VFR = new(1, "Visual Flight Rules");
    public static readonly WeatherCondition MVFR = new(2, "Marginal VFR");
    public static readonly WeatherCondition IFR = new(3, "Instrument Flight Rules");
    public static readonly WeatherCondition LIFR = new(4, "Low IFR");

    private WeatherCondition(int value, string name) : base(value, name) { }
}

/// <summary>
/// Flight clearance aggregate root - regulatory and weather clearance for flights
/// </summary>
public sealed class FlightClearance : AggregateRoot<Guid>
{
    public Guid FlightId { get; private set; }
    public ClearanceStatus Status { get; private set; } = null!;
    public WeatherCondition DepartureWeather { get; private set; } = null!;
    public WeatherCondition ArrivalWeather { get; private set; } = null!;
    public bool AirspaceApproved { get; private set; }
    public bool RegulatoryApproved { get; private set; }
    public Guid? ApprovedByOfficerId { get; private set; }
    public string? DenialReason { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? ApprovedAt { get; private set; }
    public DateTime? ExpiresAt { get; private set; }

    public bool IsGoForFlight => Status == ClearanceStatus.Approved && 
                                  DepartureWeather != WeatherCondition.LIFR && 
                                  ArrivalWeather != WeatherCondition.LIFR;

    private FlightClearance() { }

    public static FlightClearance Create(Guid flightId)
    {
        return new FlightClearance
        {
            Id = Guid.NewGuid(),
            FlightId = flightId,
            Status = ClearanceStatus.Pending,
            DepartureWeather = WeatherCondition.VFR,
            ArrivalWeather = WeatherCondition.VFR,
            AirspaceApproved = false,
            RegulatoryApproved = false,
            CreatedAt = DateTime.UtcNow
        };
    }

    public void UpdateWeatherConditions(WeatherCondition departure, WeatherCondition arrival)
    {
        DepartureWeather = departure;
        ArrivalWeather = arrival;
    }

    public void ApproveAirspace()
    {
        AirspaceApproved = true;
        CheckForAutoApproval();
    }

    public void ApproveRegulatory()
    {
        RegulatoryApproved = true;
        CheckForAutoApproval();
    }

    public void Approve(Guid officerId)
    {
        if (Status != ClearanceStatus.Pending)
            throw new InvalidOperationException("Can only approve pending clearances.");

        if (DepartureWeather == WeatherCondition.LIFR || ArrivalWeather == WeatherCondition.LIFR)
            throw new InvalidOperationException("Cannot approve with LIFR conditions.");

        ApprovedByOfficerId = officerId;
        ApprovedAt = DateTime.UtcNow;
        ExpiresAt = DateTime.UtcNow.AddHours(4);
        Status = ClearanceStatus.Approved;
    }

    public void Deny(string reason)
    {
        if (Status != ClearanceStatus.Pending)
            throw new InvalidOperationException("Can only deny pending clearances.");

        DenialReason = reason;
        Status = ClearanceStatus.Denied;
    }

    public void CheckExpiration()
    {
        if (Status == ClearanceStatus.Approved && ExpiresAt.HasValue && ExpiresAt.Value < DateTime.UtcNow)
        {
            Status = ClearanceStatus.Expired;
        }
    }

    private void CheckForAutoApproval()
    {
        if (AirspaceApproved && RegulatoryApproved && 
            DepartureWeather != WeatherCondition.LIFR && 
            ArrivalWeather != WeatherCondition.LIFR)
        {
            // Auto-approval logic could be triggered here
        }
    }
}
