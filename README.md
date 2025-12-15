# AeroLux - Enterprise Aviation Platform

AeroLux is an enterprise-grade, event-driven backend platform designed to power a private jet charter booking and aviation operations system. The platform models real-world aviation workflows, handling complex roles, safety-critical approvals, and real-time flight operations.

## 🏗️ Architecture

This repository contains backend microservices built using **.NET 10 Web API**, following:

- **Clean Architecture** - Clear separation of concerns with Domain, Application, Infrastructure, and API layers
- **Domain-Driven Design (DDD)** - Rich domain models with entities, value objects, and aggregates
- **CQRS Pattern** - Command Query Responsibility Segregation using MediatR
- **Event-Driven Architecture** - Domain events for cross-aggregate communication
- **Saga Orchestration** - Complex workflow management with compensating transactions

## 📦 Project Structure

```
AeroLux/
├── AeroLux.Domain/           # Core domain layer (entities, value objects, events)
│   ├── Common/               # Base classes (Entity, ValueObject, IDomainEvent)
│   ├── Entities/             # Domain aggregates (Aircraft, Booking, Flight, Customer)
│   ├── ValueObjects/         # Value objects (Money, Address)
│   ├── Events/               # Domain events
│   └── Enums/                # Domain enumerations
│
├── AeroLux.Application/      # Application layer (CQRS, business logic)
│   ├── Commands/             # Command handlers (CreateBooking, ScheduleFlight)
│   ├── Queries/              # Query handlers (GetBooking, GetFlight)
│   ├── DTOs/                 # Data Transfer Objects
│   ├── EventHandlers/        # Domain event handlers
│   ├── Sagas/                # Saga orchestrators
│   └── Interfaces/           # Application interfaces
│
├── AeroLux.Infrastructure/   # Infrastructure layer (persistence, external services)
│   ├── Persistence/          # EF Core DbContext and UnitOfWork
│   └── Repositories/         # Repository implementations
│
└── AeroLux.API/              # API layer (REST controllers, configuration)
    ├── Controllers/          # API controllers
    └── Program.cs            # Application startup and DI configuration
```

## 🚀 Key Features

### Domain-Driven Design
- **Aggregates**: Aircraft, Booking, Flight, Customer
- **Value Objects**: Money (with currency), Address
- **Domain Events**: FlightScheduled, BookingCreated, BookingConfirmed, FlightCompleted
- **Rich Domain Models**: Business logic encapsulated in entities

### CQRS Pattern
- **Commands**: CreateBooking, ConfirmBooking, ScheduleFlight
- **Queries**: GetBookingById, GetFlightById
- **MediatR Integration**: Centralized request/response pipeline

### Event-Driven Architecture
- **Domain Events**: Published when aggregate state changes
- **Event Handlers**: React to domain events for cross-cutting concerns
- **Automatic Dispatch**: Events dispatched during UnitOfWork SaveChanges

### Saga Orchestration
- **BookingSaga**: Orchestrates multi-step booking process
- **Compensating Transactions**: Automatic rollback on failures
- **State Management**: Tracks saga execution state

## 🛠️ Technologies

- **.NET 10** - Latest .NET framework
- **Entity Framework Core 10** - ORM with In-Memory database
- **MediatR** - CQRS implementation
- **ASP.NET Core Web API** - REST API framework
- **OpenAPI/Swagger** - API documentation

## 🏃 Getting Started

### Prerequisites
- .NET 10 SDK

### Build and Run

```bash
# Build the solution
dotnet build

# Run the API
cd AeroLux.API
dotnet run
```

The API will start on `http://localhost:5237`

### Test the API

**Root Endpoint:**
```bash
curl http://localhost:5237/
```

**Schedule a Flight:**
```bash
curl -X POST http://localhost:5237/api/flights \
  -H "Content-Type: application/json" \
  -d '{
    "aircraftId": "123e4567-e89b-12d3-a456-426614174000",
    "departureAirport": "JFK",
    "arrivalAirport": "LAX",
    "scheduledDepartureTime": "2025-12-20T10:00:00Z",
    "scheduledArrivalTime": "2025-12-20T16:00:00Z",
    "flightNumber": "AL101"
  }'
```

