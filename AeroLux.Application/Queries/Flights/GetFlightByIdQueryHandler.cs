using AeroLux.Application.DTOs;
using AeroLux.Application.Interfaces;
using AeroLux.Domain.Entities;
using MediatR;

namespace AeroLux.Application.Queries.Flights;

public class GetFlightByIdQueryHandler : IRequestHandler<GetFlightByIdQuery, FlightDto?>
{
    private readonly IRepository<Flight> _flightRepository;

    public GetFlightByIdQueryHandler(IRepository<Flight> flightRepository)
    {
        _flightRepository = flightRepository;
    }

    public async Task<FlightDto?> Handle(GetFlightByIdQuery request, CancellationToken cancellationToken)
    {
        var flight = await _flightRepository.GetByIdAsync(request.FlightId, cancellationToken);

        if (flight == null)
            return null;

        return new FlightDto(
            flight.Id,
            flight.AircraftId,
            flight.DepartureAirport,
            flight.ArrivalAirport,
            flight.ScheduledDepartureTime,
            flight.ScheduledArrivalTime,
            flight.ActualDepartureTime,
            flight.ActualArrivalTime,
            flight.Status.ToString(),
            flight.FlightNumber
        );
    }
}
