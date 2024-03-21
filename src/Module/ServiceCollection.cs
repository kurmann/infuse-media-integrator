using Kurmann.InfuseMediaIntegrator.Module;
using Kurmann.InfuseMediaIntegrator.Module.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Kurmann.InfuseMediaIntegrator.Module;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfuseMediaIntegrator(this IServiceCollection services, Action<ModuleSettings> configure)
    {
        // Konfigurationseinstellungen anwenden
        if (configure != null)
        {
            services.Configure(configure);
        }
        
        services.AddHostedService<FileWatcherService>();
        services.AddHostedService<MediaLibraryIntegrationService>();

        services.AddSingleton<IMessageService, MessageService>();

        return services;
    }
}
