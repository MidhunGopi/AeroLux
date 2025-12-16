using AeroLux.Application.Commands.Flights;
using AeroLux.Application.Queries.Flights;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace AeroLux.API.Controllers;

/// <summary>
/// API Controller for flight operations
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class FlightsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<FlightsController> _logger;

    public FlightsController(IMediator mediator, ILogger<FlightsController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Schedule a new flight
    /// </summary>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<Guid>> ScheduleFlight([FromBody] ScheduleFlightCommand command)
    {
        try
        {
            var flightId = await _mediator.Send(command);
            _logger.LogInformation("Scheduled flight: {FlightId}", flightId);
            return CreatedAtAction(nameof(GetFlight), new { id = flightId }, flightId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error scheduling flight");
            return BadRequest(new { error = ex.Message });
        }
    }

    /// <summary>
    /// Get flight by ID
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> GetFlight(Guid id)
    {
        var query = new GetFlightByIdQuery(id);
        var flight = await _mediator.Send(query);

        if (flight == null)
        {
            return NotFound(new { message = $"Flight {id} not found" });
        }

        return Ok(flight);
    }
}
