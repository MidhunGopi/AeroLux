using AeroLux.Domain.Common;

namespace AeroLux.Domain.Entities;

/// <summary>
/// Role entity for authorization
/// </summary>
public class Role : Entity
{
    public string Name { get; private set; }
    public string? Description { get; private set; }
    public bool IsSystemRole { get; private set; }

    private readonly List<UserRole> _userRoles = [];
    public IReadOnlyCollection<UserRole> UserRoles => _userRoles.AsReadOnly();

    private Role() { } // EF Core

    public Role(string name, string? description = null, bool isSystemRole = false)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
        Description = description;
        IsSystemRole = isSystemRole;
    }

    public void UpdateDescription(string? description)
    {
        Description = description;
        MarkAsUpdated();
    }

    public void Rename(string newName)
    {
        if (IsSystemRole)
            throw new InvalidOperationException("Cannot rename a system role");

        Name = newName ?? throw new ArgumentNullException(nameof(newName));
        MarkAsUpdated();
    }

    /// <summary>
    /// Predefined system roles
    /// </summary>
    public static class SystemRoles
    {
        public const string Admin = "Admin";
        public const string Manager = "Manager";
        public const string Staff = "Staff";
        public const string Customer = "Customer";
    }
}
