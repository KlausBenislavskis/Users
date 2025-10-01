# User Management API

A .NET 9 Web API for managing users and profiles with PostgreSQL. Implements Clean Architecture, CQRS, and Domain-Driven Design patterns.

## Features

- RESTful API for user and profile management
- Clean Architecture with strict dependency inversion
- CQRS pattern using Wolverine message bus
- Domain-Driven Design with aggregates, entities, and value objects
- Two-layer validation (domain invariants + application rules)
- Event publishing architecture (stub implementation)
- EF Core with PostgreSQL
- Structured console logging
- Docker support for development and deployment

## Overview

This service demonstrates architectural patterns suitable for maintainable, testable enterprise applications. It showcases Clean Architecture, CQRS, and DDD principles applied to a real-world user management scenario.

**Core functionality:**
- Create users with profile information
- Retrieve user details by ID
- Validate input using FluentValidation and domain rules
- Publish domain events (currently logged to console)

**Architecture patterns:**
- Clean Architecture with dependency inversion
- CQRS for separating read and write operations
- Domain-Driven Design with entities and value objects
- Feature-based folder structure (Screaming Architecture)

## Project Structure

```
Users/
├── Api/                      # Shared contracts (DTOs, Events)
│   └── Models/               # UserDto, ProfileDto
│   └── Events/               # UserCreatedEvent
├── Api/Server/               # Web API layer
│   └── Controllers/          # REST endpoints
├── Domain/                   # Core business logic
│   └── Users/                # User aggregate, entities, value objects
├── Application/              # Use cases
│   └── Users/Commands/       # CreateUser command
│   └── Users/Queries/        # GetUserById query
├── Infrastructure/           # External concerns
│   └── Persistence/          # EF Core, repositories
│   └── Messaging/            # Event publishing (stub)
└── docker-compose.yml        # PostgreSQL container
```

## Quick Start

### Prerequisites

- .NET 9 SDK: https://dotnet.microsoft.com/download/dotnet/9.0
- Docker Desktop: https://www.docker.com/products/docker-desktop/

### Option 1: Local Development (Recommended)

Run PostgreSQL in Docker, run the API locally for fast iteration and debugging.

1. **Start PostgreSQL**
   ```bash
   docker compose up -d postgres
   ```

2. **Run the API**
   ```bash
   cd Api/Server
   dotnet run
   ```

3. **Access the API**
   - Swagger UI: `http://localhost:5172/swagger`
   - HTTP endpoint: `http://localhost:5172`
   - HTTPS endpoint: `https://localhost:7239`

4. **Stop PostgreSQL when done**
   ```bash
   docker compose down
   ```

### Option 2: Full Docker Deployment

Run both PostgreSQL and the API in containers.

1. **Start all services**
   ```bash
   docker compose up --build
   ```

2. **Access the API**
   - Swagger UI: `http://localhost:5000/swagger`
   - HTTP endpoint: `http://localhost:5000`
   - HTTPS endpoint: `https://localhost:5001`

3. **View logs**
   ```bash
   docker compose logs -f api
   ```

4. **Stop all services**
   ```bash
   docker compose down
   ```

**Note:** EF Core migrations run automatically on startup in both scenarios.

## API Endpoints

### Create User

**Request:**
```http
POST /api/users
Content-Type: application/json

{
  "username": "klavsb",
  "email": "klavs@example.com",
  "firstName": "Klavs",
  "lastName": "Benislavskis",
  "dateOfBirth": "2001-09-26T00:00:00Z"
}
```

**Response (201 Created):**
```json
{
  "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6"
}
```

**Example (Local):**
```bash
curl -X POST http://localhost:5172/api/users \
  -H "Content-Type: application/json" \
  -d @test-create-user.json
```

**Example (Docker):**
```bash
curl -X POST http://localhost:5000/api/users \
  -H "Content-Type: application/json" \
  -d @test-create-user.json
```

### Get User by ID

**Request:**
```http
GET /api/users/{id}
```

**Response (200 OK):**
```json
{
  "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "username": "klavsb",
  "email": "klavs@example.com",
  "profile": {
    "firstName": "Klavs",
    "lastName": "Benislavskis",
    "dateOfBirth": "2001-09-26T00:00:00Z"
  }
}
```

**Example (Local):**
```bash
curl http://localhost:5172/api/users/{id}
```

**Example (Docker):**
```bash
curl http://localhost:5000/api/users/{id}
```

## Architecture

### Clean Architecture Layers

The application follows dependency inversion: outer layers depend on inner layers, never the reverse.

**Domain (Core)**
- Contains business entities, value objects, and domain logic
- No dependencies on other layers
- Example: `User`, `Profile`, `UserEmail` value object

**Application (Use Cases)**
- Implements CQRS commands and queries
- Depends only on Domain layer
- Example: `CreateUserCommandHandler`, `GetUserByIdQueryHandler`

**Infrastructure (External)**
- Implements interfaces defined in Application
- Database access with EF Core and PostgreSQL
- Message publishing (currently stubbed)

**Api/Server (Presentation)**
- REST API controllers
- Depends on Application layer through Wolverine message bus
- Maps HTTP requests to commands/queries

### CQRS Pattern

Commands and queries are separated:

**Commands** modify state:
- `CreateUserCommand` → creates a user, returns ID

**Queries** read state:
- `GetUserByIdQuery` → retrieves user data

Wolverine acts as the message bus, routing commands/queries to their handlers.

### Domain-Driven Design

**Entities:**
- `User` - Aggregate root
- `Profile` - Entity owned by User

**Value Objects:**
- `UserEmail` - Encapsulates email validation

