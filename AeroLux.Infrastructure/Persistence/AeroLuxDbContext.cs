using AeroLux.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace AeroLux.Infrastructure.Persistence;

/// <summary>
/// EF Core DbContext for AeroLux
/// </summary>
public class AeroLuxDbContext : DbContext
{
    public AeroLuxDbContext(DbContextOptions<AeroLuxDbContext> options) : base(options)
    {
    }

    public DbSet<Aircraft> Aircraft { get; set; }
    public DbSet<Customer> Customers { get; set; }
    public DbSet<Flight> Flights { get; set; }
    public DbSet<Booking> Bookings { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure Aircraft
        modelBuilder.Entity<Aircraft>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.RegistrationNumber).IsRequired().HasMaxLength(20);
            entity.Property(e => e.Model).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Manufacturer).IsRequired().HasMaxLength(100);
        });

        // Configure Customer
        modelBuilder.Entity<Customer>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.FirstName).IsRequired().HasMaxLength(100);
            entity.Property(e => e.LastName).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Email).IsRequired().HasMaxLength(200);
            entity.Property(e => e.PhoneNumber).IsRequired().HasMaxLength(20);
            entity.OwnsOne(e => e.BillingAddress);
        });

        // Configure Flight
        modelBuilder.Entity<Flight>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.DepartureAirport).IsRequired().HasMaxLength(10);
            entity.Property(e => e.ArrivalAirport).IsRequired().HasMaxLength(10);
            entity.Property(e => e.FlightNumber).HasMaxLength(20);
            entity.Ignore(e => e.DomainEvents);
        });

        // Configure Booking
        modelBuilder.Entity<Booking>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.OwnsOne(e => e.TotalPrice, money =>
            {
                money.Property(m => m.Amount).IsRequired();
                money.Property(m => m.Currency).IsRequired().HasMaxLength(3);
            });
            entity.Property(e => e.SpecialRequests).HasMaxLength(1000);
            entity.Ignore(e => e.DomainEvents);
        });
    }
}
