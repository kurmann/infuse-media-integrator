using Kurmann.InfuseMediaIntegratior;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;

namespace Kurmann.InfuseMediaIntegrator;

internal class Program
{
    static void Main(string[] args)
    {
        CreateHostBuilder(args).Build().Run();
    }

    public static IHostBuilder CreateHostBuilder(string[] args) =>
    Host.CreateDefaultBuilder(args)
        .ConfigureAppConfiguration((hostContext, config) =>
        {
            config.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
        })
        .ConfigureServices((hostContext, services) =>
        {
            var configuration = hostContext.Configuration;
            services.Configure<ModuleOptions>(configuration.GetSection("LocalFileSystem"));
            // services.AddHostedService<MyModuleService>();
            // Weitere Dienste konfigurieren
        });
}