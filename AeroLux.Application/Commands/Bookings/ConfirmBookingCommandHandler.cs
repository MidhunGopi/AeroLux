using AeroLux.Application.Interfaces;
using AeroLux.Domain.Entities;
using MediatR;

namespace AeroLux.Application.Commands.Bookings;

public class ConfirmBookingCommandHandler : IRequestHandler<ConfirmBookingCommand, bool>
{
    private readonly IRepository<Booking> _bookingRepository;
    private readonly IUnitOfWork _unitOfWork;

    public ConfirmBookingCommandHandler(
        IRepository<Booking> bookingRepository,
        IUnitOfWork unitOfWork)
    {
        _bookingRepository = bookingRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(ConfirmBookingCommand request, CancellationToken cancellationToken)
    {
        var booking = await _bookingRepository.GetByIdAsync(request.BookingId, cancellationToken);
        
        if (booking == null)
            return false;

        booking.Confirm();
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }
}
