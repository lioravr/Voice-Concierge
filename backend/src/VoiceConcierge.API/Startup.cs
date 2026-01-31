using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using VoiceConcierge.Core;
using VoiceConcierge.Core.Constants;
using VoiceConcierge.Infrastructure;
using VoiceConcierge.Infrastructure.Data;
using VoiceConcierge.Infrastructure.Data.Seed;

namespace VoiceConcierge.API;

public class Startup
{
    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    // This method gets called by the runtime. Use this method to add services to the container
    public void ConfigureServices(IServiceCollection services)
    {
        // Add controllers
        services.AddControllers();
        services.AddEndpointsApiExplorer();
        
        // Configure Swagger
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new() { Title = "Voice Concierge API", Version = "v1" });
        });

        // Add Core layer services
        services.AddCoreServices();

        // Add Infrastructure layer services (includes database, repositories, external services)
        services.AddInfrastructureServices(Configuration);

        // Configure JWT Authentication
        ConfigureAuthentication(services);

        // Configure Authorization
        services.AddAuthorization();

        // Configure CORS
        ConfigureCors(services);

        // Add health checks
        services.AddHealthChecks();
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();
        app.UseRouting();
        app.UseCors();

        app.UseAuthentication();
        app.UseAuthorization();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
            endpoints.MapHealthChecks("/health");

            // Database connection test endpoint
            endpoints.MapGet("/api/test/db-connection", async (ApplicationDbContext db) =>
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
        });
    }

    private void ConfigureAuthentication(IServiceCollection services)
    {
        var jwtKey = Configuration[ConfigurationKeys.Jwt.Key] ?? throw new InvalidOperationException("JWT Key not configured");
        var jwtIssuer = Configuration[ConfigurationKeys.Jwt.Issuer] ?? "VoiceConcierge";
        var jwtAudience = Configuration[ConfigurationKeys.Jwt.Audience] ?? "VoiceConciergeClient";

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = jwtIssuer,
                ValidAudience = jwtAudience,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
            };
        });
    }

    private void ConfigureCors(IServiceCollection services)
    {
        services.AddCors(options =>
        {
            options.AddDefaultPolicy(policy =>
            {
                policy.WithOrigins(
                        "http://localhost:3000",  // Docker admin panel
                        "http://localhost:3001",  // Local dev server (alternate port)
                        "http://localhost:5173"   // Vite default port
                    )
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials();
            });
        });
    }
}
