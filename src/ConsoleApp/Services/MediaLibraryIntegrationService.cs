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
        _messageService.Subscribe<FileChangedEventMessage>(MoveFileToLibrary);
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

    private void MoveFileToLibrary(FileChangedEventMessage message)
    {
        _logger.LogInformation("Move {FileName} in media library", message.FilePath);
        // Implementiere hier die Logik zum Verschieben der Datei.
        // Verwende den Inhalt von 'MoveFileToMediaLibraryCommand' als Referenz für die Implementierung.

    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
        _messageService.Unsubscribe<FileChangedEventMessage>(MoveFileToLibrary);
    }

}
