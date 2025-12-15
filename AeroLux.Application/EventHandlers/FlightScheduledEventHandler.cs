using AeroLux.Domain.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace AeroLux.Application.EventHandlers;

/// <summary>
/// Event handler for FlightScheduledEvent
/// </summary>
public class FlightScheduledEventHandler : INotificationHandler<FlightScheduledEvent>
{
    private readonly ILogger<FlightScheduledEventHandler> _logger;

    public FlightScheduledEventHandler(ILogger<FlightScheduledEventHandler> logger)
    {
        _logger = logger;
    }

    public Task Handle(FlightScheduledEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Flight scheduled: FlightId={FlightId}, Aircraft={AircraftId}, Route={Departure}->{Arrival}, Time={ScheduledTime}",
            notification.FlightId,
            notification.AircraftId,
            notification.DepartureAirport,
            notification.ArrivalAirport,
            notification.ScheduledDepartureTime);

        // In a real system, this could trigger:
        // - Crew scheduling
        // - Maintenance checks
        // - Fuel planning
        // - Airport slot booking

        return Task.CompletedTask;
    }
}
