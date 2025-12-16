using AeroLux.Application.Interfaces;
using AeroLux.Domain.Entities;
using MediatR;

namespace AeroLux.Application.Commands.Flights;

public class ScheduleFlightCommandHandler : IRequestHandler<ScheduleFlightCommand, Guid>
{
    private readonly IRepository<Flight> _flightRepository;
    private readonly IUnitOfWork _unitOfWork;

    public ScheduleFlightCommandHandler(
        IRepository<Flight> flightRepository,
        IUnitOfWork unitOfWork)
    {
        _flightRepository = flightRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Guid> Handle(ScheduleFlightCommand request, CancellationToken cancellationToken)
    {
        var flight = new Flight(
            request.AircraftId,
            request.DepartureAirport,
            request.ArrivalAirport,
            request.ScheduledDepartureTime,
            request.ScheduledArrivalTime,
            request.FlightNumber
        );

        await _flightRepository.AddAsync(flight, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return flight.Id;
    }
}
