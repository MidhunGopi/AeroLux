using AeroLux.Identity.Application.DTOs;
using AeroLux.Shared.Kernel.CQRS;
using AeroLux.Shared.Kernel.Results;

namespace AeroLux.Identity.Application.Commands;

public sealed record RegisterUserCommand(
    string Email,
    string Password,
    string FirstName,
    string LastName,
    IReadOnlyList<string>? Roles) : ICommand<Result<UserDto>>;

public sealed record LoginCommand(string Email, string Password) : ICommand<Result<AuthTokens>>;

public sealed record RefreshTokenCommand(string RefreshToken) : ICommand<Result<AuthTokens>>;

public sealed record LogoutCommand(Guid UserId) : ICommand<Result>;

public sealed record DeactivateUserCommand(Guid UserId) : ICommand<Result>;
