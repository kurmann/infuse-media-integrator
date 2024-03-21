namespace Kurmann.InfuseMediaIntegrator;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfuseMediaIntegrator(this IServiceCollection services, Action<AppSettings> configure)
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
