using AeroLux.Identity.Application.DTOs;
using AeroLux.Identity.Application.Interfaces;
using AeroLux.Identity.Domain.Entities;
using AeroLux.Identity.Domain.ValueObjects;
using AeroLux.Shared.Infrastructure.Authentication;
using AeroLux.Shared.Kernel.Authentication;
using AeroLux.Shared.Kernel.CQRS;
using AeroLux.Shared.Kernel.Results;

namespace AeroLux.Identity.Application.Commands;

public sealed class RegisterUserCommandHandler : ICommandHandler<RegisterUserCommand, Result<UserDto>>
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;

    public RegisterUserCommandHandler(IUserRepository userRepository, IPasswordHasher passwordHasher)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
    }

    public async Task<Result<UserDto>> HandleAsync(RegisterUserCommand command, CancellationToken cancellationToken = default)
    {
        if (await _userRepository.ExistsByEmailAsync(command.Email, cancellationToken))
        {
            return Result.Failure<UserDto>(new Error("User.EmailExists", "A user with this email already exists."));
        }

        var email = Email.Create(command.Email);
        var passwordHash = _passwordHasher.Hash(command.Password);
        var roles = command.Roles ?? [UserRoles.Passenger];

        var user = User.Create(email, passwordHash, command.FirstName, command.LastName, roles);
        await _userRepository.AddAsync(user, cancellationToken);

        return Result.Success(new UserDto(
            user.Id,
            user.Email.Value,
            user.FirstName,
            user.LastName,
            user.Roles,
            user.IsActive,
            user.CreatedAt,
            user.LastLoginAt));
    }
}

public sealed class LoginCommandHandler : ICommandHandler<LoginCommand, Result<AuthTokens>>
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly JwtTokenService _jwtTokenService;
    private readonly JwtSettings _jwtSettings;

    public LoginCommandHandler(
        IUserRepository userRepository,
        IPasswordHasher passwordHasher,
        JwtTokenService jwtTokenService,
        JwtSettings jwtSettings)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _jwtTokenService = jwtTokenService;
        _jwtSettings = jwtSettings;
    }

    public async Task<Result<AuthTokens>> HandleAsync(LoginCommand command, CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.GetByEmailAsync(command.Email, cancellationToken);

        if (user is null || !_passwordHasher.Verify(command.Password, user.PasswordHash))
        {
            return Result.Failure<AuthTokens>(new Error("Auth.InvalidCredentials", "Invalid email or password."));
        }

        if (!user.IsActive)
        {
            return Result.Failure<AuthTokens>(new Error("Auth.UserDeactivated", "This account has been deactivated."));
        }

        var accessToken = _jwtTokenService.GenerateAccessToken(user.Id, user.Email.Value, user.Roles);
        var refreshToken = _jwtTokenService.GenerateRefreshToken();
        var refreshTokenExpiry = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenExpirationDays);

        user.RecordLogin(refreshToken, refreshTokenExpiry);
        await _userRepository.UpdateAsync(user, cancellationToken);

        return Result.Success(new AuthTokens(
            accessToken,
            refreshToken,
            DateTime.UtcNow.AddMinutes(_jwtSettings.AccessTokenExpirationMinutes)));
    }
}

/// <summary>
/// Interface for password hashing
/// </summary>
public interface IPasswordHasher
{
    string Hash(string password);
    bool Verify(string password, string passwordHash);
}
