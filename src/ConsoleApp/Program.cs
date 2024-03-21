using Kurmann.InfuseMediaIntegrator.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Kurmann.InfuseMediaIntegrator.ConsoleApp;

internal class Program
{
    static void Main(string[] args) => CreateHostBuilder(args).Build().Run();

    public static IHostBuilder CreateHostBuilder(string[] args) =>
    Host.CreateDefaultBuilder(args)
        .ConfigureServices((hostContext, services) =>
        {
            services.Configure<ModuleSettings>(hostContext.Configuration);

            services.AddLogging(builder =>
            {
                builder.ClearProviders();
                builder.AddConsole();
            });

            services.AddHostedService<FileWatcherService>();
            services.AddHostedService<MediaLibraryIntegrationService>();

            services.AddSingleton<IMessageService, MessageService>();
        });
}