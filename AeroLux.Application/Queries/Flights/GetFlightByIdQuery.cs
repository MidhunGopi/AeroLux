using AeroLux.Application.DTOs;
using MediatR;

namespace AeroLux.Application.Queries.Flights;

/// <summary>
/// CQRS Query to get flight by ID
/// </summary>
public record GetFlightByIdQuery(Guid FlightId) : IRequest<FlightDto?>;
