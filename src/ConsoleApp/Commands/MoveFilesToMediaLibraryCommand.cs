using CSharpFunctionalExtensions;
using Kurmann.InfuseMediaIntegrator.Entities.Elementary;
using Kurmann.InfuseMediaIntegrator.Entities.MediaFileTypes;
using Kurmann.InfuseMediaIntegrator.Entities.MediaLibrary;
using Kurmann.InfuseMediaIntegrator.Queries;
using Microsoft.Extensions.Logging;

namespace Kurmann.InfuseMediaIntegrator.Commands;

public class MoveFilesToMediaLibraryCommand(ILogger logger)
{
    private readonly ILogger _logger = logger;

    public ICanMoveSingleFile File(string filePath)
    {
        return new CanMoveSingleFile(filePath, _logger);
    }
}

public interface ICanMoveSingleFile
{
    ICanExecuteCommandWithSingleFile To(string destinationPath);
}

internal class CanMoveSingleFile(string filePath, ILogger logger) : ICanMoveSingleFile
{
    private readonly string _filePath = filePath;
    private readonly ILogger _logger = logger;

    public ICanExecuteCommandWithSingleFile To(string destinationPath)
    {
        return new CanExecuteCommandWithSingleFile(_filePath, destinationPath, _logger);
    }
}

public interface ICanExecuteCommandWithSingleFile : ICommand
{

}

internal class CanExecuteCommandWithSingleFile(string filePath, string destinationPath, ILogger logger) : ICanExecuteCommandWithSingleFile
{
    private readonly string _filePath = filePath;
    private readonly string _destinationPath = destinationPath;
    private readonly ILogger _logger = logger;

    public Result Execute()
    {
        _logger.LogInformation($"Moving file from {_filePath} to {_destinationPath}");

        // Prüfe ob das Eingangsverzeichnis existiert
        if (!File.Exists(_filePath))
        {
            return Result.Failure($"File not found: {_filePath}");
        }
        
        // Prüfe ob der Zielpfad korrekt ist
        var destinationPathInfo = DirectoryPathInfo.Create(_destinationPath);
        if (destinationPathInfo.IsFailure)
        {
            return Result.Failure(destinationPathInfo.Error);
        }

        // Prüfe ob das Infuse Media Library-Verzeichnis existiert
        if (!Directory.Exists(destinationPathInfo.Value.DirectoryPath))
        {
            return Result.Failure($"Directory not found: {destinationPathInfo.Value.DirectoryPath}");
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

        // Prüfe ob die Mediengruppe bereits im Infuse Media Library-Verzeichnis existiert
        var mediaGroupQuery = new MediaGroupQuery(_destinationPath).ById(mediaGroupId.Value);
        var mediaGroup = mediaGroupQuery.Execute();
        if (mediaGroup.IsFailure)
        {
            // Warne, dass die Mediengruppe im Infuse Media Library-Verzeichnis nicht gesucht werden konnte
            _logger.LogWarning("Error on searching media group in Infuse Media Library: " + mediaGroup.Error);
            _logger.LogInformation("Moving file to Infuse Media Library ignoring existing media groups");

            // Ermittle die Kategorien-Verzeichnisse anhand Metadaten (sofern vorhanden) oder dem relativen Pfad der Datei gegenüber dem Eingangsverzeichnis

            // Verschiebe in eine neue Mediengruppe
            return MoveFileToNewMediaGroup(mediaFile.Value, destinationPathInfo.Value);
        }
        else
        {
            // Prüfe ob die Mediengruppe existiert
            if (mediaGroup.Value != null)
            {
                // Bewege die Datei in das Infuse Media Library-Verzeichnis
                return MoveFileToExistingMediaGroup(mediaFile.Value, mediaGroup.Value);
            }
        }

        // todo: implementiere das Verschieben in eine bestehende Mediengruppe
        return Result.Success();
    }

    private static Result MoveFileToNewMediaGroup(IMediaFileType mediaFile, DirectoryPathInfo directoryPathInfo)
    {
        // Ermittle die Mediengruppen-ID
        var mediaGroupId = MediaGroupId.CreateFromFileName(mediaFile.FilePath.FileName);
        if (mediaGroupId.IsFailure)
        {
            return Result.Failure(mediaGroupId.Error);
        }
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
}