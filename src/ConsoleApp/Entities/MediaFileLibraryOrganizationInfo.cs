using CSharpFunctionalExtensions;
using Kurmann.InfuseMediaIntegrator.Entities.Elementary;
using Kurmann.InfuseMediaIntegrator.Entities.MediaFileTypes;
using Kurmann.InfuseMediaIntegrator.Entities.MediaLibrary;

namespace Kurmann.InfuseMediaIntegrator.Entities;

/// <summary>
/// Enthält Informationen um eine Mediendatei in einer Mediathek zu organisieren.
/// Nimmt eine Mediendatei entgegen und gibt Informationen zurück, wie die Datei in der Mediathek organisiert werden soll.
/// Die Mediendatei hat Informationen über den Dateityp und die Metadaten.
/// Bei der Organisation wird der Dateityp und die Metadaten berücksichtigt, wobei die Metadaten bevorzugt werden.
/// </summary>
public class MediaFileLibraryOrganizationInfo
{
    /// <summary>
    /// Der Dateiname der Zieldatei.
    /// </summary>
    public FileNameInfo TargetFileName { get; }

    /// <summary>
    /// Das Verzeichnis, in das die Datei verschoben werden soll.
    /// </summary>
    public LibraryFileSubDirectoryPath? TargetSubDirectory { get; }

    private MediaFileLibraryOrganizationInfo(FileNameInfo targetFileName, LibraryFileSubDirectoryPath? targetSubDirectory)
    {
        TargetFileName = targetFileName;
        TargetSubDirectory = targetSubDirectory;
    }

    /// <summary>
    /// Erstellt eine Instanz von MediaFileLibraryOrganizationInfo, wenn die gegebene Mediendatei gültig ist.
    /// Wenn keine Kategorien-Pfad vorhanden ist, wird der Katgeorien-Pfad aus den Metadaten der Mediendatei gelesen.
    /// Wenn keine Metadaten vorhanden sind, wird die Mediendatei in das Root-Verzeichnis der Mediathek verschoben.
    /// </summary>
    /// <param name="mediaFile"></param>
    /// <param name="rootDirectory"></param>
    /// <returns></returns>
    public static Result<MediaFileLibraryOrganizationInfo> Create(IMediaFileType? mediaFile, string? rootDirectory = null)
    {
        // Prüfe, ob die Mediendatei angegeben ist
        if (mediaFile == null)
            return Result.Failure<MediaFileLibraryOrganizationInfo>("Media file is null.");

        // Ermittle den Ziel-Dateinamen
        var targetFileName = GetTargetFileName(mediaFile);
        if (targetFileName.IsFailure)
            return Result.Failure<MediaFileLibraryOrganizationInfo>($"Error on creating target file name: {targetFileName.Error}");
        var targetSubDirectory = GetTargetSubDirectoryFromMetadata(mediaFile).GetValueOrDefault() ?? GetTargetSubDirectoryFromSourcePath(mediaFile, rootDirectory);
        if (targetSubDirectory.IsFailure)
            return Result.Failure<MediaFileLibraryOrganizationInfo>($"Error on creating target subdirectory: {targetSubDirectory.Error}");

        return Result.Success(new MediaFileLibraryOrganizationInfo(targetFileName.Value, targetSubDirectory.Value));
    }

    private static Result<LibraryFileSubDirectoryPath?> GetTargetSubDirectoryFromSourcePath(IMediaFileType mediaFile, string? directoryPath)
    {
        // Wenn kein Kategorien-Pfad angegeben ist, wird der Root-Verzeichnis-Pfad zurückgegeben
        if (directoryPath == null)
            return null;

        var path = LibraryFileSubDirectoryPath.Create(mediaFile.FilePath, directoryPath);
        if (path.IsFailure)
            return Result.Failure<LibraryFileSubDirectoryPath?>($"Error on creating subdirectory path: {path.Error}");

        return path.Value;
    }

    private static Result<LibraryFileSubDirectoryPath> GetTargetSubDirectoryFromMetadata(IMediaFileType mediaFile)
    {
        // Der Kategorien-Pfad ist der Album-Name
        var categoryPathFromMetadata = DirectoryPathInfo.Create(mediaFile.Metadata?.Album);
        if (categoryPathFromMetadata.IsFailure)
            return Result.Failure<LibraryFileSubDirectoryPath>($"Error on reading category path from metadata: {categoryPathFromMetadata.Error}");

        return LibraryFileSubDirectoryPath.Create(categoryPathFromMetadata.Value);
    }

    private static Result<FileNameInfo> GetTargetFileName(IMediaFileType mediaFile)
    {
        // Wenn die Datei ein Bild ist, wird der Dateiname mit dem Präfix "fanart-" zurückgegeben
        if (mediaFile is JpegImage)
        {
            var fileName = LibraryFanartImageFileName.CreateWithAddedFanartPrefix(mediaFile.FilePath.FileName);
            if (fileName.IsFailure)
                return Result.Failure<FileNameInfo>($"Error on creating fanart file name: {fileName.Error}");

            return fileName.Value.FileName;
        }

        // Wenn die Datei kein Bild ist, wird der Dateiname unverändert zurückgegeben
        return mediaFile.FilePath.FileName;
    }
}