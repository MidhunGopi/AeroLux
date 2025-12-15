namespace AeroLux.Application.DTOs;

public record BookingDto(
    Guid Id,
    Guid CustomerId,
    Guid FlightId,
    string Status,
    decimal TotalAmount,
    string Currency,
    int PassengerCount,
    DateTime BookingDate,
    string? SpecialRequests,
    bool RequiresCatering
);
