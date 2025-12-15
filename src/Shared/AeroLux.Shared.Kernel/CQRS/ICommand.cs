namespace AeroLux.Shared.Kernel.CQRS;

/// <summary>
/// Marker interface for commands (no return value)
/// </summary>
public interface ICommand
{
}

/// <summary>
/// Marker interface for commands with a return value
/// </summary>
/// <typeparam name="TResult">The result type</typeparam>
public interface ICommand<TResult>
{
}
