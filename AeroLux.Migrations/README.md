# AeroLux Database Migrations

This document provides step-by-step instructions for setting up and running Entity Framework Core migrations for the AeroLux Aviation Platform.

## Prerequisites

1. **.NET 10 SDK** - Ensure you have .NET 10 SDK installed
   ```powershell
   dotnet --version
   ```

2. **SQL Server LocalDB** - Ensure SQL Server LocalDB is installed (comes with Visual Studio)
   - Instance name: `(localdb)\MSSQLLocalDB`
   - Uses Windows Authentication

## Quick Start (Easiest Method)

### Running Migrations from Visual Studio

1. **Set AeroLux.Migrations as Startup Project**
   - Right-click on `AeroLux.Migrations` in Solution Explorer
   - Select **"Set as Startup Project"**

2. **Run the Project**
   - Press **F5** or click the green **Start** button
   - The console will display migration progress and status

That's it! The database and all tables will be created automatically.

## Project Structure

```
AeroLux/
??? AeroLux.Domain/           # Domain entities
??? AeroLux.Application/      # Application layer
??? AeroLux.Infrastructure/   # DbContext and repositories
??? AeroLux.Migrations/       # Migration project (run this to apply migrations)
??? AeroLux.API/              # Web API
```

## Configuration

### Connection String

The connection string is configured in `AeroLux.Migrations/appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\MSSQLLocalDB;Database=AeroLuxDb;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True"
  }
}
```

**To use a different SQL Server instance**, update the connection string:

| Scenario | Connection String Example |
|----------|--------------------------|
| Local SQL Server | `Server=localhost;Database=AeroLuxDb;Trusted_Connection=True;TrustServerCertificate=True` |
| SQL Server with credentials | `Server=your-server;Database=AeroLuxDb;User Id=your-user;Password=your-password;TrustServerCertificate=True` |
| Azure SQL | `Server=your-server.database.windows.net;Database=AeroLuxDb;User Id=your-user;Password=your-password;Encrypt=True` |

## Alternative: Command Line Setup

If you prefer using the command line:

### Step 1: Clone the Repository

```powershell
git clone https://github.com/MidhunGopi/AeroLux.git
cd AeroLux
```

### Step 2: Install EF Core CLI Tools (if not installed)

```powershell
dotnet tool install --global dotnet-ef
```

### Step 3: Restore and Build

```powershell
dotnet restore
dotnet build
```

### Step 4: Apply Migrations

```powershell
cd AeroLux.Migrations
dotnet run
```

## Creating New Migrations

When you make changes to domain entities or DbContext configuration:

### Step 1: Make Your Entity Changes

Edit files in `AeroLux.Domain/Entities/` or `AeroLux.Infrastructure/Persistence/AeroLuxDbContext.cs`

### Step 2: Create a New Migration

From the solution root directory:

```powershell
dotnet ef migrations add <MigrationName> --project AeroLux.Migrations --startup-project AeroLux.Migrations --context AeroLuxDbContext
```

Example:
```powershell
dotnet ef migrations add AddPaymentTable --project AeroLux.Migrations --startup-project AeroLux.Migrations --context AeroLuxDbContext
```

### Step 3: Apply the Migration

**Option A: Run from Visual Studio**
- Set `AeroLux.Migrations` as Startup Project
- Press F5

**Option B: Run from command line**
```powershell
cd AeroLux.Migrations
dotnet run
```

## Removing a Migration

To remove the last migration (if not yet applied):

```powershell
dotnet ef migrations remove --project AeroLux.Migrations --startup-project AeroLux.Migrations --context AeroLuxDbContext
```

## Rolling Back Migrations

To roll back to a specific migration:

```powershell
dotnet ef database update <PreviousMigrationName> --project AeroLux.Migrations --startup-project AeroLux.Migrations --context AeroLuxDbContext
```

To roll back all migrations:

```powershell
dotnet ef database update 0 --project AeroLux.Migrations --startup-project AeroLux.Migrations --context AeroLuxDbContext
```

## Database Tables

After running migrations, the following tables will be created:

| Table | Description |
|-------|-------------|
| `Aircraft` | Private jets with registration, model, manufacturer, capacity |
| `Customers` | Customer profiles with billing address |
| `Flights` | Scheduled flights with departure/arrival info |
| `Bookings` | Charter bookings linking customers to flights |
| `Users` | Authentication - username, email, password hash, lockout support |
| `Roles` | Authorization roles (Admin, Manager, Staff, Customer) |
| `UserRoles` | Many-to-many join table for User-Role relationships |
| `RefreshTokens` | JWT refresh tokens with expiration and revocation tracking |

### Seeded Data

The following roles are automatically seeded:

| Role | Description |
|------|-------------|
| Admin | System administrator with full access |
| Manager | Manager with elevated privileges |
| Staff | Staff member with standard access |
| Customer | Customer with limited access |

## Troubleshooting

### Error: "EF Core tools not found"

Install the EF Core CLI tools:
```powershell
dotnet tool install --global dotnet-ef
```

### Error: "Connection refused" or "Cannot connect to SQL Server"

1. Verify SQL Server LocalDB is running:
   ```powershell
   sqllocaldb info MSSQLLocalDB
   ```

2. Start LocalDB if stopped:
   ```powershell
   sqllocaldb start MSSQLLocalDB
   ```

### Error: "Login failed"

- For LocalDB, ensure you're using Windows Authentication (`Trusted_Connection=True`)
- For SQL Server with credentials, verify username and password

### Error: "Database already exists"

This is not an error - migrations will be applied to the existing database.

### Error: "PendingModelChangesWarning"

This occurs when seed data uses dynamic values like `DateTime.UtcNow`. Ensure seed data uses static values.

## Visual Studio Package Manager Console

If you prefer using Visual Studio's Package Manager Console:

1. Open **Tools** ? **NuGet Package Manager** ? **Package Manager Console**
2. Set **Default project** dropdown to `AeroLux.Migrations`
3. Run commands:

```powershell
# Add a migration
Add-Migration <MigrationName> -Context AeroLuxDbContext

# Update database
Update-Database -Context AeroLuxDbContext

# Remove last migration
Remove-Migration -Context AeroLuxDbContext
```

## Quick Reference Commands

| Action | Method |
|--------|--------|
| Apply all migrations (easiest) | Set `AeroLux.Migrations` as Startup Project ? Press F5 |
| Apply migrations (CLI) | `cd AeroLux.Migrations && dotnet run` |
| Add new migration | `dotnet ef migrations add <Name> --project AeroLux.Migrations --startup-project AeroLux.Migrations --context AeroLuxDbContext` |
| Remove last migration | `dotnet ef migrations remove --project AeroLux.Migrations --startup-project AeroLux\Migrations --context AeroLuxDbContext` |
| List migrations | `dotnet ef migrations list --project AeroLux.Migrations --startup-project AeroLux.Migrations --context AeroLuxDbContext` |

## Support

For issues or questions, please open an issue on the [GitHub repository](https://github.com/MidhunGopi/AeroLux).
