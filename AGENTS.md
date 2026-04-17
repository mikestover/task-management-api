# AI Agent Instructions - Task Management API

## Quick Start

**Project Type**: ASP.NET Core 9.0 RESTful API for task management  
**Database**: PostgreSQL + Entity Framework Core  
**Main File**: [Program.cs](TaskManagementApi/Program.cs) (entry point and dependency injection)

### Build & Run

```bash
# Using Docker Compose (recommended for local dev)
docker compose up --build

# Using .NET CLI
cd TaskManagementApi
dotnet run
```

**API Endpoint**: `http://localhost:8080/swagger` (Swagger UI available in all environments)

## Project Architecture

See [README.md](README.md) for full feature list and API endpoint documentation.

### Directory Structure

```
TaskManagementApi/
├── Controllers/          # RESTful endpoint handlers
├── Models/              # Domain entities and enums
├── DTOs/                # Data transfer objects (CreateTaskDto, UpdateTaskDto, TaskResponseDto)
├── Validators/          # FluentValidation validators (CreateTaskDtoValidator, UpdateTaskDtoValidator)
├── Filters/             # Custom action filters (ValidationFilter)
├── Data/                # Entity Framework DbContext and configuration
└── Migrations/          # EF Core database migrations
```

## Core Conventions

### 1. Validation Pattern
- **Validators**: Located in `Validators/` folder, inherit from `AbstractValidator<T>`
- **FluentValidation**: Used for declarative validation rules
- **Custom Filter**: `ValidationFilter` converts ModelState errors to RFC 7231 Problem Details format
- **DTO Validation**: Both data annotations (`[Required]`, `[StringLength]`) and FluentValidation rules are used
- **Example**: See [CreateTaskDtoValidator.cs](TaskManagementApi/Validators/CreateTaskDtoValidator.cs)

```csharp
public class CreateTaskDtoValidator : AbstractValidator<CreateTaskDto>
{
    public CreateTaskDtoValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Title is required")
            .MaximumLength(200).WithMessage("Title cannot exceed 200 characters");
        // ...
    }
}
```

### 2. DTO Pattern
- **Create DTOs**: Accept input, may have fewer/different fields than response
- **Response DTOs**: Return domain data without sensitive fields
- **Update DTOs**: Similar to Create DTOs, fields match creation requirements
- **Example**: [CreateTaskDto.cs](TaskManagementApi/Dtos/CreateTaskDto.cs) vs [TaskResponseDto.cs](TaskManagementApi/Dtos/TaskResponseDto.cs)

### 3. Entity Framework Patterns
- **DbContext**: [ApplicationDbContext.cs](TaskManagementApi/Data/ApplicationDbContext.cs) - defines DbSet<T> for entities
- **Auto-Migrations**: Database.Migrate() runs on app startup in [Program.cs](TaskManagementApi/Program.cs) line 68
- **Migrations**: Located in `Migrations/` folder, auto-numbered with timestamps

### 4. Query Filtering & Pagination
- **Query Parameters**: Defined in [TaskQueryParameters.cs](TaskManagementApi/Models/TaskQueryParameters.cs)
- **Response Format**: Wrapped in [PagedResult<T>](TaskManagementApi/Models/PagedResult.cs) with Items, PageNumber, PageSize, TotalCount
- **Sorting**: Uses `sortBy` field (case-insensitive: title, priority, duedate, iscompleted, updatedat, createdat)
- **Search**: Case-insensitive text search across Title and Description

### 5. Controller Pattern
- **Naming**: Plural (TasksController not TaskController)
- **Routing**: `[Route("api/[controller]")]` → `/api/tasks`
- **Dependency Injection**: DbContext injected via constructor
- **Response Format**: `ActionResult<T>` with proper status codes (Ok, NotFound, BadRequest, etc.)
- **Example**: [TasksController.cs](TaskManagementApi/Controllers/TaskController.cs)

