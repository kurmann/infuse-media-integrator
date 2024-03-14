using CSharpFunctionalExtensions;
using Kurmann.InfuseMediaIntegrator.Entities.Elementary;
using Kurmann.InfuseMediaIntegrator.Entities.MediaLibrary;
using Kurmann.InfuseMediaIntegrator.Queries;
using Microsoft.Extensions.Logging;

namespace Kurmann.InfuseMediaIntegrator.Commands;

public class MoveFileToMediaLibraryCommand(string filePath, ILogger? logger = null)
{
    private readonly string _filePath = filePath;
    private readonly ILogger? _logger = logger;

    public ICanExecuteOrAddCategoryCommand ToMediaLibrary(string mediaLibraryPath)
    {
        return new CanExecuteOrAddCategoryCommand(_filePath, mediaLibraryPath, _logger);
    }

    // Ereignis um mitzuteilen, wann welche Datei in eine bestehende Mediengruppe verschoben wurde
    public event EventHandler<FileInfo>? FileMovedToExistingMediaGroup;
    protected virtual void OnFileMovedToExistingMediaGroup(FileInfo e) => FileMovedToExistingMediaGroup?.Invoke(this, e);

    // Ereignis um mitzuteilen, wann welche Datei in eine neue Mediengruppe verschoben wurde
    public event EventHandler<FileInfo>? FileMovedToNewMediaGroup;
    protected virtual void OnFileMovedToNewMediaGroup(FileInfo e) => FileMovedToNewMediaGroup?.Invoke(this, e);

}

public interface ICanExecuteOrAddCategoryCommand : ICommand
{
    ICanExecuteOrAddCategoryCommand ToSubDirectory(string subDirectory);
}

public class CanExecuteOrAddCategoryCommand(string filePath, string mediaLibraryPath, ILogger? logger = null)
    : ICanExecuteOrAddCategoryCommand
{
    private readonly ILogger? _logger = logger;
    private string? _subDirectory;

    public Result Execute()
    {
        _logger?.LogInformation($"Moving file from {filePath} to media library {mediaLibraryPath}");

        // Verschiebe die Datei in ein bestehendes Mediengruppen-Verzeichnis sofern vorhanden
        var existingMediaGroupDirectory = TryMovingToExistingMediaGroup(filePath, mediaLibraryPath);

        // Wenn die Datei nicht Teil einer bestehenden Mediengruppe ist, dann verschiebe die Datei in eine neue Mediengruppe
        if (existingMediaGroupDirectory.IsFailure)
        {
            // Logge eine Warnung, dass die Abfrage nach einer bestehenden Mediengruppe fehlgeschlagen ist
            _logger?.LogWarning("Error on querying existing media group: " + existingMediaGroupDirectory.Error);
            _logger?.LogInformation("Moving file to new media group.");

            return MoveToNewMediaGroup();
        }

        // Prüfe, ob die Datei bereits in eine existierende Mediengruppe verschoben wurde (mit der Methode TryMovingToExistingMediaGroup) und gib den Erfolg zurück
        if (existingMediaGroupDirectory.Value != null)
        {
            _logger?.LogInformation($"File {filePath} was successfully moved to media group {existingMediaGroupDirectory.Value}");
            return Result.Success();
        }

        // Sonst verschiebe die Datei in eine neue Mediengruppe
        return MoveToNewMediaGroup();

        Result MoveToNewMediaGroup()
        {
            var mediaFileLibraryDestinationMapping = MediaFileLibraryDestinationMapping.Create(filePath, mediaLibraryPath, _subDirectory);
            if (mediaFileLibraryDestinationMapping.IsFailure)
            {
                return Result.Failure<FileInfo>("Error on creating media file library destination mapping: " + mediaFileLibraryDestinationMapping.Error);
            }

            try
            {
                var sourceFile = new FileInfo(filePath);
                var targetDirectory = new DirectoryInfo(mediaFileLibraryDestinationMapping.Value.TargetDirectory);
                File.Move(filePath, Path.Combine(targetDirectory.FullName, sourceFile.Name));
                return Result.Success(sourceFile);
            }
            catch (Exception e)
            {
                return Result.Failure<FileInfo>("Error on moving file: " + e.Message);
            }
        }
    }

    public ICanExecuteOrAddCategoryCommand ToSubDirectory(string categoryPath)
    {
        _subDirectory = categoryPath;
        return this;
    }

    /// <summary>
    /// Versucht die Datei in eine bestehende Mediengruppe zu verschieben, falls die Mediengruppe anhand des Dateinamens ermittelt werden kann
    /// und die Mediengruppe im Infuse Media Library-Verzeichnis existiert.
    /// </summary>
    /// <returns></returns>
    private Result<DirectoryPathInfo?> TryMovingToExistingMediaGroup(string filePath, string mediaLibraryPath)
    {
        // Prüfe, ob der Dateipfad existiert
        if (!File.Exists(filePath))
        {
            return Result.Failure<DirectoryPathInfo?>("File does not exist: " + filePath);
        }

        // Leite die Mediengruppen-ID aus dem Dateinamen ab (ignoriert Dateiendung oder definierte Suffixe wie -fanart)
        var mediaGroupId = MediaGroupId.Create(filePath);
        if (mediaGroupId.IsFailure)
        {
            return Result.Failure<DirectoryPathInfo?>("Error on creating media group ID: " + mediaGroupId.Error);
        }
        // Prüfe anhand der ID ob die Mediengruppe bereits im Infuse Media Library-Verzeichnis existiert
        var mediaGroupQuery = new MediaGroupQuery(mediaLibraryPath).ById(mediaGroupId.Value);
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
            File.Move(filePath, Path.Combine(mediaGroup.Value.DirectoryPath, Path.GetFileName(filePath)));
            return Result.Success<DirectoryPathInfo?>(mediaGroup.Value.DirectoryPath);
        }
        catch (Exception e)
        {
            return Result.Failure<DirectoryPathInfo?>("Error on moving file: " + e.Message);
        }
    }

}