using AeroLux.Identity.Application.DTOs;
using AeroLux.Shared.Kernel.CQRS;
using AeroLux.Shared.Kernel.Results;

namespace AeroLux.Identity.Application.Queries;

public sealed record GetUserByIdQuery(Guid UserId) : IQuery<Result<UserDto>>;

public sealed record GetUserByEmailQuery(string Email) : IQuery<Result<UserDto>>;

public sealed record GetAllUsersQuery : IQuery<IReadOnlyList<UserDto>>;
