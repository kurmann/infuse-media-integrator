using CSharpFunctionalExtensions;
using Kurmann.InfuseMediaIntegrator.Entities.Elementary;
using Kurmann.InfuseMediaIntegrator.Entities.MediaFileTypes;
using Kurmann.InfuseMediaIntegrator.Entities.MediaLibrary;
using Kurmann.InfuseMediaIntegrator.Queries;
using Microsoft.Extensions.Logging;

namespace Kurmann.InfuseMediaIntegrator.Commands;

public class MoveFileToMediaLibraryCommand : ICommand<FileMovedToMediaLibraryResultArgs>
{
    public ILogger? Logger { get; init; }

    /// <summary>
    /// Der Pfad zur Datei, die in die Mediengruppe verschoben werden soll
    /// </summary>
    public required string FilePath { get; init; }

    /// <summary>
    /// Der Pfad zum Infuse Media Library-Verzeichnis
    /// </summary>
    public required string? MediaLibraryPath { get; init; }

    /// <summary>
    /// Der optionale Unterverzeichnisname, in das die Datei verschoben werden soll
    /// </summary>
    public string? SubDirectory { get; init; }

    public Result<FileMovedToMediaLibraryResultArgs> Execute()
    {
        Logger?.LogInformation("Moving file from {FilePath} to media library {MediaLibraryPath}", FilePath, MediaLibraryPath);

        // Sammle Informationen über die Datei
        var mediaFile = MediaFileTypeDetector.GetMediaFile(FilePath);
        if (mediaFile.IsFailure)
        {
            return Result.Failure<FileMovedToMediaLibraryResultArgs>("Error on detecting media file type: " + mediaFile.Error);
        }

        // Sammle Informationen über das Zielverzeichnis
        var mediaLibrary = DirectoryPathInfo.Create(MediaLibraryPath);
        if (mediaLibrary.IsFailure)
        {
            return Result.Failure<FileMovedToMediaLibraryResultArgs>("Error on creating media library directory path: " + mediaLibrary.Error);
        }

        // Verschiebe die Datei in ein bestehendes Mediengruppen-Verzeichnis sofern vorhanden
        var resultArgs = TryMovingToExistingMediaGroup(mediaFile.Value, mediaLibrary.Value);

        // Wenn die Datei nicht Teil einer bestehenden Mediengruppe ist, dann verschiebe die Datei in eine neue Mediengruppe
        if (resultArgs.IsFailure)
        {
            // Logge eine Warnung, dass die Abfrage nach einer bestehenden Mediengruppe fehlgeschlagen ist
            Logger?.LogWarning("Error on querying existing media group: {Error}", resultArgs.Error);
            Logger?.LogInformation("Moving file to new media group.");

            return MoveToNewMediaGroup();
        }

        // Prüfe, ob die Datei bereits in eine existierende Mediengruppe verschoben wurde (mit der Methode TryMovingToExistingMediaGroup) und gib den Erfolg zurück
        if (resultArgs.Value != null)
        {
            Logger?.LogInformation("File {FilePath} was successfully moved to media group {existingMediaGroupDirectory}", FilePath, resultArgs.Value);
            return Result.Success(resultArgs.Value);
        }

        // Sonst verschiebe die Datei in eine neue Mediengruppe
        return MoveToNewMediaGroup();

        Result<FileMovedToMediaLibraryResultArgs> MoveToNewMediaGroup()
        {
            var mediaFileLibraryDestinationMapping = MediaFileLibraryDestinationMapping.Create(FilePath, mediaLibrary.Value, SubDirectory);
            if (mediaFileLibraryDestinationMapping.IsFailure)
            {
                return Result.Failure<FileMovedToMediaLibraryResultArgs>("Error on creating media file library destination mapping: " + mediaFileLibraryDestinationMapping.Error);
            }

            try
            {
                // Ermittle die Quelldatei und das Zielverzeichnis
                var sourceFile = new FileInfo(FilePath);
                var targetDirectory = new DirectoryInfo(mediaFileLibraryDestinationMapping.Value.TargetDirectory);

                // Erstellen des Zielverzeichnisses, falls es nicht existiert
                if (!targetDirectory.Exists)
                {
                    targetDirectory.Create();
                }

                var targetFilePath = Path.Combine(targetDirectory.FullName, sourceFile.Name);

                // Prüfe, ob die Datei bereits im Zielverzeichnis existiert
                var targetFileAlreadyExists = File.Exists(targetFilePath);

                // Verschiebe die Datei und gib den Erfolg zurück
                File.Move(FilePath, targetFilePath, true);
                var resultArgs = new FileMovedToMediaLibraryResultArgs(new FileInfo(targetFilePath), false, targetFileAlreadyExists);

                return Result.Success(resultArgs);
            }
            catch (Exception e)
            {
                return Result.Failure<FileMovedToMediaLibraryResultArgs>("Error on moving file to new media group: " + e.Message);
            }
        }
    }

