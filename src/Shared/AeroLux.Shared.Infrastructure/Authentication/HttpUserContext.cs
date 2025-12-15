using System.Security.Claims;
using AeroLux.Shared.Kernel.Authentication;
using Microsoft.AspNetCore.Http;

namespace AeroLux.Shared.Infrastructure.Authentication;

/// <summary>
/// HTTP context-based user context implementation
/// </summary>
public class HttpUserContext : IUserContext
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public HttpUserContext(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public Guid? UserId
    {
        get
        {
            var userIdClaim = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier);
            return userIdClaim is not null && Guid.TryParse(userIdClaim.Value, out var userId) 
                ? userId 
                : null;
        }
    }

    public string? Email => _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.Email)?.Value;

    public IReadOnlyList<string> Roles
    {
        get
        {
            var roles = _httpContextAccessor.HttpContext?.User
                .FindAll(ClaimTypes.Role)
                .Select(c => c.Value)
                .ToList() ?? [];
            return roles.AsReadOnly();
        }
    }

    public IReadOnlyList<string> Permissions
    {
        get
        {
            var permissions = _httpContextAccessor.HttpContext?.User
                .FindAll("permission")
                .Select(c => c.Value)
                .ToList() ?? [];
            return permissions.AsReadOnly();
        }
    }

    public bool IsAuthenticated => _httpContextAccessor.HttpContext?.User.Identity?.IsAuthenticated ?? false;
}
