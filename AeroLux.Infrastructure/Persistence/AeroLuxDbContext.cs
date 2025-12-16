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
    public DbSet<User> Users { get; set; }
    public DbSet<Role> Roles { get; set; }
    public DbSet<UserRole> UserRoles { get; set; }
    public DbSet<RefreshToken> RefreshTokens { get; set; }

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
            entity.Ignore(e => e.DomainEvents);
        });

        // Configure Customer
        modelBuilder.Entity<Customer>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.FirstName).IsRequired().HasMaxLength(100);
            entity.Property(e => e.LastName).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Email).IsRequired().HasMaxLength(200);
            entity.Property(e => e.PhoneNumber).IsRequired().HasMaxLength(20);
            entity.OwnsOne(e => e.BillingAddress, address =>
            {
                address.Property(a => a.Street).HasMaxLength(200);
                address.Property(a => a.City).HasMaxLength(100);
                address.Property(a => a.Country).HasMaxLength(100);
                address.Property(a => a.PostalCode).HasMaxLength(20);
            });
            entity.Ignore(e => e.DomainEvents);
        });

        // Configure Flight
        modelBuilder.Entity<Flight>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.DepartureAirport).IsRequired().HasMaxLength(10);
            entity.Property(e => e.ArrivalAirport).IsRequired().HasMaxLength(10);
            entity.Property(e => e.FlightNumber).HasMaxLength(20);
            entity.HasOne<Aircraft>()
                  .WithMany()
                  .HasForeignKey(e => e.AircraftId)
                  .OnDelete(DeleteBehavior.Restrict);
            entity.Ignore(e => e.DomainEvents);
        });

        // Configure Booking
        modelBuilder.Entity<Booking>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.OwnsOne(e => e.TotalPrice, money =>
            {
                money.Property(m => m.Amount).HasColumnName("TotalPriceAmount").HasPrecision(18, 2).IsRequired();
                money.Property(m => m.Currency).HasColumnName("TotalPriceCurrency").IsRequired().HasMaxLength(3);
            });
            entity.Property(e => e.SpecialRequests).HasMaxLength(1000);
            entity.HasOne<Customer>()
                  .WithMany()
                  .HasForeignKey(e => e.CustomerId)
                  .OnDelete(DeleteBehavior.Restrict);
            entity.HasOne<Flight>()
                  .WithMany()
                  .HasForeignKey(e => e.FlightId)
                  .OnDelete(DeleteBehavior.Restrict);
            entity.Ignore(e => e.DomainEvents);
        });

        // Configure User
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Username).IsRequired().HasMaxLength(100);
            entity.HasIndex(e => e.Username).IsUnique();
            entity.Property(e => e.Email).IsRequired().HasMaxLength(200);
            entity.HasIndex(e => e.Email).IsUnique();
            entity.Property(e => e.PasswordHash).IsRequired().HasMaxLength(500);
            entity.Property(e => e.FirstName).HasMaxLength(100);
            entity.Property(e => e.LastName).HasMaxLength(100);
            entity.Ignore(e => e.DomainEvents);
        });

        // Configure Role
        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(50);
            entity.HasIndex(e => e.Name).IsUnique();
            entity.Property(e => e.Description).HasMaxLength(200);
            entity.Ignore(e => e.DomainEvents);
        });

        // Configure UserRole (many-to-many join table)
        modelBuilder.Entity<UserRole>(entity =>
        {
            entity.HasKey(e => new { e.UserId, e.RoleId });
            entity.HasOne(e => e.User)
                  .WithMany(u => u.UserRoles)
                  .HasForeignKey(e => e.UserId)
                  .OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(e => e.Role)
                  .WithMany(r => r.UserRoles)
                  .HasForeignKey(e => e.RoleId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // Configure RefreshToken
        modelBuilder.Entity<RefreshToken>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Token).IsRequired().HasMaxLength(500);
            entity.HasIndex(e => e.Token).IsUnique();
            entity.Property(e => e.CreatedByIp).IsRequired().HasMaxLength(50);
            entity.Property(e => e.RevokedByIp).HasMaxLength(50);
            entity.Property(e => e.RevokedReason).HasMaxLength(200);
            entity.Property(e => e.ReplacedByToken).HasMaxLength(500);
            entity.HasOne(e => e.User)
                  .WithMany(u => u.RefreshTokens)
                  .HasForeignKey(e => e.UserId)
                  .OnDelete(DeleteBehavior.Cascade);
            entity.Ignore(e => e.DomainEvents);
        });

        // Seed default roles
        SeedRoles(modelBuilder);
    }

    private static void SeedRoles(ModelBuilder modelBuilder)
    {
        // Use a fixed date for seeding to avoid PendingModelChangesWarning
        var seedDate = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        
        modelBuilder.Entity<Role>().HasData(
            new { Id = Guid.Parse("11111111-1111-1111-1111-111111111111"), Name = "Admin", Description = "System administrator with full access", IsSystemRole = true, CreatedAt = seedDate, UpdatedAt = (DateTime?)null },
            new { Id = Guid.Parse("22222222-2222-2222-2222-222222222222"), Name = "Manager", Description = "Manager with elevated privileges", IsSystemRole = true, CreatedAt = seedDate, UpdatedAt = (DateTime?)null },
            new { Id = Guid.Parse("33333333-3333-3333-3333-333333333333"), Name = "Staff", Description = "Staff member with standard access", IsSystemRole = true, CreatedAt = seedDate, UpdatedAt = (DateTime?)null },
            new { Id = Guid.Parse("44444444-4444-4444-4444-444444444444"), Name = "Customer", Description = "Customer with limited access", IsSystemRole = true, CreatedAt = seedDate, UpdatedAt = (DateTime?)null }
        );
    }
}
