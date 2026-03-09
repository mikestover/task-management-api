using Microsoft.EntityFrameworkCore;
using TaskManagementApi.Data;
using FluentValidation;
using TaskManagementApi.Filters;

var builder = WebApplication.CreateBuilder(args);

// Bind to Railway's PORT environment variable
var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";
builder.WebHost.UseUrls($"http://0.0.0.0:{port}");

// Get connection string from Railway or fallback to appsettings.json
var connectionString = Environment.GetEnvironmentVariable("DATABASE_URL");
if (!string.IsNullOrEmpty(connectionString))
{
    // Parse Railway's DATABASE_URL format
    var databaseUri = new Uri(connectionString);
    var userInfo = databaseUri.UserInfo.Split(':');

    var sslMode = Environment.GetEnvironmentVariable("RAILWAY_ENVIRONMENT") != null
        ? ";SSL Mode=Require;Trust Server Certificate=true"
        : "";
    connectionString = $"Host={databaseUri.Host};Port={databaseUri.Port};Database={databaseUri.LocalPath.TrimStart('/')};Username={userInfo[0]};Password={userInfo[1]}{sslMode}";
}
else
{
    connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
}

// Add services to the container.
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(connectionString));

// Add controllers with custom validation filter
builder.Services.AddControllers(options =>
{
    options.Filters.Add<ValidationFilter>();
});

// Add FluentValidation
builder.Services.AddValidatorsFromAssemblyContaining<Program>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        builder =>
        {
            builder.AllowAnyOrigin()
                   .AllowAnyMethod()
                   .AllowAnyHeader();
        });
});

// Add Health Checks
builder.Services.AddHealthChecks()
    .AddNpgSql(connectionString ?? "", name: "database");

var app = builder.Build();

// Use CORS
app.UseCors("AllowAll");

// Map Health Check endpoints
app.MapHealthChecks("/health");
app.MapHealthChecks("/health/ready", new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions
{
    Predicate = check => check.Tags.Contains("ready")
});

// Auto-migrate database on startup
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    db.Database.Migrate();
}

// Enable Swagger in all environments
app.UseSwagger();
app.UseSwaggerUI();

app.UseAuthorization();

// Redirect root to Swagger
app.MapGet("/", () => Results.Redirect("/swagger")).ExcludeFromDescription();

app.MapControllers();

app.Run();