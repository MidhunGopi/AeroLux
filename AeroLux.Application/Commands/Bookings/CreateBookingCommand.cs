using MediatR;

namespace AeroLux.Application.Commands.Bookings;

/// <summary>
/// CQRS Command to create a new booking
/// </summary>
public record CreateBookingCommand(
    Guid CustomerId,
    Guid FlightId,
    decimal Amount,
    string Currency,
    int PassengerCount,
    string? SpecialRequests = null,
    bool RequiresCatering = false
) : IRequest<Guid>;
