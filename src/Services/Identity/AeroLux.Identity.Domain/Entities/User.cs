using AeroLux.Identity.Domain.Events;
using AeroLux.Identity.Domain.ValueObjects;
using AeroLux.Shared.Kernel.DDD;

namespace AeroLux.Identity.Domain.Entities;

/// <summary>
/// User aggregate root representing a system user
/// </summary>
public sealed class User : AggregateRoot<Guid>
{
    private readonly List<string> _roles = [];
    
    public Email Email { get; private set; } = null!;
    public string PasswordHash { get; private set; } = string.Empty;
    public string FirstName { get; private set; } = string.Empty;
    public string LastName { get; private set; } = string.Empty;
    public bool IsActive { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? LastLoginAt { get; private set; }
    public string? RefreshToken { get; private set; }
    public DateTime? RefreshTokenExpiryTime { get; private set; }

    public IReadOnlyList<string> Roles => _roles.AsReadOnly();
    public string FullName => $"{FirstName} {LastName}";

    private User() { }

    public static User Create(
        Email email,
        string passwordHash,
        string firstName,
        string lastName,
        IEnumerable<string> roles)
    {
        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = email,
            PasswordHash = passwordHash,
            FirstName = firstName,
            LastName = lastName,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        user._roles.AddRange(roles);
        user.RaiseDomainEvent(new UserCreatedEvent(user.Id, email.Value, firstName, lastName));

        return user;
    }

    public void UpdateProfile(string firstName, string lastName)
    {
        FirstName = firstName;
        LastName = lastName;
    }

    public void AddRole(string role)
    {
        if (!_roles.Contains(role))
        {
            _roles.Add(role);
        }
    }

    public void RemoveRole(string role)
    {
        _roles.Remove(role);
    }

    public void RecordLogin(string refreshToken, DateTime refreshTokenExpiry)
    {
        LastLoginAt = DateTime.UtcNow;
        RefreshToken = refreshToken;
        RefreshTokenExpiryTime = refreshTokenExpiry;
        RaiseDomainEvent(new UserLoggedInEvent(Id, Email.Value));
    }

    public void RevokeRefreshToken()
    {
        RefreshToken = null;
        RefreshTokenExpiryTime = null;
    }

    public void Deactivate()
    {
        IsActive = false;
        RevokeRefreshToken();
        RaiseDomainEvent(new UserDeactivatedEvent(Id, Email.Value));
    }

    public void Activate()
    {
        IsActive = true;
    }

    public void ChangePassword(string newPasswordHash)
    {
        PasswordHash = newPasswordHash;
        RevokeRefreshToken();
    }
}