**Repositories:**
- `IUserRepository` - Interface in Application, implemented in Infrastructure

**Factory Methods:**
- `User.Create()` - Enforces business rules during construction

### Validation Strategy

**Two-layer validation:**

1. **Domain validation** - Enforces invariants (rules that must always be true)
   - Example: Email format, username minimum length
   - Protects domain integrity regardless of how objects are created

2. **Application validation** - Enforces business rules for specific operations
   - Example: Username uniqueness, date range checks
   - Provides detailed error messages for API responses
   - Uses FluentValidation for declarative rules

Both layers work together: application validation runs first, then domain validation ensures the object state is always valid.

## Technology Stack

### Wolverine (Message Handling)

Used instead of MediatR for CQRS implementation.

**Why Wolverine:**
- Convention-based handlers reduce boilerplate
- Source-generated code (no reflection at runtime)
- Built-in FluentValidation support
- MIT license

**How it works:**
```csharp
// Handler is a static method - Wolverine discovers it
public static async Task<Result<Guid>> Handle(
    CreateUserCommand command,
    IUserRepository repository,
    CancellationToken cancellationToken)
{
    // Dependencies injected as parameters
}
```

### Mapperly (Object Mapping)

Used instead of AutoMapper for entity-to-DTO mapping.

**Why Mapperly:**
- Compile-time code generation (errors caught during build)
- No runtime reflection
- Type-safe mappings

**How it works:**
```csharp
[Mapper]
public partial class UserMapper
{
    public partial UserDto MapToDto(User user);

    // Custom mapping for value objects
    private string MapEmail(UserEmail email) => email.Value;
}
```

### FluentValidation

Declarative validation rules for commands and queries.

**Example:**
```csharp
public class CreateUserCommandValidator : AbstractValidator<CreateUserCommand>
{
    public CreateUserCommandValidator()
    {
        RuleFor(x => x.Username).NotEmpty().MinimumLength(3);
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
    }
}
```

### Shared Contracts (Api Project)

The `Api/` project contains DTOs and events that other services can reference.

**Why:**
- Enables type-safe integration between microservices
- Other services can deserialize events without duplicating models
- Supports event-driven architecture

**Example:**
```csharp
// Another service can reference Users.Api and listen to this event
public record UserCreatedEvent
{
    public Guid UserId { get; init; }
    public string Username { get; init; }
    public string EventType => nameof(UserCreatedEvent);
}
```

### Message Publisher (Stub)

The `IMessagePublisher` currently logs events to the console. In production, replace with a real message broker.

**Current implementation:**
```csharp
public Task PublishUserCreatedAsync(User user)
{
    var @event = new UserCreatedEvent { /* ... */ };
    _logger.LogInformation("Publishing {EventType} message: {Message}",
        @event.EventType, JsonSerializer.Serialize(@event));
    return Task.CompletedTask;
}
```

**Production replacement:**
- RabbitMQ
- Azure Service Bus
- AWS SNS/SQS
- Kafka

## Development

### Database Migrations

**Create a new migration:**
```bash
cd Api/Server
dotnet ef migrations add MigrationName --project ../../Infrastructure/Users.Infrastructure.csproj
```

**Apply migrations manually:**
```bash
dotnet ef database update --project ../../Infrastructure/Users.Infrastructure.csproj
```

Migrations run automatically on application startup (configured in `Program.cs`).

### Connection Strings

**Local development** (`appsettings.Development.json`):
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=homeworkdb;Username=taskuser;Password=mypassword"
  }
}
```

**Docker** (via environment variables in `docker-compose.yml`):
```yaml
environment:
  - ConnectionStrings__DefaultConnection=Host=postgres;Port=5432;Database=homeworkdb;Username=taskuser;Password=mypassword
```

**Security Note:** The connection string contains a default password for development. In production, use environment variables or secure secret management (Azure Key Vault, AWS Secrets Manager, etc.).

### Logging

Logs output to console with structured logging format.

**Configuration** (`appsettings.Development.json`):
```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.EntityFrameworkCore": "Information",
      "Users": "Debug"
    }
  }
}
```

**Example log output:**
```
2025-10-01 14:23:45 info: Creating new user: klavsb
2025-10-01 14:23:45 info: Executed DbCommand (INSERT INTO "Users"...)
2025-10-01 14:23:45 info: Publishing UserCreatedEvent message to queue: {...}
```

### Testing

**Swagger UI (Recommended):**
- Navigate to `http://localhost:5172/swagger` when running locally
- Interactive API documentation with "Try it out" functionality

**VS Code REST Client:**
- Install the "REST Client" extension by Huachao Mao
- Open `api-tests.http` in the project root
- Click "Send Request" above each test case
- Includes validation tests and variable chaining

**curl with test file:**
```bash
curl -X POST http://localhost:5172/api/users \
  -H "Content-Type: application/json" \
  -d @test-create-user.json
```

**Test files included:**
- `test-create-user.json` - Sample user data for curl
- `api-tests.http` - Full test suite for VS Code REST Client

## Dependencies

| Package | Purpose |
|---------|---------|
| WolverineFx | CQRS message handling |
| Riok.Mapperly | Object mapping |
| FluentValidation | Validation rules |
| Microsoft.EntityFrameworkCore | ORM |
| Npgsql.EntityFrameworkCore.PostgreSQL | PostgreSQL provider |

## License

This project is for demonstration and educational purposes.

## Acknowledgments

This project demonstrates Clean Architecture and DDD principles following guidance from:
- Robert C. Martin's Clean Architecture
- Eric Evans' Domain-Driven Design
- Jimmy Bogard's architectural patterns