**Create a Booking:**
```bash
curl -X POST http://localhost:5237/api/bookings \
  -H "Content-Type: application/json" \
  -d '{
    "customerId": "223e4567-e89b-12d3-a456-426614174001",
    "flightId": "<flight-id-from-previous-response>",
    "amount": 25000.00,
    "currency": "USD",
    "passengerCount": 4,
    "specialRequests": "Vegetarian meals",
    "requiresCatering": true
  }'
```

**Confirm a Booking:**
```bash
curl -X POST http://localhost:5237/api/bookings/<booking-id>/confirm
```

**Get Booking Details:**
```bash
curl http://localhost:5237/api/bookings/<booking-id>
```

## 📡 API Endpoints

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/` | Service information and available endpoints |
| GET | `/health` | Health check endpoint |
| GET | `/openapi/v1.json` | OpenAPI specification |
| POST | `/api/flights` | Schedule a new flight |
| GET | `/api/flights/{id}` | Get flight details |
| POST | `/api/bookings` | Create a new booking |
| GET | `/api/bookings/{id}` | Get booking details |
| POST | `/api/bookings/{id}/confirm` | Confirm a booking |

## 🎯 Design Patterns

### Clean Architecture
- **Dependencies flow inward**: API → Infrastructure → Application → Domain
- **Domain layer has no dependencies**: Pure business logic
- **Infrastructure implements interfaces**: Dependency Inversion Principle

### CQRS
- **Command handlers**: Modify state, return IDs
- **Query handlers**: Read-only, return DTOs
- **Separation of concerns**: Different models for reads and writes

### Repository Pattern
- **Generic repository**: CRUD operations for all entities
- **Unit of Work**: Transaction management and change tracking
- **Abstraction**: Application layer depends on interfaces, not implementations

### Event Sourcing Foundation
- **Domain events**: Capture state changes as events
- **Event handlers**: React to domain events
- **Audit trail**: Complete history of domain changes

## 🔐 Enterprise Patterns

- **Unit of Work**: Transactional consistency
- **Repository Pattern**: Data access abstraction
- **Dependency Injection**: Loose coupling and testability
- **Saga Pattern**: Long-running transactions with compensation
- **Domain Events**: Event-driven communication between aggregates

## 📝 Example Workflow

1. **Schedule Flight**: Create a flight with departure/arrival airports and times
   - Domain Event: `FlightScheduledEvent` is published
   - Event Handler: Logs flight details for monitoring

2. **Create Booking**: Customer books a flight
   - Domain Event: `BookingCreatedEvent` is published
   - Event Handler: Could trigger payment processing, notifications

3. **Confirm Booking**: Booking moves from Pending to Confirmed
   - Domain Event: `BookingConfirmedEvent` is published
   - Saga: Could orchestrate crew assignment, catering, ground services

4. **Flight Operations**: Update flight status (Boarding, InFlight, Landed)
   - Domain Events: `FlightDepartedEvent`, `FlightCompletedEvent`
   - Event Handlers: Update connected systems, complete bookings

## 🧪 Testing

The platform demonstrates enterprise testing strategies:
- **Domain Logic**: Unit tests for entities and value objects
- **Application Logic**: Handler tests with mocked dependencies
- **Integration Tests**: End-to-end API tests
- **Event Processing**: Event handler verification

## 🐳 Docker Support

The platform includes Docker support for easy deployment:

```bash
# Build and run with Docker Compose
docker-compose up --build

# The API will be available at http://localhost:8080
```

## 🌟 Future Enhancements

- Message broker integration (RabbitMQ/Kafka)
- SQL Server/PostgreSQL persistence
- Authentication & Authorization
- API versioning
- Rate limiting
- Distributed tracing
- Metrics and monitoring
- CI/CD pipeline

## 📄 License

This is a demonstration project showcasing enterprise .NET patterns and practices.

## 👥 Contributing

This project demonstrates best practices for enterprise .NET development. Feel free to use it as a reference or starting point for your own projects.
