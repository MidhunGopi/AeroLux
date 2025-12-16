using AeroLux.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace AeroLux.Migrations;

/// <summary>
/// Migration runner application.
/// Run this project to automatically apply all pending migrations to the database.
/// </summary>
public class Program
{
    public static async Task Main(string[] args)
    {
        Console.WriteLine("╔════════════════════════════════════════════════════════════╗");
        Console.WriteLine("║         AeroLux Database Migration Tool                    ║");
        Console.WriteLine("╚════════════════════════════════════════════════════════════╝");
        Console.WriteLine();

        try
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            var connectionString = configuration.GetConnectionString("DefaultConnection");

            if (string.IsNullOrEmpty(connectionString))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("ERROR: Connection string 'DefaultConnection' not found in appsettings.json");
                Console.ResetColor();
                Environment.Exit(1);
            }

            Console.WriteLine($"Connecting to database...");
            Console.WriteLine($"Connection: {MaskConnectionString(connectionString)}");
            Console.WriteLine();

            var optionsBuilder = new DbContextOptionsBuilder<AeroLuxDbContext>();
            optionsBuilder.UseSqlServer(connectionString, options =>
            {
                options.MigrationsAssembly(typeof(Program).Assembly.FullName);
                options.EnableRetryOnFailure(
                    maxRetryCount: 3,
                    maxRetryDelay: TimeSpan.FromSeconds(30),
                    errorNumbersToAdd: null);
                options.CommandTimeout(120);
            });

            await using var context = new AeroLuxDbContext(optionsBuilder.Options);

            Console.WriteLine("Checking for pending migrations...");
            var pendingMigrations = (await context.Database.GetPendingMigrationsAsync()).ToList();

            if (pendingMigrations.Count == 0)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("✓ Database is up to date. No pending migrations.");
                Console.ResetColor();
            }
            else
            {
                Console.WriteLine($"Found {pendingMigrations.Count} pending migration(s):");
                foreach (var migration in pendingMigrations)
                {
                    Console.WriteLine($"  • {migration}");
                }
                Console.WriteLine();

                Console.WriteLine("Applying migrations...");
                await context.Database.MigrateAsync();

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine();
                Console.WriteLine("✓ All migrations applied successfully!");
                Console.ResetColor();
            }

            // Display applied migrations
            Console.WriteLine();
            Console.WriteLine("Applied migrations:");
            var appliedMigrations = await context.Database.GetAppliedMigrationsAsync();
            foreach (var migration in appliedMigrations)
            {
                Console.WriteLine($"  ✓ {migration}");
            }

            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("╔════════════════════════════════════════════════════════════╗");
            Console.WriteLine("║         Database setup completed successfully!             ║");
            Console.WriteLine("╚════════════════════════════════════════════════════════════╝");
            Console.ResetColor();
        }
        catch (Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine();
            Console.WriteLine("╔════════════════════════════════════════════════════════════╗");
            Console.WriteLine("║                    MIGRATION FAILED                        ║");
            Console.WriteLine("╚════════════════════════════════════════════════════════════╝");
            Console.WriteLine();
            Console.WriteLine($"Error: {ex.Message}");
            Console.WriteLine();
            Console.WriteLine("Stack trace:");
            Console.WriteLine(ex.StackTrace);
            Console.ResetColor();
            Environment.Exit(1);
        }
    }

    private static string MaskConnectionString(string connectionString)
    {
        // Mask password if present
        var parts = connectionString.Split(';');
        for (int i = 0; i < parts.Length; i++)
        {
            if (parts[i].StartsWith("Password=", StringComparison.OrdinalIgnoreCase) ||
                parts[i].StartsWith("Pwd=", StringComparison.OrdinalIgnoreCase))
            {
                var key = parts[i].Split('=')[0];
                parts[i] = $"{key}=********";
            }
        }
        return string.Join(';', parts);
    }
}
