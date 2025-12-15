using AeroLux.Shared.Kernel.Events;

namespace AeroLux.Shared.Kernel.DDD;

/// <summary>
/// Marker interface for aggregate roots
/// </summary>
public interface IAggregateRoot
{
    IReadOnlyCollection<IDomainEvent> DomainEvents { get; }
    void ClearDomainEvents();
}
