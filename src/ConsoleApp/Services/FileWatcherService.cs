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
            _fileWatcher.Filters.Add($"*{fileExtension}");
        }

        // Logge, die Dateiendungen, die überwacht werden.
        _logger.LogInformation("FileWatcherService is watching for file extensions: {WatchedFileExtensions}", string.Join(", ", _watchedFileExtensions));

        // Event-Handler für die verschiedenen Ereignisse.
        // Hinweis: Die Ereignisse 'Created', 'Changed', 'Renamed' und 'Deleted' werden alle in 'OnFileChanged' behandelt.
        _fileWatcher.Created += OnFileChanged;
        _fileWatcher.Changed += OnFileChanged;
        _fileWatcher.Renamed += OnFileChanged;
        _fileWatcher.Deleted += OnFileChanged;
        _fileWatcher.EnableRaisingEvents = true;

        _logger.LogInformation("FileWatcherService has started on directory: {WatchPath}", _watchPath);

        return Task.CompletedTask;
    }

    private void OnFileChanged(object sender, FileSystemEventArgs e)
    {
        _logger.LogInformation("File {filePath} {changeType}", e.FullPath, e.ChangeType.ToString());
        _messageService.Send(new InputDirectoryFileChangedEventMessage(e.FullPath, e.ChangeType.ToString()));
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

/// <summary>
/// Nachricht, die gesendet wird, wenn eine Datei im Überwachungsverzeichnis geändert wird.
/// Hinweis: Diese Nachricht wird von 'FileWatcherService' gesendet.
/// </summary>
/// <param name="filePath"></param>
public class InputDirectoryFileChangedEventMessage(string filePath, string? changeType = null) : EventMessageBase
{
    public string FilePath { get; } = filePath;

    public string? ChangeType { get; } = changeType;
}
