﻿using Kurmann.InfuseMediaIntegrator.Module;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Kurmann.InfuseMediaIntegrator.ConsoleApp;

internal class Program
{
    static void Main(string[] args) => CreateHostBuilder(args).Build().Run();

    public static IHostBuilder CreateHostBuilder(string[] args)
    {
        return Host.CreateDefaultBuilder(args)
            .ConfigureServices((hostContext, services) =>
            {
                // Lade und binde die ModuleSettings aus der Konfiguration
                var moduleSettingsSection = hostContext.Configuration;
                services.Configure<ModuleSettings>(moduleSettingsSection);

                // Übergebe die ModuleSettings an den InfuseMediaIntegrator
                var moduleSettings = moduleSettingsSection.Get<ModuleSettings>();
                services.AddInfuseMediaIntegrator(moduleSettings);

                // Konfiguriere das Logging
                services.AddLogging(builder =>
                {
                    builder.ClearProviders();
                    builder.AddConsole();
                });
            });
    }

}