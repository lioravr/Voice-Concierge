using Microsoft.Extensions.DependencyInjection;
using VoiceConcierge.Core.Services;

namespace VoiceConcierge.Core;

public static class DependencyInjection
{
    public static IServiceCollection AddCoreServices(this IServiceCollection services)
    {
        // Register core application services
        services.AddScoped<IFAQService, FAQService>();
        services.AddScoped<IUnansweredQuestionService, UnansweredQuestionService>();
        services.AddScoped<IVoiceConfigurationService, VoiceConfigurationService>();
        services.AddScoped<IAuthService, AuthService>();

        return services;
    }
}
