using Microsoft.EntityFrameworkCore;
using VoiceConcierge.Core.Domain.Interfaces;
using VoiceConcierge.Core.Services;
using VoiceConcierge.Infrastructure.Data;
using VoiceConcierge.Infrastructure.Data.Repositories;
using VoiceConcierge.Infrastructure.Data.Seed;
using VoiceConcierge.Infrastructure.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "Voice Concierge API", Version = "v1" });
});

// Configure PostgreSQL with pgvector
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseNpgsql(connectionString, npgsqlOptions =>
    {
        npgsqlOptions.UseVector();
    });
});

// Register repositories
builder.Services.AddScoped<IFAQRepository, FAQRepository>();
builder.Services.AddScoped<IUnansweredQuestionRepository, UnansweredQuestionRepository>();
builder.Services.AddScoped<IVoiceConfigurationRepository, VoiceConfigurationRepository>();

// Register services
builder.Services.AddScoped<IEmbeddingService, OpenAIEmbeddingService>();
builder.Services.AddScoped<IFAQService, FAQService>();
builder.Services.AddScoped<IUnansweredQuestionService, UnansweredQuestionService>();
builder.Services.AddScoped<IVoiceConfigurationService, VoiceConfigurationService>();

// Register database seeder
builder.Services.AddScoped<DatabaseSeeder>();

// Add health checks
builder.Services.AddHealthChecks();

// Configure CORS
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins("http://localhost:3000", "http://localhost:5173")
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors();

app.MapHealthChecks("/health");

// Database connection test endpoint
app.MapGet("/api/test/db-connection", async (ApplicationDbContext db) =>
{
    try
    {
        await db.Database.CanConnectAsync();
        return Results.Ok(new { message = "Database connection successful", timestamp = DateTime.UtcNow });
    }
    catch (Exception ex)
    {
        return Results.Problem($"Database connection failed: {ex.Message}");
    }
});

// Auto-apply migrations and seed data in development
if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();
    var services = scope.ServiceProvider;
    var logger = services.GetRequiredService<ILogger<Program>>();
    
    try
    {
        // Apply migrations
        var db = services.GetRequiredService<ApplicationDbContext>();
        logger.LogInformation("Applying database migrations...");
        await db.Database.MigrateAsync();
        logger.LogInformation("Database migrations applied successfully");
        
        // Seed data
        var seeder = services.GetRequiredService<DatabaseSeeder>();
        await seeder.SeedAsync();
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "An error occurred during database initialization");
        throw;
    }
}

app.MapControllers();

app.Run();
