using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using VoiceConcierge.Core.Domain.Interfaces;
using VoiceConcierge.Core.Services;
using VoiceConcierge.Infrastructure.Data;
using VoiceConcierge.Infrastructure.Data.Repositories;
using VoiceConcierge.Infrastructure.Data.Seed;
using VoiceConcierge.Infrastructure.Repositories;
using VoiceConcierge.Infrastructure.Services;

namespace VoiceConcierge.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        // Configure PostgreSQL with pgvector
        var connectionString = configuration.GetConnectionString("DefaultConnection");
        services.AddDbContext<ApplicationDbContext>(options =>
        {
            options.UseNpgsql(connectionString, npgsqlOptions =>
            {
                npgsqlOptions.UseVector();
            });
        });

        // Register repositories
        services.AddScoped<IFAQRepository, FAQRepository>();
        services.AddScoped<IUnansweredQuestionRepository, UnansweredQuestionRepository>();
        services.AddScoped<IVoiceConfigurationRepository, VoiceConfigurationRepository>();
        services.AddScoped<IUserRepository, UserRepository>();

        // Register infrastructure services
        services.AddScoped<IEmbeddingService, OpenAIEmbeddingService>();

        // Register database seeder
        services.AddScoped<DatabaseSeeder>();

        // Add HttpClient for external API calls (OpenAI, etc.)
        services.AddHttpClient();

        return services;
    }
}
