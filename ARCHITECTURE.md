# AeroLux Architecture Documentation

## Overview

AeroLux is an enterprise-grade, event-driven backend platform built with .NET 10, demonstrating modern software architecture patterns and practices for a private jet charter booking system.

## Architecture Layers

### 1. Domain Layer (`AeroLux.Domain`)
**Pure business logic with no external dependencies**

#### Entities (Aggregates)
- **Aircraft**: Represents private jets with status, capacity, and maintenance tracking
- **Booking**: Charter booking aggregate with lifecycle management
- **Flight**: Flight scheduling and operations tracking
- **Customer**: Customer profiles and VIP management

#### Value Objects
- **Money**: Amount with currency validation
- **Address**: Structured address with all required fields

#### Domain Events
Domain events capture state changes and enable event-driven communication:
- `FlightScheduledEvent`: Published when a flight is scheduled
- `BookingCreatedEvent`: Published when a booking is created
- `BookingConfirmedEvent`: Published when a booking is confirmed
- `FlightDepartedEvent`: Published when a flight departs
- `FlightCompletedEvent`: Published when a flight lands

#### Base Classes
- **Entity**: Base class for all entities with ID, timestamps, and domain event support
- **ValueObject**: Base class for value objects with equality comparison
- **IDomainEvent**: Marker interface for all domain events (implements `INotification`)

### 2. Application Layer (`AeroLux.Application`)
**Business logic orchestration and CQRS implementation**

#### Commands (Write Operations)
- `CreateBookingCommand`: Creates a new booking
- `ConfirmBookingCommand`: Confirms a pending booking
- `ScheduleFlightCommand`: Schedules a new flight

#### Queries (Read Operations)
- `GetBookingByIdQuery`: Retrieves booking details
- `GetFlightByIdQuery`: Retrieves flight details

#### Event Handlers
- `BookingCreatedEventHandler`: Handles booking creation events
- `FlightScheduledEventHandler`: Handles flight scheduling events

#### Sagas
- `BookingSaga`: Orchestrates the booking workflow with steps:
  1. Validate booking
  2. Reserve aircraft
  3. Process payment
  4. Confirm booking
  - Includes compensating transactions for rollback

#### Interfaces
- `IRepository<T>`: Generic repository interface
- `IUnitOfWork`: Transaction management interface

### 3. Infrastructure Layer (`AeroLux.Infrastructure`)
**External concerns and persistence implementation**

#### Persistence
- **AeroLuxDbContext**: EF Core DbContext with entity configurations
- **UnitOfWork**: Implements IUnitOfWork with domain event dispatching
- **Repository<T>**: Generic repository implementation

#### Key Features
- In-Memory database for demonstration
- Automatic domain event dispatching on save
- Entity configuration with EF Core Fluent API
- Transaction support

### 4. API Layer (`AeroLux.API`)
**HTTP endpoints and application configuration**

#### Controllers
- **BookingsController**: Booking management endpoints
- **FlightsController**: Flight management endpoints

#### Configuration
- Dependency injection setup
- MediatR registration
- CORS configuration
- Health checks
- OpenAPI/Swagger documentation

## Design Patterns

### 1. Clean Architecture
**Dependency Rule**: Dependencies flow inward toward the domain
```
API → Infrastructure → Application → Domain
```
- Domain has no dependencies
- Application depends only on Domain
- Infrastructure depends on Application and Domain
- API depends on all layers

### 2. Domain-Driven Design (DDD)

#### Aggregates
Each aggregate is a cluster of domain objects that can be treated as a single unit:
- **Aircraft Aggregate**: Self-contained aircraft management
- **Booking Aggregate**: Encapsulates booking lifecycle
- **Flight Aggregate**: Manages flight operations
- **Customer Aggregate**: Customer information management

#### Ubiquitous Language
Domain model reflects the language used by domain experts:
- Flight, Booking, Aircraft, Customer
- Scheduled, Confirmed, Cancelled, Completed
- Charter, VIP, Passenger Capacity

### 3. CQRS (Command Query Responsibility Segregation)

#### Commands
- Modify state
- Return IDs or success indicators
- Execute business logic
- Generate domain events

#### Queries
- Read-only operations
- Return DTOs
- No state changes
- Optimized for reading

### 4. Event-Driven Architecture

#### Event Flow
1. Entity state changes
2. Domain event added to entity
3. UnitOfWork.SaveChanges called
4. Events dispatched via MediatR
5. Event handlers execute
6. Entity events cleared

