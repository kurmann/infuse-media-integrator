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
    private readonly IMessageService _messageService;

    public FileWatcherService(ILogger<FileWatcherService> logger, IOptions<ModuleOptions> options, IMessageService messageService)
    {
        if (options is null)
            throw new ArgumentNullException(nameof(options));
        if (options.Value.LocalFileSystem is null)
            throw new ArgumentNullException(nameof(options.Value.LocalFileSystem));
        if (options.Value.LocalFileSystem.WatchPath is null)
            throw new ArgumentNullException(nameof(options.Value.LocalFileSystem.WatchPath));

        _logger = logger;
        _watchPath = options.Value.LocalFileSystem.WatchPath;
        _messageService = messageService;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _fileWatcher = new FileSystemWatcher(_watchPath)
        {
            NotifyFilter = NotifyFilters.FileName | NotifyFilters.LastWrite,
            Filter = "*.*" // Überwacht alle Dateitypen, dies kann nach Bedarf angepasst werden.
        };

        _fileWatcher.Created += OnCreated;
        _fileWatcher.Deleted += OnDeleted;
        _fileWatcher.Changed += OnChanged;
        _fileWatcher.EnableRaisingEvents = true;

        _logger.LogInformation("FileWatcherService has started on directory: {WatchPath}", _watchPath);

        return Task.CompletedTask;
    }

    private void OnCreated(object sender, FileSystemEventArgs e)
    {
        _logger.LogInformation("New file: {FileName}", e.FullPath);
        _messageService.Send(new FileAddedEventMessage(e.FullPath));
    }

    private void OnDeleted(object sender, FileSystemEventArgs e)
    {
        _logger.LogInformation("File deleted: {FileName}", e.FullPath);
        _messageService.Send(new FileChangedEventMessage(e.FullPath));
    }

    private void OnChanged(object sender, FileSystemEventArgs e)
    {
        _logger.LogInformation("File changed: {FileName}", e.FullPath);
        _messageService.Send(new FileDeletedEventMessage(e.FullPath));
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

public class FileAddedEventMessage(string? filePath) : EventMessageBase
{
    public string? FilePath { get; } = filePath;
}

public class FileChangedEventMessage(string? filePath) : EventMessageBase
{
    public string? FilePath { get; } = filePath;
}

public class FileDeletedEventMessage(string? filePath) : EventMessageBase
{
    public string? FilePath { get; } = filePath;
}
