using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Kurmann.InfuseMediaIntegrator.Services;

public class MediaLibraryIntegrationService : IHostedService, IDisposable
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

    public void Dispose()
    {
        GC.SuppressFinalize(this);
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

    private void MoveFileToLibrary(FileAddedEventMessage fileAddedEventMessage)
    {
        _logger.LogInformation("Move {FileName} in media library", fileAddedEventMessage.FilePath);
        // Implementiere hier die Logik zum Verschieben der Datei.
        // Verwende den Inhalt von 'MoveFileToMediaLibraryCommand' als Referenz für die Implementierung.

    }

}
