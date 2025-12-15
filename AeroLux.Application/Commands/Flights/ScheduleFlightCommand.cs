using MediatR;

namespace AeroLux.Application.Commands.Flights;

/// <summary>
/// CQRS Command to schedule a new flight
/// </summary>
public record ScheduleFlightCommand(
    Guid AircraftId,
    string DepartureAirport,
    string ArrivalAirport,
    DateTime ScheduledDepartureTime,
    DateTime ScheduledArrivalTime,
    string? FlightNumber = null
) : IRequest<Guid>;
