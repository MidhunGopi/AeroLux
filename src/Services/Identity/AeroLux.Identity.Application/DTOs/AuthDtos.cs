namespace AeroLux.Identity.Application.DTOs;

public sealed record AuthTokens(string AccessToken, string RefreshToken, DateTime ExpiresAt);

public sealed record UserDto(
    Guid Id,
    string Email,
    string FirstName,
    string LastName,
    IReadOnlyList<string> Roles,
    bool IsActive,
    DateTime CreatedAt,
    DateTime? LastLoginAt);

public sealed record RegisterRequest(
    string Email,
    string Password,
    string FirstName,
    string LastName,
    IReadOnlyList<string>? Roles);

public sealed record LoginRequest(string Email, string Password);

public sealed record RefreshTokenRequest(string RefreshToken);