    /// <summary>
    /// Versucht die Datei in eine bestehende Mediengruppe zu verschieben, falls die Mediengruppe anhand des Dateinamens ermittelt werden kann
    /// und die Mediengruppe im Infuse Media Library-Verzeichnis existiert.
    /// </summary>
    /// <returns></returns>
    private static Result<FileMovedToMediaLibraryResultArgs?> TryMovingToExistingMediaGroup(IMediaFileType mediaFile, DirectoryPathInfo mediaLibrary)
    {
        // Prüfe, ob der Dateipfad existiert
        if (!File.Exists(mediaFile.FilePath))
        {
            return Result.Failure<FileMovedToMediaLibraryResultArgs?>("Media file does not exist: " + mediaFile.FilePath);
        }

        // Prüfe, ob der Medienbibliothekspfad angegeben ist
        if (string.IsNullOrWhiteSpace(mediaLibrary))
        {
            return Result.Failure<FileMovedToMediaLibraryResultArgs?>("Media library path is not set.");
        }

        // Prüfe, ob der Medienbibliothekspfad existiert
        if (!Directory.Exists(mediaLibrary))
        {
            return Result.Failure<FileMovedToMediaLibraryResultArgs?>("Media library does not exist: " + mediaLibrary);
        }

        // Leite die Mediengruppen-ID aus dem Dateinamen ab (ignoriert Dateiendung oder definierte Suffixe wie -fanart)
        var mediaGroupId = MediaGroupId.Create(mediaFile.FilePath.FileName.FileNameWithoutExtension);
        if (mediaGroupId.IsFailure)
        {
            return Result.Failure<FileMovedToMediaLibraryResultArgs?>("Error on creating media group ID: " + mediaGroupId.Error);
        }

        // Prüfe anhand der ID ob die Mediengruppe bereits im Infuse Media Library-Verzeichnis existiert
        var mediaGroupQuery = new MediaGroupQuery(mediaLibrary).ById(mediaGroupId.Value);
        var mediaGroup = mediaGroupQuery.Execute();
        if (mediaGroup.IsFailure)
        {
            return Result.Failure<FileMovedToMediaLibraryResultArgs?>("Error on querying media group: " + mediaGroup.Error);
        }

        // Wenn Mediengruppe nicht existiert, dann gib einen leeren Wert zurück
        if (mediaGroup.Value?.DirectoryPath == null)
        {
            return Result.Success<FileMovedToMediaLibraryResultArgs?>(null);
        }

        // Wenn die Mediengruppe existiert, dann verschiebe die Datei in die Mediengruppe
        try
        {
            // Prüfe, ob die Datei bereits im Zielverzeichnis existiert
            var targetFile = new FileInfo(Path.Combine(mediaGroup.Value.DirectoryPath, mediaFile.FilePath.FileName));
            var targetFileAlreadyExists = targetFile.Exists;

            File.Move(mediaFile.FilePath, targetFile.FullName, true);
            var resultArgs = new FileMovedToMediaLibraryResultArgs(targetFile, true, targetFileAlreadyExists);

            return Result.Success<FileMovedToMediaLibraryResultArgs?>(resultArgs);
        }
        catch (Exception e)
        {
            return Result.Failure<FileMovedToMediaLibraryResultArgs?>("Error on moving file to existing media group: " + e.Message);
        }
    }

}

public record FileMovedToMediaLibraryResultArgs(FileInfo FileInfo, bool HasMovedToExistingMediaGroup = false, bool HasTargetFileBeenOverwritten = false);