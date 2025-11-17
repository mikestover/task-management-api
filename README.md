# Task Management API

A RESTful API built with ASP.NET Core for managing tasks.

## Features

- Create, read, update, and delete tasks
- Task properties: Title, Description, Priority (Low/Medium/High), Due Date, Completion Status
- PostgreSQL database with Entity Framework Core
- Swagger/OpenAPI documentation
- Auto-migration on startup

## Local Development

### Prerequisites

- .NET 9.0 SDK
- PostgreSQL

### Setup

1. Clone the repository
2. Update the connection string in `appsettings.json` if needed
3. Run the application:
```bash
dotnet run
```

4. Navigate to `http://localhost:5088/swagger` to view the API documentation

## Deploy to Railway

### Prerequisites

- Railway account (https://railway.app)
- Git repository

### Deployment Steps

1. **Create a new project on Railway**

2. **Add PostgreSQL database**
   - Click "New" → "Database" → "Add PostgreSQL"

3. **Deploy the API**
   - Click "New" → "GitHub Repo" (or "Empty Service" for manual deploy)
   - Select your repository
   - Railway will automatically detect it's a .NET project

4. **Configure Environment Variables** (Optional)
   - Railway automatically provides `DATABASE_URL`
   - The app will automatically use it

5. **Deploy**
   - Railway will build and deploy your app
   - Migrations run automatically on startup

6. **Access your API**
   - Railway will provide a public URL
   - Add `/swagger` to see the API documentation

## API Endpoints

- `GET /api/tasks` - Get all tasks
- `GET /api/tasks/{id}` - Get a specific task
- `POST /api/tasks` - Create a new task
- `PUT /api/tasks/{id}` - Update a task
- `DELETE /api/tasks/{id}` - Delete a task

## Tech Stack

- ASP.NET Core 9.0
- Entity Framework Core
- PostgreSQL
- Swagger/OpenAPI