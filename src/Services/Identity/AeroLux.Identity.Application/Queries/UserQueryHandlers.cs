using AeroLux.Identity.Application.DTOs;
using AeroLux.Identity.Application.Interfaces;
using AeroLux.Shared.Kernel.CQRS;
using AeroLux.Shared.Kernel.Results;

namespace AeroLux.Identity.Application.Queries;

public sealed class GetUserByIdQueryHandler : IQueryHandler<GetUserByIdQuery, Result<UserDto>>
{
    private readonly IUserRepository _userRepository;

    public GetUserByIdQueryHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<Result<UserDto>> HandleAsync(GetUserByIdQuery query, CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.GetByIdAsync(query.UserId, cancellationToken);

        if (user is null)
        {
            return Result.Failure<UserDto>(new Error("User.NotFound", "User not found."));
        }

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

public sealed class GetAllUsersQueryHandler : IQueryHandler<GetAllUsersQuery, IReadOnlyList<UserDto>>
{
    private readonly IUserRepository _userRepository;

    public GetAllUsersQueryHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<IReadOnlyList<UserDto>> HandleAsync(GetAllUsersQuery query, CancellationToken cancellationToken = default)
    {
        var users = await _userRepository.GetAllAsync(cancellationToken);

        return users.Select(u => new UserDto(
            u.Id,
            u.Email.Value,
            u.FirstName,
            u.LastName,
            u.Roles,
            u.IsActive,
            u.CreatedAt,
            u.LastLoginAt)).ToList().AsReadOnly();
    }
}
