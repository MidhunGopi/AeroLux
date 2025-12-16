namespace AeroLux.Application.DTOs;

public record FlightDto(
    Guid Id,
    Guid AircraftId,
    string DepartureAirport,
    string ArrivalAirport,
    DateTime ScheduledDepartureTime,
    DateTime ScheduledArrivalTime,
    DateTime? ActualDepartureTime,
    DateTime? ActualArrivalTime,
    string Status,
    string? FlightNumber
);
