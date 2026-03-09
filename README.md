# Task Management API

A production-ready RESTful API for task management, built with ASP.NET Core and PostgreSQL. Designed for one-click Railway deployment.

[![Deploy on Railway](https://railway.app/button.svg)](https://railway.app/template/your-template-id)

## Features

- Full CRUD for tasks with validation (FluentValidation)
- Filtering, sorting, search, and pagination
- Priority levels (Low, Medium, High)
- Auto-migration on startup
- Health check endpoints (`/health`, `/health/ready`)
- Swagger UI in all environments
- Docker Compose for local development

## Tech Stack

- ASP.NET Core 9.0
- PostgreSQL + Entity Framework Core
- FluentValidation
- Swagger/OpenAPI

## Deploy to Railway

1. Click the **Deploy on Railway** button above
2. Railway will provision a PostgreSQL database and deploy the API automatically
3. Visit your app URL + `/swagger` to explore the API

## Local Development

### With Docker (recommended)

```bash
docker compose up --build
```

API available at `http://localhost:8080/swagger`

### Without Docker

Prerequisites: .NET 9.0 SDK, PostgreSQL running locally

```bash
cd TaskManagementApi
dotnet run
```

Update `appsettings.json` with your local connection string if needed.

## API Endpoints

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/tasks` | List tasks (with filtering, sorting, pagination) |
| GET | `/api/tasks/{id}` | Get task by ID |
| POST | `/api/tasks` | Create task |
| PUT | `/api/tasks/{id}` | Update task |
| DELETE | `/api/tasks/{id}` | Delete task |
| GET | `/health` | Health check |

### Query Parameters (GET /api/tasks)

| Parameter | Type | Description |
|-----------|------|-------------|
| `pageNumber` | int | Page number (default: 1) |
| `pageSize` | int | Items per page (default: 10) |
| `isCompleted` | bool? | Filter by completion status |
| `priority` | enum? | Filter by priority (Low, Medium, High) |
| `dueDateFrom` | datetime? | Filter tasks due after this date |
| `dueDateTo` | datetime? | Filter tasks due before this date |
| `sortBy` | string? | Sort field (title, priority, duedate, iscompleted, updatedat, createdat) |
| `sortDescending` | bool | Sort direction (default: true) |
| `searchTerm` | string? | Search in title and description |

### Example: Create a Task

```json
POST /api/tasks
{
  "title": "Learn Railway deployment",
  "description": "Deploy .NET API to Railway",
  "priority": 1,
  "dueDate": "2026-12-31T00:00:00Z"
}
```

## Configuration

Railway automatically sets `DATABASE_URL`. No manual configuration needed.

For local development, the `docker-compose.yml` handles everything. For bare-metal, edit `appsettings.json`:

```json
"ConnectionStrings": {
  "DefaultConnection": "Host=localhost;Database=taskmanagementdb;Username=postgres;Password=postgres"
}
```
