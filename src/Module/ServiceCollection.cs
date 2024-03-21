using Kurmann.InfuseMediaIntegrator.Module;
using Kurmann.InfuseMediaIntegrator.Module.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Kurmann.InfuseMediaIntegrator.Module;

public static class ServiceCollection
{
    public static IServiceCollection AddInfuseMediaIntegrator(this IServiceCollection services, ModuleSettings? moduleSettings)
    {
        // Konfigurationseinstellungen anwenden
        if (moduleSettings != null)
        {
            services.AddSingleton(moduleSettings);
        }
        
        services.AddHostedService<FileWatcherService>();
        services.AddHostedService<MediaLibraryIntegrationService>();

        services.AddSingleton<IMessageService, MessageService>();

        return services;
    }
}
