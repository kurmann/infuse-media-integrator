using CSharpFunctionalExtensions;
using Kurmann.InfuseMediaIntegrator.Entities.Elementary;
using Kurmann.InfuseMediaIntegrator.Entities.MediaLibrary;
using Kurmann.InfuseMediaIntegrator.Queries;
using Microsoft.Extensions.Logging;

namespace Kurmann.InfuseMediaIntegrator.Commands;

public class MoveFileToMediaLibraryCommand
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

    // Allgemeines Ereignis um mitzuteilen, dass eine Datei in die Mediathek verschoben wurde.
    public event EventHandler<FileMovedToMediaLibraryEventArgs>? FileMovedToMediaLibrary;
    protected virtual void OnFileMovedToMediaLibrary(FileMovedToMediaLibraryEventArgs e) => FileMovedToMediaLibrary?.Invoke(this, e);

    public Result Execute()
    {
        Logger?.LogInformation("Moving file from {FilePath} to media library {MediaLibraryPath}", FilePath, MediaLibraryPath);

        // Verschiebe die Datei in ein bestehendes Mediengruppen-Verzeichnis sofern vorhanden
        var existingMediaGroupDirectory = TryMovingToExistingMediaGroup(FilePath, MediaLibraryPath);

        // Wenn die Datei nicht Teil einer bestehenden Mediengruppe ist, dann verschiebe die Datei in eine neue Mediengruppe
        if (existingMediaGroupDirectory.IsFailure)
        {
            // Logge eine Warnung, dass die Abfrage nach einer bestehenden Mediengruppe fehlgeschlagen ist
            Logger?.LogWarning("Error on querying existing media group: {Error}", existingMediaGroupDirectory.Error);
            Logger?.LogInformation("Moving file to new media group.");

            return MoveToNewMediaGroup();
        }

        // Prüfe, ob die Datei bereits in eine existierende Mediengruppe verschoben wurde (mit der Methode TryMovingToExistingMediaGroup) und gib den Erfolg zurück
        if (existingMediaGroupDirectory.Value != null)
        {
            Logger?.LogInformation("File {FilePath} was successfully moved to media group {existingMediaGroupDirectory}", FilePath, existingMediaGroupDirectory.Value);
            return Result.Success();
        }

        // Sonst verschiebe die Datei in eine neue Mediengruppe
        return MoveToNewMediaGroup();

        Result MoveToNewMediaGroup()
        {
            var mediaFileLibraryDestinationMapping = MediaFileLibraryDestinationMapping.Create(FilePath, MediaLibraryPath, SubDirectory);
            if (mediaFileLibraryDestinationMapping.IsFailure)
            {
                return Result.Failure<FileInfo>("Error on creating media file library destination mapping: " + mediaFileLibraryDestinationMapping.Error);
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
                OnFileMovedToMediaLibrary(new FileMovedToMediaLibraryEventArgs(new FileInfo(targetFilePath), false, targetFileAlreadyExists));

                return Result.Success(sourceFile);
            }
            catch (Exception e)
            {
                return Result.Failure<FileInfo>("Error on moving file: " + e.Message);
            }
        }
    }

    /// <summary>
    /// Versucht die Datei in eine bestehende Mediengruppe zu verschieben, falls die Mediengruppe anhand des Dateinamens ermittelt werden kann
    /// und die Mediengruppe im Infuse Media Library-Verzeichnis existiert.
    /// </summary>
    /// <returns></returns>
    private Result<DirectoryPathInfo?> TryMovingToExistingMediaGroup(string FilePath, string MediaLibraryPath)
    {
        // Prüfe, ob der Dateipfad existiert
        if (!File.Exists(FilePath))
        {
            return Result.Failure<DirectoryPathInfo?>("File does not exist: " + FilePath);
        }

        // Leite die Mediengruppen-ID aus dem Dateinamen ab (ignoriert Dateiendung oder definierte Suffixe wie -fanart)
        var mediaGroupId = MediaGroupId.Create(Path.GetFileNameWithoutExtension(FilePath));
        if (mediaGroupId.IsFailure)
        {
            return Result.Failure<DirectoryPathInfo?>("Error on creating media group ID: " + mediaGroupId.Error);
        }
        // Prüfe anhand der ID ob die Mediengruppe bereits im Infuse Media Library-Verzeichnis existiert
        var mediaGroupQuery = new MediaGroupQuery(MediaLibraryPath).ById(mediaGroupId.Value);
        var mediaGroup = mediaGroupQuery.Execute();
        if (mediaGroup.IsFailure)
        {
            return Result.Failure<DirectoryPathInfo?>("Error on querying media group: " + mediaGroup.Error);
        }

        // Wenn Mediengruppe nicht existiert, dann gib einen leeren Wert zurück
        if (mediaGroup.Value?.DirectoryPath == null)
        {
            return Result.Success<DirectoryPathInfo?>(null);
        }

        // Wenn die Mediengruppe existiert, dann verschiebe die Datei in die Mediengruppe
        try
        {
            // Prüfe, ob die Datei bereits im Zielverzeichnis existiert
            var targetFile = new FileInfo(Path.Combine(mediaGroup.Value.DirectoryPath, Path.GetFileName(FilePath)));
            var targetFileAlreadyExists = targetFile.Exists;

            File.Move(FilePath, targetFile.FullName, true);
            OnFileMovedToMediaLibrary(new FileMovedToMediaLibraryEventArgs(targetFile, true, targetFileAlreadyExists));
            return Result.Success<DirectoryPathInfo?>(mediaGroup.Value.DirectoryPath);
        }
        catch (Exception e)
        {
            return Result.Failure<DirectoryPathInfo?>("Error on moving file: " + e.Message);
        }
    }

}

public record FileMovedToMediaLibraryEventArgs(FileInfo FileInfo, bool HasMovedToExistingMediaGroup = false, bool HasTargetFileBeenOverwritten = false);