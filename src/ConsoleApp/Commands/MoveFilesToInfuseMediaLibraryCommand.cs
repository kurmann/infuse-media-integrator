using CSharpFunctionalExtensions;
using Kurmann.InfuseMediaIntegrator.Commands;
using Kurmann.InfuseMediaIntegrator.Entities;
using Microsoft.Extensions.Logging;

namespace Kurmann.InfuseMediaIntegrator.Tests.Entities;

public class MoveFilesToInfuseMediaLibraryCommand(string inputDirectoryPath, string infuseMediaLibraryPath, ILogger logger) : ICommand
{
    /// <summary>
    /// Der Pfad des Eingangsverzeichnisses.
    /// </summary>
    public string InputDirectoryPath { get; } = inputDirectoryPath;

    private readonly ILogger _logger = logger;
    

    /// <summary>
    /// Der Pfad des Infuse Media Library-Verzeichnisses.
    /// </summary>
    public string InfuseMediaLibraryPath { get; } = infuseMediaLibraryPath;

    public Result Execute()
    {
        _logger.LogInformation($"Moving files from {InputDirectoryPath} to {InfuseMediaLibraryPath}");

        // Prüfe ob das Eingangsverzeichnis existiert
        if (!Directory.Exists(InputDirectoryPath))
        {
            return Result.Failure($"Directory not found: {InputDirectoryPath}");
        }

        // Prüfe ob das Infuse Media Library-Verzeichnis existiert
        if (!Directory.Exists(InfuseMediaLibraryPath))
        {
            return Result.Failure($"Directory not found: {InfuseMediaLibraryPath}");
        }

        // Erstelle ein VideoIntegrationDirectory-Objekt
        var videoIntegrationDirectory = VideoIntegrationDirectory.Create(InputDirectoryPath);
        if (videoIntegrationDirectory.IsFailure)
        {
            return Result.Failure(videoIntegrationDirectory.Error);
        }
        // Liste die Dateien im Eingangsverzeichnis auf
        _logger.LogInformation($"Found {videoIntegrationDirectory.Value.Mpeg4VideoFiles.Count} MPEG4 video files.");
        _logger.LogInformation($"Found {videoIntegrationDirectory.Value.QuickTimeVideoFiles.Count} QuickTime video files.");
        _logger.LogInformation($"Found {videoIntegrationDirectory.Value.NotSupportedFiles.Count} of not supported files.");

        // Bewege die MPEG4-Dateien in das Infuse Media Library-Verzeichnis
        _logger.LogInformation($"Moving {videoIntegrationDirectory.Value.Mpeg4VideoFiles.Count} MPEG4 video files to {InfuseMediaLibraryPath}");
        foreach (var mpeg4VideoFile in videoIntegrationDirectory.Value.Mpeg4VideoFiles)
        {
            // Platzhalter für die Videokategorie, die aus den Metadaten ausgelesen wird
            var category = string.Empty;

            // Lese die Metadaten aus dem Mpeg4VideoWithMetadata
            var metadata = Mpeg4VideoWithMetadata.Create(mpeg4VideoFile.FileInfo.FullName);
            if (metadata.IsSuccess)
            {
                // Verwende die Videokategorie aus den Metadaten (entspricht "Album" in den Metadaten)
                _logger.LogInformation($"Category found in metadata: {metadata.Value.Album}");
                category = metadata.Value.Album;
            }
            _logger.LogWarning("No category found in metadata. Leaving category empty.");

            // Erstelle ein FileMappingInfo-Objekt
            var fileMappingInfo = FileMappingInfo.Create(category, mpeg4VideoFile.FileInfo.Name);
            if (fileMappingInfo.IsFailure)
            {
                // Wenn das FileMappingInfo-Objekt nicht erstellt werden konnte, liegt ein kririscher Fehler vor, der das Verschieben der Datei verhindert
                return Result.Failure(fileMappingInfo.Error);
            }

            // Verschiebe die Datei gemäss fileMappingInfo.TargetPath
            var targetPath = Path.Combine(InfuseMediaLibraryPath, fileMappingInfo.Value.TargetPath);
            try
            {
                // Prüfe ob das Zielverzeichnis existiert und erstelle es, falls es nicht existiert
                var targetDirectory = Path.GetDirectoryName(targetPath);
                if (targetDirectory == null)
                {
                    return Result.Failure($"The target directory could not be determined: {targetPath}");
                }
                if (!Directory.Exists(targetDirectory))
                {
                    _logger.LogInformation($"Creating directory: {targetDirectory}");
                    Directory.CreateDirectory(targetDirectory);
                }

                // Prüfe ob die Datei bereits existiert und lösche sie, falls sie existiert
                if (File.Exists(targetPath))
                {
                    // Lösche die Datei
                    _logger.LogInformation($"Deleting existing file: {targetPath}");
                    File.Delete(targetPath);
                }

                _logger.LogInformation($"Moving file: {mpeg4VideoFile.FileInfo.FullName} to {targetPath}");
                File.Move(mpeg4VideoFile.FileInfo.FullName, targetPath);
            }
            catch (Exception e)
            {
                return Result.Failure($"The file could not be moved: {e.Message}");
            }

            _logger.LogInformation($"File successfully moved: {targetPath}");
        }

        _logger.LogInformation($"Files successfully moved from {InputDirectoryPath} to {InfuseMediaLibraryPath}");
        return Result.Success();
    }
}