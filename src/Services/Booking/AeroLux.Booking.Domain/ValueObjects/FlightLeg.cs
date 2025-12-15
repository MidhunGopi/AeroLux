using AeroLux.Shared.Kernel.DDD;

namespace AeroLux.Booking.Domain.ValueObjects;

/// <summary>
/// Flight leg representing a single segment of a multi-leg flight
/// </summary>
public sealed class FlightLeg : ValueObject
{
    public int Sequence { get; }
    public AirportCode DepartureAirport { get; }
    public AirportCode ArrivalAirport { get; }
    public DateTime DepartureTime { get; }
    public DateTime ArrivalTime { get; }

    private FlightLeg(int sequence, AirportCode departureAirport, AirportCode arrivalAirport, 
        DateTime departureTime, DateTime arrivalTime)
    {
        Sequence = sequence;
        DepartureAirport = departureAirport;
        ArrivalAirport = arrivalAirport;
        DepartureTime = departureTime;
        ArrivalTime = arrivalTime;
    }

    public static FlightLeg Create(int sequence, string departureAirport, string arrivalAirport, 
        DateTime departureTime, DateTime arrivalTime)
    {
        if (sequence < 1)
            throw new ArgumentException("Sequence must be positive.", nameof(sequence));

        if (arrivalTime <= departureTime)
            throw new ArgumentException("Arrival time must be after departure time.", nameof(arrivalTime));

        return new FlightLeg(
            sequence,
            AirportCode.Create(departureAirport),
            AirportCode.Create(arrivalAirport),
            departureTime,
            arrivalTime);
    }

    public TimeSpan Duration => ArrivalTime - DepartureTime;

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Sequence;
        yield return DepartureAirport;
        yield return ArrivalAirport;
        yield return DepartureTime;
        yield return ArrivalTime;
    }
}
