using AeroLux.Shared.Kernel.DDD;

namespace AeroLux.Booking.Domain.Entities;

/// <summary>
/// Booking status enumeration
/// </summary>
public sealed class BookingStatus : Enumeration<BookingStatus>
{
    public static readonly BookingStatus Draft = new(1, nameof(Draft));
    public static readonly BookingStatus Pending = new(2, nameof(Pending));
    public static readonly BookingStatus Confirmed = new(3, nameof(Confirmed));
    public static readonly BookingStatus InProgress = new(4, nameof(InProgress));
    public static readonly BookingStatus Completed = new(5, nameof(Completed));
    public static readonly BookingStatus Cancelled = new(6, nameof(Cancelled));
    public static readonly BookingStatus Refunded = new(7, nameof(Refunded));

    private BookingStatus(int value, string name) : base(value, name) { }
}
