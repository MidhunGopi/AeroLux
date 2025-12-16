using AeroLux.Application.Interfaces;
using AeroLux.Domain.Entities;
using AeroLux.Domain.ValueObjects;
using MediatR;

namespace AeroLux.Application.Commands.Bookings;

public class CreateBookingCommandHandler : IRequestHandler<CreateBookingCommand, Guid>
{
    private readonly IRepository<Booking> _bookingRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateBookingCommandHandler(
        IRepository<Booking> bookingRepository,
        IUnitOfWork unitOfWork)
    {
        _bookingRepository = bookingRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Guid> Handle(CreateBookingCommand request, CancellationToken cancellationToken)
    {
        var money = new Money(request.Amount, request.Currency);
        
        var booking = new Booking(
            request.CustomerId,
            request.FlightId,
            money,
            request.PassengerCount,
            request.SpecialRequests,
            request.RequiresCatering
        );

        await _bookingRepository.AddAsync(booking, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return booking.Id;
    }
}