### 6. Model Structure
- **Timestamp Fields**: All entities have `CreatedAt` and `UpdatedAt` (DateTime)
- **Soft Delete**: Not currently implemented - deletions are hard deletes
- **Enums**: Priority is an enum (Low=0, Medium=1, High=2)
- **Required Fields**: Use C# 11 `required` keyword, not just null-coalescing
- **Example**: [TaskItem.cs](TaskManagementApi/Models/TaskItem.cs)

## Common Tasks for AI Agents

### Adding a New API Endpoint
1. Add method to [TasksController.cs](TaskManagementApi/Controllers/TaskController.cs)
2. Use async/await with `Task<ActionResult<T>>`
3. Create/use appropriate DTOs in `Dtos/`
4. Add validator if it's a POST/PUT endpoint (inherit from `AbstractValidator<YourDto>`)
5. Return appropriate status codes

### Adding Database Fields
1. Add property to [TaskItem.cs](TaskManagementApi/Models/TaskItem.cs) model
2. Create new migration: `dotnet ef migrations add DescriptiveName`
3. The migration runs automatically on startup via `db.Database.Migrate()`

### Handling Query Parameters
1. Add to [TaskQueryParameters.cs](TaskManagementApi/Models/TaskQueryParameters.cs)
2. Use in controller: `[FromQuery] TaskQueryParameters queryParams`
3. Apply filters to `IQueryable<T>` before `ToListAsync()`
4. Always apply sorting and pagination last

## Deployment Context

### Railway Configuration
- **Environment Variable**: `DATABASE_URL` is parsed from Railway's PostgreSQL connection string format
- **Port**: Reads from `PORT` environment variable (default 8080)
- **SSL Mode**: Automatically set to require SSL in Railway environments
- **Auto-Migration**: Database migrations run on every app startup via [Program.cs](TaskManagementApi/Program.cs)
- **See**: [railway.toml](railway.toml) and connection string parsing in [Program.cs](TaskManagementApi/Program.cs) lines 9-23

### Health Checks
- **Endpoint**: `/health` - basic liveness
- **Endpoint**: `/health/ready` - checks database connectivity (PostgreSQL)
- **Used by**: Railway's health monitoring and load balancer

## Important Notes

### Do's
✅ Use async/await for all database operations  
✅ Apply FluentValidation validators for all input DTOs  
✅ Include proper error handling and status codes  
✅ Use Entity Framework's `Where()` for filtering (LINQ to Entities)  
✅ Apply pagination to avoid loading all records  
✅ Use DTOs to separate API contract from domain model  

### Don'ts
❌ Don't hardcode connection strings - use configuration  
❌ Don't apply `ToList()` before filtering/sorting in EF queries  
❌ Don't mix domain entities with API DTOs  
❌ Don't forget to add validators when adding POST/PUT endpoints  
❌ Don't modify auto-migrations manually - recreate them instead  
❌ Don't use synchronous database operations (sync over async)  

## Dependencies & Versions

| Package | Version | Purpose |
|---------|---------|---------|
| Microsoft.EntityFrameworkCore | 9.0.0 | ORM for PostgreSQL |
| Npgsql.EntityFrameworkCore.PostgreSQL | 9.0.4 | PostgreSQL provider |
| FluentValidation | 12.1.0 | Validation framework |
| Swashbuckle.AspNetCore | 6.9.0 | Swagger/OpenAPI |
| AspNetCore.HealthChecks.NpgSql | 9.0.0 | Database health checks |

### Nullable Reference Types
Project has `<Nullable>enable</Nullable>` enabled - use null-forgiving operator `!` or `required` keyword appropriately.

## Testing API Locally

Use [TaskManagementApi.http](TaskManagementApi/TaskManagementApi.http) file with REST Client extension or Postman:

```http
GET http://localhost:8080/api/tasks?pageNumber=1&pageSize=10 HTTP/1.1

POST http://localhost:8080/api/tasks HTTP/1.1
Content-Type: application/json

{
  "title": "Sample Task",
  "description": "Description",
  "priority": 1,
  "dueDate": "2026-12-31T00:00:00Z"
}
```

---

**Last Updated**: April 2026  
For detailed API endpoints, parameters, and examples, see [README.md](README.md)
