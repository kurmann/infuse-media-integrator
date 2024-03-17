using Kurmann.InfuseMediaIntegratior;
using Kurmann.InfuseMediaIntegrator.Commands;
using Kurmann.InfuseMediaIntegrator.Entities.Elementary;
using Kurmann.InfuseMediaIntegrator.Entities.MediaLibrary;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Kurmann.InfuseMediaIntegrator.Services;

public class MediaLibraryIntegrationService : IHostedService, IDisposable
{
    private readonly ILogger<MediaLibraryIntegrationService> _logger;
    private readonly IMessageService _messageService;
    private readonly string? _mediaLibraryPath;
    private readonly string? _inputDirectory;

    public MediaLibraryIntegrationService(ILogger<MediaLibraryIntegrationService> logger, IOptions<AppSettings> options, IMessageService messageService)
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
        // Überprüfen, ob der Dateipfad in der Nachricht leer ist
        if (string.IsNullOrWhiteSpace(eventMessage.FilePath))
        {
            _logger.LogWarning("Received empty file path from event message. Ignoring this message.");
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
            subDirectory = eventMessage.FilePath.Replace(_inputDirectory, string.Empty).TrimStart('\\');
            _logger.LogInformation("Subdirectory of file {FilePath} is {SubDirectory}. This will be represented in the media library.", eventMessage.FilePath, subDirectory);
        }

        var command = new MoveFileToMediaLibraryCommand
        {
            Logger = _logger,
            FilePath = eventMessage.FilePath,
            MediaLibraryPath = _mediaLibraryPath,
            SubDirectory = subDirectory
        };

        var result = command.Execute();
        if (result.IsFailure)
        {
            // todo: refactor the command event away
        }
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
        _messageService.Unsubscribe<InputDirectoryFileChangedEventMessage>(MoveFileToLibrary);
    }

}

public class FileMovedToMediaLibraryEventMessage : EventMessageBase
{
    public required FileInfo SourceFilePath { get; init; }
    public required FileInfo TargetFilePath { get; init; }
    bool HasTargetFileBeenOverwritten { get; init; }
    public required MediaGroupInformation MediaGroup { get; init; }
}

public class MediaGroupInformation
{
    public required MediaGroupId Id { get; init; }
    public required DirectoryPathInfo RelativeMediaGroupDirectory { get; init; }
    public required DirectoryPathInfo RelativeSubDirectory { get; init; }
    bool HasMovedToExistingMediaGroup { get; init; }
}
    