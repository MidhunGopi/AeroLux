using AeroLux.Application.DTOs;
using AeroLux.Application.Interfaces;
using AeroLux.Domain.Entities;
using MediatR;

namespace AeroLux.Application.Queries.Bookings;

public class GetBookingByIdQueryHandler : IRequestHandler<GetBookingByIdQuery, BookingDto?>
{
    private readonly IRepository<Booking> _bookingRepository;

    public GetBookingByIdQueryHandler(IRepository<Booking> bookingRepository)
    {
        _bookingRepository = bookingRepository;
    }

    public async Task<BookingDto?> Handle(GetBookingByIdQuery request, CancellationToken cancellationToken)
    {
        var booking = await _bookingRepository.GetByIdAsync(request.BookingId, cancellationToken);

        if (booking == null)
            return null;

        return new BookingDto(
            booking.Id,
            booking.CustomerId,
            booking.FlightId,
            booking.Status.ToString(),
            booking.TotalPrice.Amount,
            booking.TotalPrice.Currency,
            booking.PassengerCount,
            booking.BookingDate,
            booking.SpecialRequests,
            booking.RequiresCatering
        );
    }
}
