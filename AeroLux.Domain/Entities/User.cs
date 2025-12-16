using AeroLux.Domain.Common;

namespace AeroLux.Domain.Entities;

/// <summary>
/// User entity for authentication and authorization
/// </summary>
public class User : Entity
{
    public string Username { get; private set; }
    public string Email { get; private set; }
    public string PasswordHash { get; private set; }
    public string? FirstName { get; private set; }
    public string? LastName { get; private set; }
    public bool IsActive { get; private set; }
    public bool EmailConfirmed { get; private set; }
    public DateTime? LastLoginAt { get; private set; }
    public int FailedLoginAttempts { get; private set; }
    public DateTime? LockoutEndAt { get; private set; }

    private readonly List<UserRole> _userRoles = [];
    public IReadOnlyCollection<UserRole> UserRoles => _userRoles.AsReadOnly();

    private readonly List<RefreshToken> _refreshTokens = [];
    public IReadOnlyCollection<RefreshToken> RefreshTokens => _refreshTokens.AsReadOnly();

    private User() { } // EF Core

    public User(string username, string email, string passwordHash, string? firstName = null, string? lastName = null)
    {
        Username = username ?? throw new ArgumentNullException(nameof(username));
        Email = email ?? throw new ArgumentNullException(nameof(email));
        PasswordHash = passwordHash ?? throw new ArgumentNullException(nameof(passwordHash));
        FirstName = firstName;
        LastName = lastName;
        IsActive = true;
        EmailConfirmed = false;
        FailedLoginAttempts = 0;
    }

    public void UpdatePassword(string newPasswordHash)
    {
        PasswordHash = newPasswordHash ?? throw new ArgumentNullException(nameof(newPasswordHash));
        MarkAsUpdated();
    }

    public void UpdateProfile(string? firstName, string? lastName)
    {
        FirstName = firstName;
        LastName = lastName;
        MarkAsUpdated();
    }

    public void RecordLogin()
    {
        LastLoginAt = DateTime.UtcNow;
        FailedLoginAttempts = 0;
        LockoutEndAt = null;
        MarkAsUpdated();
    }

    public void RecordFailedLogin()
    {
        FailedLoginAttempts++;
        if (FailedLoginAttempts >= 5)
        {
            LockoutEndAt = DateTime.UtcNow.AddMinutes(15);
        }
        MarkAsUpdated();
    }

    public bool IsLockedOut => LockoutEndAt.HasValue && LockoutEndAt.Value > DateTime.UtcNow;

    public void ConfirmEmail()
    {
        EmailConfirmed = true;
        MarkAsUpdated();
    }

    public void Deactivate()
    {
        IsActive = false;
        MarkAsUpdated();
    }

    public void Activate()
    {
        IsActive = true;
        MarkAsUpdated();
    }

    public void AddRole(Role role)
    {
        if (_userRoles.Any(ur => ur.RoleId == role.Id))
            return;

        _userRoles.Add(new UserRole(Id, role.Id));
        MarkAsUpdated();
    }

    public void RemoveRole(Guid roleId)
    {
        var userRole = _userRoles.FirstOrDefault(ur => ur.RoleId == roleId);
        if (userRole != null)
        {
            _userRoles.Remove(userRole);
            MarkAsUpdated();
        }
    }

    public void AddRefreshToken(RefreshToken refreshToken)
    {
        // Remove expired tokens
        _refreshTokens.RemoveAll(rt => rt.ExpiresAt < DateTime.UtcNow);
        _refreshTokens.Add(refreshToken);
    }

    public string FullName => string.IsNullOrWhiteSpace(FirstName) && string.IsNullOrWhiteSpace(LastName)
        ? Username
        : $"{FirstName} {LastName}".Trim();
}
