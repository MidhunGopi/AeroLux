using AeroLux.Shared.Kernel.DDD;

namespace AeroLux.Booking.Domain.ValueObjects;

/// <summary>
/// Airport code value object (IATA format)
/// </summary>
public sealed class AirportCode : ValueObject
{
    public string Value { get; }

    private AirportCode(string value)
    {
        Value = value;
    }

    public static AirportCode Create(string code)
    {
        if (string.IsNullOrWhiteSpace(code))
            throw new ArgumentException("Airport code cannot be empty.", nameof(code));

        var normalized = code.ToUpperInvariant().Trim();
        if (normalized.Length != 3)
            throw new ArgumentException("Airport code must be exactly 3 characters.", nameof(code));

        return new AirportCode(normalized);
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }

    public override string ToString() => Value;
    public static implicit operator string(AirportCode code) => code.Value;
}
