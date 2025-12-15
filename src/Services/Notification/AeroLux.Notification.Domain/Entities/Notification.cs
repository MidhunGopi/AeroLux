using AeroLux.Shared.Kernel.DDD;

namespace AeroLux.Notification.Domain.Entities;

/// <summary>
/// Notification channel enumeration
/// </summary>
public sealed class NotificationChannel : Enumeration<NotificationChannel>
{
    public static readonly NotificationChannel Email = new(1, nameof(Email));
    public static readonly NotificationChannel SMS = new(2, nameof(SMS));
    public static readonly NotificationChannel Push = new(3, nameof(Push));
    public static readonly NotificationChannel InApp = new(4, nameof(InApp));

    private NotificationChannel(int value, string name) : base(value, name) { }
}

/// <summary>
/// Notification status enumeration
/// </summary>
public sealed class NotificationStatus : Enumeration<NotificationStatus>
{
    public static readonly NotificationStatus Pending = new(1, nameof(Pending));
    public static readonly NotificationStatus Sent = new(2, nameof(Sent));
    public static readonly NotificationStatus Delivered = new(3, nameof(Delivered));
    public static readonly NotificationStatus Read = new(4, nameof(Read));
    public static readonly NotificationStatus Failed = new(5, nameof(Failed));

    private NotificationStatus(int value, string name) : base(value, name) { }
}

/// <summary>
/// Notification aggregate root
/// </summary>
public sealed class Notification : AggregateRoot<Guid>
{
    public Guid RecipientId { get; private set; }
    public string Subject { get; private set; } = string.Empty;
    public string Content { get; private set; } = string.Empty;
    public NotificationChannel Channel { get; private set; } = null!;
    public NotificationStatus Status { get; private set; } = null!;
    public string? TemplateId { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? SentAt { get; private set; }
    public DateTime? ReadAt { get; private set; }
    public string? FailureReason { get; private set; }
    public int RetryCount { get; private set; }

    private Notification() { }

    public static Notification Create(
        Guid recipientId,
        string subject,
        string content,
        NotificationChannel channel,
        string? templateId = null)
    {
        return new Notification
        {
            Id = Guid.NewGuid(),
            RecipientId = recipientId,
            Subject = subject,
            Content = content,
            Channel = channel,
            TemplateId = templateId,
            Status = NotificationStatus.Pending,
            CreatedAt = DateTime.UtcNow,
            RetryCount = 0
        };
    }

    public void MarkAsSent()
    {
        Status = NotificationStatus.Sent;
        SentAt = DateTime.UtcNow;
    }

    public void MarkAsDelivered()
    {
        Status = NotificationStatus.Delivered;
    }

    public void MarkAsRead()
    {
        Status = NotificationStatus.Read;
        ReadAt = DateTime.UtcNow;
    }

    public void MarkAsFailed(string reason)
    {
        Status = NotificationStatus.Failed;
        FailureReason = reason;
        RetryCount++;
    }

    public bool CanRetry() => RetryCount < 3;
}
