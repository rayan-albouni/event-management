# Event Management System

A RESTful ASP.NET Core Web API application built with .NET 9.0 that manages events and registrations following Domain-Driven Design (DDD) principles and SOLID design patterns.

## Architecture

The application follows a clean architecture with the following layers:

- **EventManagement.Api**: Presentation layer with controllers and API endpoints
- **EventManagement.Application**: Application layer with business logic and DTOs
- **EventManagement.Infrastructure**: Infrastructure layer with data access and external services
- **EventManagement.Domain**: Domain layer with entities, value objects, and business rules

## Getting Started

### Prerequisites
- .NET 9.0 SDK
- Any IDE that supports .NET development (Visual Studio, VS Code, Rider)

### Running the Application
1. Clone the repository
2. Navigate to the solution directory
3. Run the application:
   ```bash
   dotnet run --project EventManagement.Api
   ```
4. The API will be available at `http://localhost:5131`
5. Swagger documentation is available at `http://localhost:5131/swagger`
6. SQLite database file (`EventManagement.db`) will be created automatically in the project root

### Running Tests
Tests can be run using the .NET CLI:
```bash
dotnet test
```

## API Endpoints

### Authentication
- `POST /api/v1/auth/register`
- `POST /api/v1/auth/login`
- `GET /api/v1/auth/profile`

### Events
- `GET /api/v1/events`
- `GET /api/v1/events/{id}`
- `POST /api/v1/events`
- `PUT /api/v1/events/{id}`
- `DELETE /api/v1/events/{id}`

### Registrations
- `POST /api/v1/events/{eventId}/registrations`
- `GET /api/v1/events/{eventId}/registrations/{eventId}`
- `DELETE /api/v1/events/{eventId}/registrations/{registrationId}`

## Authentication

The API uses JWT Bearer tokens for authentication. After logging in, include the token in the Authorization header:

```
Authorization: Bearer <your-jwt-token>
```

## Role-Based Access Control (RBAC)

The application implements a comprehensive permission-based authorization system:

### User Roles
- **EventCreator**: Can create, update, delete events and manage registrations
- **EventParticipant**: Can view events and register for them

### Permissions
- **CreateEvent**: Create new events
- **ReadEvent**: View event details
- **UpdateEvent**: Modify existing events
- **DeleteEvent**: Remove events
- **CreateRegistration**: Register for events
- **ReadRegistration**: View registration details
- **DeleteRegistration**: Remove registrations
- **ReadEventRegistrations**: View all registrations for an event

### Permission Mapping
- **EventCreator**: All permissions
- **EventParticipant**: CreateRegistration, ReadEvent, ReadRegistration

