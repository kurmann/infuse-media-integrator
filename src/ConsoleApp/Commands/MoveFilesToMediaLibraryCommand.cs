using CSharpFunctionalExtensions;
using Kurmann.InfuseMediaIntegrator.Entities.MediaFileTypes;
using Kurmann.InfuseMediaIntegrator.Queries;
using Microsoft.Extensions.Logging;

namespace Kurmann.InfuseMediaIntegrator.Commands;

public class MoveFilesToInfuseMediaLibraryCommand(ILogger logger)
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

        // Prüfe ob das Infuse Media Library-Verzeichnis existiert
        if (!Directory.Exists(_destinationPath))
        {
            return Result.Failure($"Directory not found: {_destinationPath}");
        }

        // Sammle Informationen über die Datei
        var mediaFile = MediaFileTypeDetector.GetMediaFile(_filePath);
        if (mediaFile.IsFailure)
        {
            return Result.Failure(mediaFile.Error);
        }

        // Prüfe, ob die Mediengruppe bereits in der Medienbibliothek vorhanden ist
        var query = new MediaLibraryQuery(_destinationPath).ById(mediaFile.Value.FilePath.FileName);
        var queryResult = query.Execute();
        if (queryResult.IsFailure)
        {
            // Bei einem Fehler wird angenommen, dass die Mediengruppe nicht vorhanden ist.
            // Warne, dass die Abfrage fehlgeschlagen ist und fahre fort.
            _logger.LogWarning("Error on querying media library: " + queryResult.Error);
            _logger.LogInformation("File will be moved to media library.");
        }

        // todo: implementiere das Verschieben in eine bestehende Mediengruppe
        return Result.Success();
    }

    private static Result MoveFileToExistingMediaGroup(string filePath, string destinationPath, ILogger logger)
    {
        // Bewege die Datei in das Infuse Media Library-Verzeichnis
        var destinationFilePath = Path.Combine(destinationPath, Path.GetFileName(filePath));
        try
        {
            File.Move(filePath, destinationFilePath);
            logger.LogInformation($"File moved to {destinationFilePath}");
            return Result.Success();
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error on moving file");
            return Result.Failure("Error on moving file: " + e.Message);
        }
    }
}