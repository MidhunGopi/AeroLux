# AeroLux

AeroLux is an enterprise-grade, event-driven backend platform designed to power a private jet charter booking and aviation operations system.

## 🌟 Key Capabilities

- **Multi-role user management** (Passengers, Pilots, Flight Attendants, Flight Control Officers, Operations, Finance, Compliance)
- **Private jet booking** with multi-leg flights and aircraft constraints
- **Aircraft fleet & maintenance management**
- **Crew scheduling** with certification and duty-time rules
- **Safety-critical flight lifecycle orchestration**
- **Regulatory clearance and go/no-go approvals**
- **Real-time flight status & telemetry updates**
- **Event-driven billing, escrow, and settlement**
- **Centralized audit & compliance tracking**

## 🏗️ Microservices Architecture

The system is decomposed into independently deployable microservices:

| Service | Description | Port |
|---------|-------------|------|
| **Identity & Access Service** | Multi-tenant RBAC/ABAC, JWT authentication, audit-ready authorization | 5101 |
| **Booking & Charter Service** | Flight search, booking, pricing, and cancellations | 5102 |
| **Aircraft Fleet Service** | Aircraft availability, maintenance windows, airworthiness checks | 5103 |
| **Crew Management Service** | Pilot and crew scheduling, certification validation, rest-rule enforcement | 5104 |
| **Flight Operations Service** | Saga-based orchestration of the full flight lifecycle | 5105 |
| **Flight Control & Clearance Service** | Weather, airspace, regulatory clearance approvals | 5106 |
| **Real-Time Tracking Service** | Live flight status and updates using SignalR | 5107 |
| **Billing & Settlement Service** | Dynamic pricing, invoicing, escrow, refunds | 5108 |
| **Notification Service** | Asynchronous and real-time alerts for all stakeholders | 5109 |
| **Audit & Compliance Service** | Immutable audit logs and regulatory traceability | 5110 |
| **API Gateway** | YARP-based reverse proxy with JWT validation | 5000 |

## 🔁 Event-Driven Communication

All inter-service communication is asynchronous and event-based using RabbitMQ.

**Implemented patterns:**
- ✅ Saga Orchestration
- ✅ Outbox Pattern
- ✅ Idempotent Consumers
- ✅ Dead-Letter Queues
- ✅ Retry with Backoff
- ✅ Event Versioning

## 🧠 Architectural Patterns & Practices

- **Domain-Driven Design (DDD)** - Rich domain models with aggregates, entities, and value objects
- **Clean Architecture** - Clear separation of concerns across Domain, Application, Infrastructure, and API layers
- **CQRS** (Command & Query Separation) - Separate read and write models
- **Saga Pattern** - Long-running workflow orchestration for flight lifecycle
- **State Machines** - Flight lifecycle management with explicit state transitions
- **Event-Driven Architecture** - Loose coupling through domain and integration events
- **Optimistic Concurrency Control** - Conflict detection for concurrent updates
- **Distributed Systems Fault Handling** - Resilient inter-service communication

## 🔐 Security & Compliance

- JWT + Refresh Tokens
- Policy-based authorization
- Context-aware permissions (role per flight)
- Full audit trail for every critical action
- Compliance-ready data retention and traceability

## 🟢 Tech Stack

| Category | Technology |
|----------|------------|
| Backend Framework | .NET 10 Web API |
| Architecture | Clean Architecture, DDD, CQRS |
| Messaging | RabbitMQ |
| Real-Time Communication | SignalR |
| Database | PostgreSQL |
| Caching | Redis |
| Search & Logs | OpenSearch |
| API Gateway | YARP |
| Observability | OpenTelemetry |
| Logging | Serilog |
| Containers | Docker |

## 📁 Project Structure

```
AeroLux/
├── src/
│   ├── Gateway/
│   │   └── AeroLux.Gateway/          # YARP API Gateway
│   ├── Services/
│   │   ├── Identity/                 # Identity & Access Service
│   │   │   ├── AeroLux.Identity.Api
│   │   │   ├── AeroLux.Identity.Application
│   │   │   ├── AeroLux.Identity.Domain
│   │   │   └── AeroLux.Identity.Infrastructure
│   │   ├── Booking/                  # Booking & Charter Service
│   │   ├── Fleet/                    # Aircraft Fleet Service
│   │   ├── Crew/                     # Crew Management Service
│   │   ├── FlightOperations/         # Flight Operations Service
│   │   ├── FlightControl/            # Flight Control & Clearance Service
│   │   ├── Tracking/                 # Real-Time Tracking Service
│   │   ├── Billing/                  # Billing & Settlement Service
│   │   ├── Notification/             # Notification Service
│   │   └── Audit/                    # Audit & Compliance Service
│   └── Shared/
│       ├── AeroLux.Shared.Kernel/    # DDD building blocks, CQRS, Events
│       └── AeroLux.Shared.Infrastructure/  # Common infrastructure components
├── tests/
├── scripts/
│   └── init-databases.sql
├── docker-compose.yml
└── AeroLux.sln
```

## 🚀 Getting Started

### Prerequisites

- .NET 10 SDK
- Docker & Docker Compose
- PostgreSQL (or use Docker)
- RabbitMQ (or use Docker)

### Running with Docker Compose

```bash
# Start all services with infrastructure
docker-compose up -d

# View logs
docker-compose logs -f

# Stop all services
docker-compose down
```

### Running Locally

```bash
# Restore dependencies
dotnet restore

# Build the solution
dotnet build

# Run a specific service
dotnet run --project src/Services/Identity/AeroLux.Identity.Api
```

### API Endpoints

All services are accessible through the API Gateway at `http://localhost:5000`:

- `POST /api/auth/register` - Register a new user
- `POST /api/auth/login` - Authenticate and get JWT tokens
- `GET /api/auth/me` - Get current user profile
- `GET /api/bookings` - List bookings
- `POST /api/bookings` - Create a new booking
- `GET /api/aircraft` - List available aircraft
- `GET /api/crew` - List crew members
- `GET /api/flights` - List flights
- `GET /api/tracking/{flightId}` - Get real-time flight position

## 🧪 Testing

```bash
# Run all tests
dotnet test

# Run specific test project
dotnet test tests/Services/AeroLux.Identity.Tests
```

## 📊 Health Checks

Each service exposes a health check endpoint:

```bash
curl http://localhost:5000/health  # Gateway
curl http://localhost:5101/health  # Identity
curl http://localhost:5102/health  # Booking
# ... etc
```

## 🎯 Project Goals

- Demonstrate enterprise-level backend system design
- Showcase real-world aviation domain complexity
- Serve as a reference architecture for large-scale, distributed .NET systems
- Provide a solid foundation for future frontend, mobile, or third-party integrations

## 📄 License

This project is licensed under the MIT License.

