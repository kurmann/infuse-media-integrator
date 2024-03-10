using CSharpFunctionalExtensions;
using Kurmann.InfuseMediaIntegrator.Entities.Elementary;
using Kurmann.InfuseMediaIntegrator.Entities.MediaFileTypes;
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
}

public interface ICanExecuteOrAddCategoryCommand : ICommand
{
    ICanExecuteOrAddCategoryCommand ToSubDirectory(string subDirectory);
}

public class CanExecuteOrAddCategoryCommand(string filePath, string mediaLibraryPath, ILogger? logger = null)
    : ICanExecuteOrAddCategoryCommand
{
    private readonly string _filePath = filePath;
    private readonly string _mediaLibraryPath = mediaLibraryPath;
    private readonly ILogger? _logger = logger;
    private string? _subDirectory;

    public Result Execute()
    {
        _logger?.LogInformation($"Moving file from {_filePath} to media library {_mediaLibraryPath}");

        // Prüfe ob das Eingangsverzeichnis existiert
        if (!File.Exists(_filePath))
        {
            return Result.Failure($"File not found: {_filePath}");
        }

        // Prüfe ob der Zielpfad korrekt ist
        var mediaLibraryPathInfo = DirectoryPathInfo.Create(_mediaLibraryPath);
        if (mediaLibraryPathInfo.IsFailure)
        {
            return Result.Failure(mediaLibraryPathInfo.Error);
        }

        // Prüfe ob das Infuse Media Library-Verzeichnis existiert
        if (!Directory.Exists(mediaLibraryPathInfo.Value.DirectoryPath))
        {
            return Result.Failure($"Directory not found: {mediaLibraryPathInfo.Value.DirectoryPath}");
        }

        // Sammle Informationen über die Datei
        var mediaFile = MediaFileTypeDetector.GetMediaFile(_filePath);
        if (mediaFile.IsFailure)
        {
            return Result.Failure(mediaFile.Error);
        }

        // Leite die Mediengruppen-ID vom Dateinamen ab
        var mediaGroupId = MediaGroupId.Create(mediaFile.Value.FilePath);
        if (mediaGroupId.IsFailure)
        {
            return Result.Failure("Error on deriving media group ID from file name: " + mediaGroupId.Error);
        }

        // Prüfe, ob das Unterverzeichnis gültig ist, falls es definiert ist
        DirectoryPathInfo? subDirectoryPathInfo = null;
        if (_subDirectory != null)
        {
            var subDirectoryPathInfoResult = DirectoryPathInfo.Create(_subDirectory);
            if (subDirectoryPathInfoResult.IsFailure)
            {
                // Logge eine Warnung, dass das Unterverzeichnis nicht gültig ist und informiere, dass es ignoriert wird
                _logger?.LogWarning("Subdirectory is not valid: " + subDirectoryPathInfoResult.Error);
                _logger?.LogInformation("Moving file to Infuse Media Library ignoring subdirectory");
            }
            else
            {
                subDirectoryPathInfo = subDirectoryPathInfoResult.Value;
            }
        }

        // Prüfe ob die Mediengruppe bereits im Infuse Media Library-Verzeichnis existiert
        var mediaGroupQuery = new MediaGroupQuery(mediaLibraryPathInfo.Value).ById(mediaGroupId.Value);
        var mediaGroup = mediaGroupQuery.Execute();
        if (mediaGroup.IsFailure)
        {
            // Warne, dass die Mediengruppe im Infuse Media Library-Verzeichnis nicht gesucht werden konnte
            _logger?.LogWarning("Error on searching media group in Infuse Media Library: " + mediaGroup.Error);
            _logger?.LogInformation("Moving file to Infuse Media Library ignoring existing media groups");

            // Verschiebe in eine neue Mediengruppe
            return MoveFileToNewMediaGroup(mediaFile.Value, mediaGroupId.Value, subDirectoryPathInfo, mediaLibraryPathInfo.Value);       
        }

        // Falls die Mediengruppe gefunden wurde, verschiebe die Datei in die Mediengruppe
        if (mediaGroup.Value != null)
        {
            return MoveFileToExistingMediaGroup(mediaFile.Value, mediaGroup.Value);
        }

        // Verschiebe in eine neue Mediengruppe
        return MoveFileToNewMediaGroup(mediaFile.Value, mediaGroupId.Value, subDirectoryPathInfo, mediaLibraryPathInfo.Value);
    }

    private static Result MoveFileToExistingMediaGroup(IMediaFileType mediaFile, MediaGroupDirectory mediaGroupDirectory)
    {
        // Bewege die Datei in das Infuse Media Library-Verzeichnis
        var destinationFilePath = Path.Combine(mediaGroupDirectory.DirectoryPath, mediaFile.FilePath);
        try
        {
            File.Move(mediaFile.FilePath, destinationFilePath);
            return Result.Success();
        }
        catch (Exception e)
        {
            return Result.Failure("Error on moving file: " + e.Message);
        }
    }

    /// <summary>
    /// Verschiebt die Datei in eine neue Mediengruppe
    /// Die Mediengruppe ist Teil des Unterverzeichnisses, das wiederum Teil des Infuse Media Library-Verzeichnisses ist
    /// </summary>
    /// <param name="mediaFile"></param>
    /// <param name="mediaGroupDirectory"></param>
    /// <param name="subDirectory"></param>
    /// <param name="mediaLibraryPathInfo"></param>
    /// <returns></returns>
    private static Result MoveFileToNewMediaGroup(IMediaFileType mediaFile, MediaGroupId mediaGroupId, DirectoryPathInfo? subDirectory, DirectoryPathInfo mediaLibraryPathInfo)
    {
        // Ermittle das Zielverzeichnis anhand der Kombination von Medienbibliothek-Verzeichnis, Unterverzeichnis und Mediengruppen-Verzeichnis (ID)
        var destinationDirectoryPath = subDirectory != null
            ? Path.Combine(mediaLibraryPathInfo.DirectoryPath, subDirectory.DirectoryPath, mediaGroupId)
            : Path.Combine(mediaLibraryPathInfo.DirectoryPath, mediaGroupId);

        // Erstelle das Zielverzeichnis falls es nicht existiert
        if (!Directory.Exists(destinationDirectoryPath))
        {
            Directory.CreateDirectory(destinationDirectoryPath);
        }

        // Bewege die Datei in das Infuse Media Library-Verzeichnis
        var destinationFilePath = Path.Combine(destinationDirectoryPath, mediaFile.FilePath);
        try
        {
            File.Move(mediaFile.FilePath, destinationFilePath);
            return Result.Success();
        }
        catch (Exception e)
        {
            return Result.Failure("Error on moving file: " + e.Message);
        }
    }

    public ICanExecuteOrAddCategoryCommand ToSubDirectory(string categoryPath)
    {
        _subDirectory = categoryPath;
        return this;
    }

}