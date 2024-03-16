using Kurmann.InfuseMediaIntegratior;
using Kurmann.InfuseMediaIntegrator.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Kurmann.InfuseMediaIntegrator;

internal class Program
{
    static void Main(string[] args) => CreateHostBuilder(args).Build().Run();

    public static IHostBuilder CreateHostBuilder(string[] args) =>
    Host.CreateDefaultBuilder(args)
        .ConfigureServices((hostContext, services) =>
        {
            var configuration = hostContext.Configuration;
            services.Configure<ModuleOptions>(configuration.GetSection("LocalFileSystem"));

            services.AddHostedService<FileWatcherService>();
            services.AddSingleton<IMessageService, MessageService>();
        });
}