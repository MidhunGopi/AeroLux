using AeroLux.Domain.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace AeroLux.Application.EventHandlers;

/// <summary>
/// Event handler for BookingCreatedEvent - demonstrates event-driven architecture
/// </summary>
public class BookingCreatedEventHandler : INotificationHandler<BookingCreatedEvent>
{
    private readonly ILogger<BookingCreatedEventHandler> _logger;

    public BookingCreatedEventHandler(ILogger<BookingCreatedEventHandler> logger)
    {
        _logger = logger;
    }

    public Task Handle(BookingCreatedEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Booking created: BookingId={BookingId}, CustomerId={CustomerId}, FlightId={FlightId}, Amount={Amount}",
            notification.BookingId,
            notification.CustomerId,
            notification.FlightId,
            notification.TotalAmount);

        // In a real system, this could trigger:
        // - Email notifications
        // - Payment processing
        // - External system integrations
        // - Analytics events

        return Task.CompletedTask;
    }
}
