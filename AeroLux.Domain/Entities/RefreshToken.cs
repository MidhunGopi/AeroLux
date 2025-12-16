using AeroLux.Domain.Common;

namespace AeroLux.Domain.Entities;

/// <summary>
/// Refresh token entity for JWT token refresh functionality
/// </summary>
public class RefreshToken : Entity
{
    public Guid UserId { get; private set; }
    public User User { get; private set; } = null!;

    public string Token { get; private set; }
    public DateTime ExpiresAt { get; private set; }
    public DateTime? RevokedAt { get; private set; }
    public string? ReplacedByToken { get; private set; }
    public string? RevokedReason { get; private set; }
    public string CreatedByIp { get; private set; }
    public string? RevokedByIp { get; private set; }

    private RefreshToken() { } // EF Core

    public RefreshToken(Guid userId, string token, DateTime expiresAt, string createdByIp)
    {
        UserId = userId;
        Token = token ?? throw new ArgumentNullException(nameof(token));
        ExpiresAt = expiresAt;
        CreatedByIp = createdByIp ?? throw new ArgumentNullException(nameof(createdByIp));
    }

    public bool IsExpired => DateTime.UtcNow >= ExpiresAt;
    public bool IsRevoked => RevokedAt.HasValue;
    public bool IsActive => !IsRevoked && !IsExpired;

    public void Revoke(string? revokedByIp = null, string? reason = null, string? replacedByToken = null)
    {
        RevokedAt = DateTime.UtcNow;
        RevokedByIp = revokedByIp;
        RevokedReason = reason;
        ReplacedByToken = replacedByToken;
        MarkAsUpdated();
    }
}
