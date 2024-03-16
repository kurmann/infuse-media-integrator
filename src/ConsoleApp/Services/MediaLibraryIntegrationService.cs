using Microsoft.Extensions.Logging;

namespace Kurmann.InfuseMediaIntegrator.Services;

public class MediaLibraryIntegrationService
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

    private void MoveFileToLibrary(FileAddedEventMessage fileCreatedEvent)
    {
        // Implementiere hier die Logik zum Verschieben der Datei.
        // Verwende den Inhalt von 'MoveFileToMediaLibraryCommand' als Referenz für die Implementierung.

    }

}
