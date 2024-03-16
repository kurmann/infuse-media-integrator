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
    private readonly List<string> _watchedFileExtensions;

    public FileWatcherService(ILogger<FileWatcherService> logger, IOptions<ModuleOptions> options, IMessageService messageService)
    {
        ArgumentNullException.ThrowIfNull(options);
        ArgumentNullException.ThrowIfNull(options.Value.LocalFileSystem);
        ArgumentNullException.ThrowIfNull(options.Value.LocalFileSystem.WatchPath);

        _logger = logger;
        _watchPath = options.Value.LocalFileSystem.WatchPath;
        _messageService = messageService;
        _watchedFileExtensions = options.Value.LocalFileSystem.WatchedFileExtensions;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _fileWatcher = new FileSystemWatcher(_watchPath)
        {
            // Überwacht nur Dateinamen und Änderungen an Dateien.
            NotifyFilter = NotifyFilters.FileName | NotifyFilters.LastWrite | NotifyFilters.DirectoryName,

            // Filter für Dateiendungen, die nicht überwacht werden sollen.
            IncludeSubdirectories = true,
        };

        // Fügt die Dateiendungen hinzu, die überwacht werden sollen.
        foreach (var fileExtension in _watchedFileExtensions)
        {
            _fileWatcher.Filters.Add(fileExtension);
        }

        // Logge, die Dateiendungen, die überwacht werden.
        _logger.LogInformation("FileWatcherService is watching for file extensions: {WatchedFileExtensions}", string.Join(", ", _watchedFileExtensions));

        // Event-Handler für die verschiedenen Ereignisse.
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

    private void OnChanged(object sender, FileSystemEventArgs e)
    {
        _logger.LogInformation("File changed: {FileName}", e.FullPath);
        _messageService.Send(new FileChangedEventMessage(e.FullPath));
    }

    private void OnDeleted(object sender, FileSystemEventArgs e)
    {
        _logger.LogInformation("File deleted: {FileName}", e.FullPath);
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
