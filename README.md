# Employee Management System API

ASP.NET Core Web API application for managing employee data.

This project is built using **Clean Architecture principles** with separation of responsibilities between Domain, Application, Infrastructure, and API layers.

---

# Technologies

- .NET 8
- ASP.NET Core Web API
- Entity Framework Core
- SQL Server
- Docker
- Swagger / OpenAPI
- xUnit
- AutoMapper
- Dependency Injection
- Repository Pattern

---

# Solution Structure

The solution is organized into the following projects:

```
Employee
│
├── Employee
│   └── API Layer
│       ├── Controllers
│       ├── Middleware
│       ├── Authentication
│       └── Configuration
│
├── Employee.Core
│   └── Application and Domain Layer
│       ├── Entities
│       ├── DTOs
│       ├── Interfaces
│       ├── Business Logic
│       └── Validation
│
├── Employee.Infrastructure
│   └── Infrastructure Layer
│       ├── Repositories
│       ├── External Services
│       └── Implementations
│
├── Employee.Data
│   └── Data Access Layer
│       ├── DbContext
│       ├── Entity Configurations
│       └── Database Setup
│
└── Employee.Tests
    └── Unit Tests
```

---

# Architecture Overview

The project follows Clean Architecture:

```
          API
           |
           |
      Application/Core
           |
           |
    Infrastructure
           |
           |
       Database
```

## Dependency Direction

Dependencies flow inward:

```
API
 |
Infrastructure
 |
Core
```

The Core layer does not depend on external frameworks or databases.

---

# Prerequisites

Install the following:

## .NET SDK

Check installation:

```bash
dotnet --version
```

Required version:

```
.NET 8 SDK
```

## SQL Server

The application uses SQL Server as the database.

## Docker (optional)

Docker Desktop is required if running containers.

---

# Database Configuration

Update the connection string in:

```
Employee/appsettings.json
```

Example:

```json
{
  "ConnectionStrings": {
    "DatabaseConnectionString": "Server=localhost;Database=EmployeeDb;Trusted_Connection=True;TrustServerCertificate=True"
  }
}
```

---

# Entity Framework Core Migrations

Install EF Core tools if not installed:

```bash
dotnet tool install --global dotnet-ef
```

Create a migration:

```bash
dotnet ef migrations add InitialCreate
```

Apply migration to database:

```bash
dotnet ef database update
```

---

# Running the Application

## Using Visual Studio

1. Open:

```
Employee.sln
```

2. Set:

```
Employee
```

as the startup project.

3. Press:

```
F5
```

Swagger will open automatically.

---

## Using .NET CLI

Run:

```bash
dotnet run --project Employee
```

API will start on:

```
https://localhost:<port>
```

Swagger:

```
https://localhost:<port>/swagger
```

---

# Running with Docker

Build containers:

```bash
docker compose build
```

Start containers:

```bash
docker compose up
```

Stop containers:

```bash
docker compose down
```

The solution contains:

- API container
- SQL Server container

---

# Running Tests

Execute unit tests:

```bash
dotnet test
```

Test project:

```
Employee.Tests
```

---

# Features

Implemented:

- Employee CRUD operations
- Clean Architecture structure
- Repository Pattern
- Dependency Injection
- Entity Framework Core
- DTO mapping
- Validation
- Unit Testing
- Docker support
- Swagger documentation

---

# Author

Employee Management System API

Built with ASP.NET Core and Clean Architecture.
