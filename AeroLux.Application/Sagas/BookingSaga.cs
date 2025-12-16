using Microsoft.Extensions.Logging;

namespace AeroLux.Application.Sagas;

/// <summary>
/// Saga orchestrator for booking workflow
/// Coordinates complex multi-step business processes with compensating transactions
/// </summary>
public class BookingSaga
{
    private readonly ILogger<BookingSaga> _logger;

    public BookingSaga(ILogger<BookingSaga> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Orchestrates the complete booking workflow
    /// Steps: Validate -> Reserve -> Process Payment -> Confirm
    /// Each step has a compensating action for rollback
    /// </summary>
    public async Task<BookingSagaResult> ExecuteAsync(BookingSagaRequest request, CancellationToken cancellationToken)
    {
        var state = new BookingSagaState { BookingId = request.BookingId };

        try
        {
            // Step 1: Validate booking
            _logger.LogInformation("Saga: Validating booking {BookingId}", request.BookingId);
            await ValidateBookingAsync(request, state, cancellationToken);

            // Step 2: Reserve aircraft
            _logger.LogInformation("Saga: Reserving aircraft for booking {BookingId}", request.BookingId);
            await ReserveAircraftAsync(request, state, cancellationToken);

            // Step 3: Process payment
            _logger.LogInformation("Saga: Processing payment for booking {BookingId}", request.BookingId);
            await ProcessPaymentAsync(request, state, cancellationToken);

            // Step 4: Confirm booking
            _logger.LogInformation("Saga: Confirming booking {BookingId}", request.BookingId);
            await ConfirmBookingAsync(request, state, cancellationToken);

            _logger.LogInformation("Saga: Successfully completed booking {BookingId}", request.BookingId);
            return new BookingSagaResult { Success = true, BookingId = request.BookingId };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Saga: Failed to process booking {BookingId}, executing compensating transactions", request.BookingId);
            await CompensateAsync(state, cancellationToken);
            return new BookingSagaResult { Success = false, Error = ex.Message };
        }
    }

    private async Task ValidateBookingAsync(BookingSagaRequest request, BookingSagaState state, CancellationToken cancellationToken)
    {
        // Validate business rules
        await Task.Delay(100, cancellationToken); // Simulate async operation
        state.IsValidated = true;
    }

    private async Task ReserveAircraftAsync(BookingSagaRequest request, BookingSagaState state, CancellationToken cancellationToken)
    {
        // Reserve aircraft for the flight
        await Task.Delay(100, cancellationToken); // Simulate async operation
        state.IsAircraftReserved = true;
    }

    private async Task ProcessPaymentAsync(BookingSagaRequest request, BookingSagaState state, CancellationToken cancellationToken)
    {
        // Process payment with payment gateway
        await Task.Delay(100, cancellationToken); // Simulate async operation
        state.IsPaymentProcessed = true;
    }

    private async Task ConfirmBookingAsync(BookingSagaRequest request, BookingSagaState state, CancellationToken cancellationToken)
    {
        // Confirm the booking
        await Task.Delay(100, cancellationToken); // Simulate async operation
        state.IsConfirmed = true;
    }

    private async Task CompensateAsync(BookingSagaState state, CancellationToken cancellationToken)
    {
        // Execute compensating transactions in reverse order
        if (state.IsPaymentProcessed)
        {
            _logger.LogInformation("Saga: Refunding payment for booking {BookingId}", state.BookingId);
            await Task.Delay(100, cancellationToken); // Refund payment
        }

        if (state.IsAircraftReserved)
        {
            _logger.LogInformation("Saga: Releasing aircraft reservation for booking {BookingId}", state.BookingId);
            await Task.Delay(100, cancellationToken); // Release reservation
        }

        _logger.LogInformation("Saga: Compensation completed for booking {BookingId}", state.BookingId);
    }
}

public record BookingSagaRequest(Guid BookingId, Guid CustomerId, Guid FlightId, decimal Amount);
public record BookingSagaResult { public bool Success { get; init; } public Guid? BookingId { get; init; } public string? Error { get; init; } }
public class BookingSagaState
{
    public Guid BookingId { get; set; }
    public bool IsValidated { get; set; }
    public bool IsAircraftReserved { get; set; }
    public bool IsPaymentProcessed { get; set; }
    public bool IsConfirmed { get; set; }
}
