using AeroLux.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace AeroLux.Migrations;

/// <summary>
/// Design-time factory for creating AeroLuxDbContext for EF Core migrations.
/// This is used by the EF Core tools (dotnet ef migrations) to create the DbContext.
/// </summary>
public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<AeroLuxDbContext>
{
    public AeroLuxDbContext CreateDbContext(string[] args)
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .Build();

        var connectionString = configuration.GetConnectionString("DefaultConnection");

        var optionsBuilder = new DbContextOptionsBuilder<AeroLuxDbContext>();
        optionsBuilder.UseSqlServer(connectionString, options =>
        {
            options.MigrationsAssembly(typeof(DesignTimeDbContextFactory).Assembly.FullName);
            options.EnableRetryOnFailure(
                maxRetryCount: 3,
                maxRetryDelay: TimeSpan.FromSeconds(30),
                errorNumbersToAdd: null);
        });

        return new AeroLuxDbContext(optionsBuilder.Options);
    }
}
