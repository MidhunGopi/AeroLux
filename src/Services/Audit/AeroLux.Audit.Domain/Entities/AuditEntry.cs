using AeroLux.Shared.Kernel.DDD;

namespace AeroLux.Audit.Domain.Entities;

/// <summary>
/// Audit action type enumeration
/// </summary>
public sealed class AuditActionType : Enumeration<AuditActionType>
{
    public static readonly AuditActionType Create = new(1, nameof(Create));
    public static readonly AuditActionType Update = new(2, nameof(Update));
    public static readonly AuditActionType Delete = new(3, nameof(Delete));
    public static readonly AuditActionType Read = new(4, nameof(Read));
    public static readonly AuditActionType Login = new(5, nameof(Login));
    public static readonly AuditActionType Logout = new(6, nameof(Logout));
    public static readonly AuditActionType Approve = new(7, nameof(Approve));
    public static readonly AuditActionType Reject = new(8, nameof(Reject));
    public static readonly AuditActionType StatusChange = new(9, nameof(StatusChange));

    private AuditActionType(int value, string name) : base(value, name) { }
}

/// <summary>
/// Immutable audit log entry for compliance tracking
/// </summary>
public sealed class AuditEntry : Entity<Guid>
{
    public DateTime Timestamp { get; private set; }
    public Guid? UserId { get; private set; }
    public string? UserEmail { get; private set; }
    public string? UserRole { get; private set; }
    public AuditActionType ActionType { get; private set; } = null!;
    public string EntityType { get; private set; } = string.Empty;
    public string? EntityId { get; private set; }
    public string ServiceName { get; private set; } = string.Empty;
    public string? OldValues { get; private set; }
    public string? NewValues { get; private set; }
    public string? IpAddress { get; private set; }
    public string? UserAgent { get; private set; }
    public string? CorrelationId { get; private set; }
    public bool IsCompliant { get; private set; }

    private AuditEntry() { }

    public static AuditEntry Create(
        AuditActionType actionType,
        string entityType,
        string? entityId,
        string serviceName,
        Guid? userId = null,
        string? userEmail = null,
        string? userRole = null,
        string? oldValues = null,
        string? newValues = null,
        string? ipAddress = null,
        string? userAgent = null,
        string? correlationId = null)
    {
        return new AuditEntry
        {
            Id = Guid.NewGuid(),
            Timestamp = DateTime.UtcNow,
            UserId = userId,
            UserEmail = userEmail,
            UserRole = userRole,
            ActionType = actionType,
            EntityType = entityType,
            EntityId = entityId,
            ServiceName = serviceName,
            OldValues = oldValues,
            NewValues = newValues,
            IpAddress = ipAddress,
            UserAgent = userAgent,
            CorrelationId = correlationId,
            IsCompliant = true
        };
    }

    public void MarkAsNonCompliant()
    {
        IsCompliant = false;
    }
}
