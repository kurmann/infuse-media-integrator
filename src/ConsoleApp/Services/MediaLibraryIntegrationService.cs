using Microsoft.Extensions.Logging;

namespace Kurmann.InfuseMediaIntegrator.Services;

public interface IMediaLibraryIntegrationService
{
}

public class MediaLibraryIntegrationService : IMediaLibraryIntegrationService
{
    private readonly ILogger<MediaLibraryIntegrationService> _logger;
    private readonly IMessageService _messageService;

    public MediaLibraryIntegrationService(ILogger<MediaLibraryIntegrationService> logger, IMessageService messageService)
    {
        _logger = logger;
        _messageService = messageService;

        // Abonnieren von Nachrichten, um Dateien zu verschieben, wenn die entsprechende Nachricht empfangen wird.
        _messageService.Subscribe<FileAddedEventMessage>(MoveFileToLibrary);
    }

    private void MoveFileToLibrary(FileAddedEventMessage fileAddedEventMessage)
    {
        _logger.LogInformation("Move {FileName} in media library", fileAddedEventMessage.FilePath);
        // Implementiere hier die Logik zum Verschieben der Datei.
        // Verwende den Inhalt von 'MoveFileToMediaLibraryCommand' als Referenz für die Implementierung.

    }

}
