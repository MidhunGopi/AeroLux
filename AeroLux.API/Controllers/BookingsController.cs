using AeroLux.Application.Commands.Bookings;
using AeroLux.Application.Queries.Bookings;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace AeroLux.API.Controllers;

/// <summary>
/// API Controller for booking operations
/// Demonstrates CQRS pattern with commands and queries
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class BookingsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<BookingsController> _logger;

    public BookingsController(IMediator mediator, ILogger<BookingsController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Create a new booking
    /// </summary>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<Guid>> CreateBooking([FromBody] CreateBookingCommand command)
    {
        try
        {
            var bookingId = await _mediator.Send(command);
            _logger.LogInformation("Created booking: {BookingId}", bookingId);
            return CreatedAtAction(nameof(GetBooking), new { id = bookingId }, bookingId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating booking");
            return BadRequest(new { error = ex.Message });
        }
    }

    /// <summary>
    /// Get booking by ID
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> GetBooking(Guid id)
    {
        var query = new GetBookingByIdQuery(id);
        var booking = await _mediator.Send(query);

        if (booking == null)
        {
            return NotFound(new { message = $"Booking {id} not found" });
        }

        return Ok(booking);
    }

    /// <summary>
    /// Confirm a booking
    /// </summary>
    [HttpPost("{id}/confirm")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> ConfirmBooking(Guid id)
    {
        try
        {
            var command = new ConfirmBookingCommand(id);
            var result = await _mediator.Send(command);

            if (!result)
            {
                return NotFound(new { message = $"Booking {id} not found" });
            }

            _logger.LogInformation("Confirmed booking: {BookingId}", id);
            return Ok(new { message = "Booking confirmed successfully", bookingId = id });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error confirming booking {BookingId}", id);
            return BadRequest(new { error = ex.Message });
        }
    }
}
