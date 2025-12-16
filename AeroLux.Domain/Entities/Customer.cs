using AeroLux.Domain.Common;
using AeroLux.Domain.ValueObjects;

namespace AeroLux.Domain.Entities;

/// <summary>
/// Customer aggregate root
/// </summary>
public class Customer : Entity
{
    public string FirstName { get; private set; }
    public string LastName { get; private set; }
    public string Email { get; private set; }
    public string PhoneNumber { get; private set; }
    public Address? BillingAddress { get; private set; }
    public bool IsVip { get; private set; }

    private Customer() { } // EF Core

    public Customer(string firstName, string lastName, string email, string phoneNumber)
    {
        FirstName = firstName ?? throw new ArgumentNullException(nameof(firstName));
        LastName = lastName ?? throw new ArgumentNullException(nameof(lastName));
        Email = email ?? throw new ArgumentNullException(nameof(email));
        PhoneNumber = phoneNumber ?? throw new ArgumentNullException(nameof(phoneNumber));
        IsVip = false;
    }

    public void UpdateContactInfo(string email, string phoneNumber)
    {
        Email = email ?? throw new ArgumentNullException(nameof(email));
        PhoneNumber = phoneNumber ?? throw new ArgumentNullException(nameof(phoneNumber));
        MarkAsUpdated();
    }

    public void SetBillingAddress(Address address)
    {
        BillingAddress = address ?? throw new ArgumentNullException(nameof(address));
        MarkAsUpdated();
    }

    public void PromoteToVip()
    {
        IsVip = true;
        MarkAsUpdated();
    }

    public string FullName => $"{FirstName} {LastName}";
}
