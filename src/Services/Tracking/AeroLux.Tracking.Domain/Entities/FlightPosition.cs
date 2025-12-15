using AeroLux.Shared.Kernel.DDD;

namespace AeroLux.Tracking.Domain.Entities;

/// <summary>
/// Real-time flight position data
/// </summary>
public sealed class FlightPosition : AggregateRoot<Guid>
{
    public Guid FlightId { get; private set; }
    public double Latitude { get; private set; }
    public double Longitude { get; private set; }
    public int AltitudeFeet { get; private set; }
    public int GroundSpeedKnots { get; private set; }
    public int HeadingDegrees { get; private set; }
    public DateTime Timestamp { get; private set; }

    private FlightPosition() { }

    public static FlightPosition Create(
        Guid flightId,
        double latitude,
        double longitude,
        int altitudeFeet,
        int groundSpeedKnots,
        int headingDegrees)
    {
        return new FlightPosition
        {
            Id = Guid.NewGuid(),
            FlightId = flightId,
            Latitude = latitude,
            Longitude = longitude,
            AltitudeFeet = altitudeFeet,
            GroundSpeedKnots = groundSpeedKnots,
            HeadingDegrees = headingDegrees,
            Timestamp = DateTime.UtcNow
        };
    }

    public void Update(
        double latitude,
        double longitude,
        int altitudeFeet,
        int groundSpeedKnots,
        int headingDegrees)
    {
        Latitude = latitude;
        Longitude = longitude;
        AltitudeFeet = altitudeFeet;
        GroundSpeedKnots = groundSpeedKnots;
        HeadingDegrees = headingDegrees;
        Timestamp = DateTime.UtcNow;
    }
}
