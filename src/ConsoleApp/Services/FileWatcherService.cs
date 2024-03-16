using Kurmann.InfuseMediaIntegratior;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Kurmann.InfuseMediaIntegrator.Services;

public class FileWatcherService : IHostedService, IDisposable
{
    private readonly ILogger<FileWatcherService> _logger;
    private FileSystemWatcher? _fileWatcher;
    private readonly string _watchPath;

    public FileWatcherService(ILogger<FileWatcherService> logger, IOptions<ModuleOptions> options)
    {
        ArgumentNullException.ThrowIfNull(options?.Value.WatchPath);

        _logger = logger;
        _watchPath = options.Value.WatchPath;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _fileWatcher = new FileSystemWatcher(_watchPath)
        {
            NotifyFilter = NotifyFilters.FileName | NotifyFilters.LastWrite,
            Filter = "*.*" // Überwacht alle Dateitypen, dies kann nach Bedarf angepasst werden.
        };

        _fileWatcher.Created += OnCreated;
        _fileWatcher.EnableRaisingEvents = true;

        _logger.LogInformation("FileWatcherService has started on directory: {WatchPath}", _watchPath);

        return Task.CompletedTask;
    }

    private void OnCreated(object sender, FileSystemEventArgs e)
    {
        // Hinweis: Hier können Sie Ihren internen Nachrichtenkanal integrieren, um eine Nachricht zu versenden.
        _logger.LogInformation("New file: {FileName}", e.FullPath);

        // Beispiel: NachrichtenkanalEvent?.Invoke(this, new NachrichtenkanalEventArgs(e.FullPath));
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _fileWatcher?.Dispose();
        _logger.LogInformation("FileWatcherService has been stopped.");
        return Task.CompletedTask;
    }

    public void Dispose()
    {
        _fileWatcher?.Dispose();
        GC.SuppressFinalize(this);
    }
}
