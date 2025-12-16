using AeroLux.Application.Interfaces;
using AeroLux.Application.Sagas;
using AeroLux.Infrastructure.Persistence;
using AeroLux.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApi();
builder.Services.AddSwaggerGen();

// Configure DbContext with In-Memory database for demonstration
builder.Services.AddDbContext<AeroLuxDbContext>(options =>
    options.UseInMemoryDatabase("AeroLuxDb"));

// Register MediatR for CQRS
builder.Services.AddMediatR(cfg => {
    cfg.RegisterServicesFromAssembly(Assembly.Load("AeroLux.Application"));
});

// Register repositories and unit of work
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// Register Sagas
builder.Services.AddScoped<BookingSaga>();

// Add health checks
builder.Services.AddHealthChecks();

// Configure CORS for development
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        policy => policy.AllowAnyOrigin()
                       .AllowAnyMethod()
                       .AllowAnyHeader());
});

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "AeroLux API v1");
        options.RoutePrefix = "swagger";
    });
}

app.UseHttpsRedirection();
app.UseCors("AllowAll");
app.UseAuthorization();

app.MapControllers();
app.MapHealthChecks("/health");

// Welcome endpoint
app.MapGet("/", () => new
{
    Service = "AeroLux Aviation Platform",
    Version = "1.0.0",
    Description = "Enterprise-grade, event-driven backend for private jet charter booking",
    Architecture = new[] { "Clean Architecture", "DDD", "CQRS", "Event-Driven", "Saga Pattern" },
    Endpoints = new
    {
        Health = "/health",
        OpenAPI = "/openapi/v1.json",
        Bookings = "/api/bookings",
        Flights = "/api/flights"
    }
})
.WithName("GetServiceInfo")
.WithTags("Info");

app.Run();
