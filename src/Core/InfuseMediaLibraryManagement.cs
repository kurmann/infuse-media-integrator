using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Kurmann.InfuseMediaIntegrator;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfuseMediaLibraryManagement(this IServiceCollection services, IConfiguration configuration)
    {
        // Beispiel für das Auslesen von Konfigurationswerten
        var sourceDirectory = configuration.GetRequiredSection("Directories");

        // Registriere deine Services mit den ausgelesenen Konfigurationswerten
        services.AddSingleton<IInfuseMediaLibraryService, InfuseMediaLibraryService>();

        // Weitere Konfigurationen und Service-Registrierungen

        return services;
    }
}
