using CSharpFunctionalExtensions;
using Kurmann.InfuseMediaIntegrator.Commands;
using Kurmann.InfuseMediaIntegrator.Entities;
using Kurmann.InfuseMediaIntegrator.Entities.MediaFileTypes;
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
            // Lese die Metadaten aus dem Mpeg4VideoWithMetadata
            var metadataFromFileQuery = new MetadataFromFileQuery(mpeg4VideoFile.FilePath);
            var metadata = metadataFromFileQuery.Execute();
            if (metadata.IsFailure)
            {
                _logger.LogWarning("Error on reading metadata");
                _logger.LogWarning(metadata.Error);
                _logger.LogInformation("File mapping will use directory structure and file name for file mapping.");
            }
            else
            {
                // Aktualisiere das Mpeg4Video-Objekt mit den Metadaten
                mpeg4VideoFile.SetMetadata(metadata.Value);
            }

            return MoveFileToLibrary(mpeg4VideoFile);
        }

        _logger.LogInformation($"Files successfully moved from {InputDirectoryPath} to {InfuseMediaLibraryPath}");
        return Result.Success();
    }

    private Result MoveFileToLibrary(IMediaFileType mediaFile)
    {
        // Erstelle ein MediaFileLibraryOrganizationInfo-Objekt
        var fileMappingInfo = MediaFileLibraryOrganizationInfo.Create(mediaFile, InfuseMediaLibraryPath);
        if (fileMappingInfo.IsFailure)
        {
            return Result.Failure(fileMappingInfo.Error);
        }

        // Verschiebe die Datei gemäss fileMappingInfo.TargetPath
        var targetPath = Path.Combine(InfuseMediaLibraryPath, fileMappingInfo.Value.TargetSubDirectory ?? string.Empty, fileMappingInfo.Value.TargetFileName);
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

            _logger.LogInformation($"Moving file: {mediaFile} to {targetPath}");
            File.Move(mediaFile.FilePath, targetPath);
        }
        catch (Exception e)
        {
            return Result.Failure($"The file could not be moved: {e.Message}");
        }

        _logger.LogInformation($"File successfully moved: {targetPath}");
        return Result.Success();
    }
}