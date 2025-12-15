namespace AeroLux.Shared.Kernel.Results;

/// <summary>
/// Represents an error with a code and message
/// </summary>
public sealed record Error(string Code, string Message)
{
    public static readonly Error None = new(string.Empty, string.Empty);
    public static readonly Error NullValue = new("Error.NullValue", "The specified result value is null.");
}
