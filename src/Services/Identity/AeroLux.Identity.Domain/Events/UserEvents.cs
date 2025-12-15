using AeroLux.Shared.Kernel.Events;

namespace AeroLux.Identity.Domain.Events;

public sealed class UserCreatedEvent : DomainEvent
{
    public Guid UserId { get; }
    public string Email { get; }
    public string FirstName { get; }
    public string LastName { get; }

    public UserCreatedEvent(Guid userId, string email, string firstName, string lastName)
    {
        UserId = userId;
        Email = email;
        FirstName = firstName;
        LastName = lastName;
    }
}

public sealed class UserLoggedInEvent : DomainEvent
{
    public Guid UserId { get; }
    public string Email { get; }

    public UserLoggedInEvent(Guid userId, string email)
    {
        UserId = userId;
        Email = email;
    }
}

public sealed class UserDeactivatedEvent : DomainEvent
{
    public Guid UserId { get; }
    public string Email { get; }

    public UserDeactivatedEvent(Guid userId, string email)
    {
        UserId = userId;
        Email = email;
    }
}
