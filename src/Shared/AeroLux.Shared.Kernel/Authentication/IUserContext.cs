namespace AeroLux.Shared.Kernel.Authentication;

/// <summary>
/// Represents a user context for authentication and authorization
/// </summary>
public interface IUserContext
{
    Guid? UserId { get; }
    string? Email { get; }
    IReadOnlyList<string> Roles { get; }
    IReadOnlyList<string> Permissions { get; }
    bool IsAuthenticated { get; }
}