#### Benefits
- Loose coupling between aggregates
- Audit trail of changes
- Integration with external systems
- Asynchronous processing support

### 5. Repository Pattern

#### Benefits
- Abstracts data access
- Enables unit testing
- Consistent API for all entities
- Swappable implementations

#### Implementation
```csharp
public interface IRepository<T> where T : Entity
{
    Task<T?> GetByIdAsync(Guid id);
    Task<IEnumerable<T>> GetAllAsync();
    Task<T> AddAsync(T entity);
    Task UpdateAsync(T entity);
    Task DeleteAsync(T entity);
}
```

### 6. Unit of Work Pattern

#### Responsibilities
- Transaction management
- Change tracking
- Domain event dispatching
- Ensures consistency

### 7. Saga Pattern

#### BookingSaga Workflow
```
1. ValidateBooking
   ↓
2. ReserveAircraft
   ↓
3. ProcessPayment
   ↓
4. ConfirmBooking
   
On Failure:
   ← Refund Payment
   ← Release Aircraft
   ← Rollback Booking
```

## Event-Driven Communication

### Domain Events
Domain events enable decoupled communication between aggregates:

```csharp
// Event Definition
public record FlightScheduledEvent(
    Guid FlightId,
    Guid AircraftId,
    string DepartureAirport,
    string ArrivalAirport,
    DateTime ScheduledDepartureTime) : IDomainEvent;

// Event Publishing (in aggregate)
AddDomainEvent(new FlightScheduledEvent(...));

// Event Handling
public class FlightScheduledEventHandler : INotificationHandler<FlightScheduledEvent>
{
    public Task Handle(FlightScheduledEvent notification, CancellationToken cancellationToken)
    {
        // React to flight scheduled
        // e.g., send notifications, update external systems
    }
}
```

## Data Flow

### Command Flow (Write)
```
Controller → MediatR → CommandHandler → Repository → UnitOfWork → Database
                                                         ↓
                                              Domain Event Dispatcher
                                                         ↓
                                                  Event Handlers
```

### Query Flow (Read)
```
Controller → MediatR → QueryHandler → Repository → Database
                                           ↓
                                         DTO
```

## Security Considerations

### Implemented
- Input validation in commands
- Domain model invariants enforced
- No SQL injection (EF Core parameterized queries)
- No hardcoded secrets
- Health check endpoint

### Future Enhancements
- Authentication (JWT/OAuth)
- Authorization (role-based)
- Rate limiting
- API key management
- Audit logging
- Encryption at rest

## Scalability

### Current Architecture Supports
- **Horizontal Scaling**: Stateless API instances
- **Database Scaling**: Can switch to SQL Server/PostgreSQL
- **Message Queues**: Event handlers can be moved to message broker
- **Microservices**: Clean boundaries enable service extraction

### Future Scaling Options
- Event Store for event sourcing
- Read replicas for queries
- Message broker (RabbitMQ/Kafka) for async events
- Cache layer (Redis) for frequently accessed data
- API Gateway for routing and aggregation

## Testing Strategy

### Unit Tests
- Domain entities and value objects
- Command and query handlers
- Event handlers
- Saga orchestration

### Integration Tests
- API endpoints
- Database operations
- Event dispatching
- Transaction management

### Architecture Tests
- Dependency rules enforcement
- Naming conventions
- Layer isolation

## Deployment

### Docker
```bash
docker-compose up --build
```

### Manual
```bash
dotnet build
cd AeroLux.API
dotnet run
```

## Monitoring and Observability

### Current
- Health check endpoint (`/health`)
- Structured logging
- API documentation (`/openapi/v1.json`)

### Future
- Application Insights
- Distributed tracing (OpenTelemetry)
- Metrics (Prometheus)
- Log aggregation (ELK stack)

## Dependencies

### Core
- .NET 10
- MediatR 14.0.0
- Entity Framework Core 10.0.1

### Benefits
- Latest features and performance
- Long-term support
- Active community
- No known vulnerabilities

## Summary

AeroLux demonstrates a modern, scalable, and maintainable architecture suitable for enterprise applications. The combination of Clean Architecture, DDD, CQRS, and event-driven patterns provides:

- **Maintainability**: Clear separation of concerns
- **Testability**: Isolated layers with dependency injection
- **Scalability**: Event-driven and stateless design
- **Flexibility**: Easy to swap implementations
- **Domain Focus**: Business logic at the center
- **Event Tracing**: Complete audit trail of operations
