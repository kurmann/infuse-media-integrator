using Kurmann.InfuseMediaIntegrator.Module.Commands;
using Kurmann.InfuseMediaIntegrator.Entities.Elementary;
using Kurmann.InfuseMediaIntegrator.Entities.MediaFileTypes;
using Kurmann.InfuseMediaIntegrator.Entities.MediaLibrary;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Kurmann.InfuseMediaIntegrator.Module.Services;

public class MediaLibraryIntegrationService : IHostedService, IDisposable
{
    private readonly ILogger<MediaLibraryIntegrationService> _logger;
    private readonly IMessageService _messageService;
    private readonly string? _mediaLibraryPath;
    private readonly string? _inputDirectory;

    public MediaLibraryIntegrationService(ILogger<MediaLibraryIntegrationService> logger, IOptions<ModuleSettings> options, IMessageService messageService)
    {
        _logger = logger;
        _messageService = messageService;
        _mediaLibraryPath = options.Value.MediaLibraryPath;
        _inputDirectory = options.Value.InputDirectory?.Path;

        // Abonnieren von Nachrichten, um Dateien zu verschieben, wenn die entsprechende Nachricht empfangen wird.
        _messageService.Subscribe<InputDirectoryFileChangedEventMessage>(MoveFileToLibrary);
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("MediaLibraryIntegrationService has started");
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("MediaLibraryIntegrationService has stopped");
        return Task.CompletedTask;
    }

    private void MoveFileToLibrary(InputDirectoryFileChangedEventMessage eventMessage)
    {
        // Überprüfen, ob überhaupt eine Datei verschoben werden soll
        if (eventMessage.ChangeType == WatcherChangeTypes.Deleted)
        {
            _logger.LogInformation("File {FilePath} has been deleted. Ignoring the file.", eventMessage.File.FilePath);
            return;
        }

        // Überprüfen, ob das Eingangsverzeichnis nicht angegeben ist
        var subDirectory = string.Empty;
        if (string.IsNullOrWhiteSpace(_inputDirectory))
        {
            _logger.LogWarning("Input directory is not set. This is necessary to detect the sub directory of the file. Ignoring the subdirectories.");
        }
        else
        {
            // Ermitte das Unterverzeichnis in das die Datei verschoben werden soll,
            // indem der relative Pfad der Datei zum Eingangsverzeichnis ermittelt wird
            subDirectory = Path.GetRelativePath(_inputDirectory, eventMessage.File.FilePath.DirectoryPathInfo);
            _logger.LogInformation("Subdirectory of file {FilePath} is {SubDirectory}. This will be represented in the media library.", eventMessage.File, subDirectory);
        }

        var command = new MoveFileToMediaLibraryCommand
        {
            Logger = _logger,
            FilePath = eventMessage.File.FilePath,
            MediaLibraryPath = _mediaLibraryPath,
            SubDirectory = subDirectory
        };

        var result = command.Execute();
        if (result.IsFailure)
        {
            _logger.LogError("Error while moving file {FilePath} to media library. Error: {Error}", eventMessage.File.FilePath, result.Error);
            return;
        }

        _logger.LogInformation("File {FilePath} has been moved to media library.", eventMessage.File.FilePath);
        _messageService.Publish(new FileMovedToMediaLibraryEventMessage
        {
            SourceFilePath = eventMessage.File,
            TargetFilePath = result.Value.MediaFile,
            HasTargetFileBeenOverwritten = result.Value.HasTargetFileBeenOverwritten
        });
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
        _messageService.Unsubscribe<InputDirectoryFileChangedEventMessage>(MoveFileToLibrary);
    }

}

public class FileMovedToMediaLibraryEventMessage : EventMessageBase
{
    public required IMediaFileType SourceFilePath { get; init; }
    public required IMediaFileType TargetFilePath { get; init; }
    public bool HasTargetFileBeenOverwritten { get; init; }
    public MediaGroupInformation? MediaGroup { get; init; }
}

public class MediaGroupInformation
{
    public required MediaGroupId Id { get; init; }
    public required DirectoryPathInfo RelativeMediaGroupDirectory { get; init; }
    public required DirectoryPathInfo RelativeSubDirectory { get; init; }
    public bool HasMovedToExistingMediaGroup { get; init; }
}
    