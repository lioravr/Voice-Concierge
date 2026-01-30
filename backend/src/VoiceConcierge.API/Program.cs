using Microsoft.EntityFrameworkCore;
using VoiceConcierge.API;
using VoiceConcierge.Infrastructure.Data;
using VoiceConcierge.Infrastructure.Data.Seed;

var builder = WebApplication.CreateBuilder(args);

// Configure services using Startup
var startup = new Startup(builder.Configuration);
startup.ConfigureServices(builder.Services);

var app = builder.Build();

// Configure the HTTP request pipeline using Startup
startup.Configure(app, app.Environment);

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

app.Run();
