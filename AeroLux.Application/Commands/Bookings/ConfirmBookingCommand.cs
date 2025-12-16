using MediatR;

namespace AeroLux.Application.Commands.Bookings;

/// <summary>
/// CQRS Command to confirm a booking
/// </summary>
public record ConfirmBookingCommand(Guid BookingId) : IRequest<bool>;
