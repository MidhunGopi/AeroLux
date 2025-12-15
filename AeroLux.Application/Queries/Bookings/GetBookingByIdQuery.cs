using AeroLux.Application.DTOs;
using MediatR;

namespace AeroLux.Application.Queries.Bookings;

/// <summary>
/// CQRS Query to get booking by ID
/// </summary>
public record GetBookingByIdQuery(Guid BookingId) : IRequest<BookingDto?>;
