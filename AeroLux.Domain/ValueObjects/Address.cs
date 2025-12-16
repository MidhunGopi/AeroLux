using AeroLux.Domain.Common;

namespace AeroLux.Domain.ValueObjects;

public class Address : ValueObject
{
    public string Street { get; }
    public string City { get; }
    public string Country { get; }
    public string PostalCode { get; }

    private Address() { } // EF Core

    public Address(string street, string city, string country, string postalCode)
    {
        Street = street ?? throw new ArgumentNullException(nameof(street));
        City = city ?? throw new ArgumentNullException(nameof(city));
        Country = country ?? throw new ArgumentNullException(nameof(country));
        PostalCode = postalCode ?? throw new ArgumentNullException(nameof(postalCode));
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Street;
        yield return City;
        yield return Country;
        yield return PostalCode;
    }

    public override string ToString() => $"{Street}, {City}, {Country} {PostalCode}";
}
