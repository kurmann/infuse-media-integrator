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

    /// <summary>
    /// Der Modus, wie das Unterverzeichnis abgeleitet wurde.
    /// </summary>
    public SubdirectoryDerivationMode SubdirectoryDerivationMode { get; }

    /// <summary>
    /// Gibt an, ob der Album-Name in den Metadaten vorhanden ist.
    /// Dies ist relevant, wenn der Kategorien-Pfad nicht aus den Metadaten abgeleitet wurde, obwohl Metadaten vorhanden sind.
    /// </summary>
    public bool HasAlbumNameInMetadata { get; }

    private MediaFileLibraryOrganizationInfo(FileNameInfo targetFileName,
                                             LibraryFileSubDirectoryPath? targetSubDirectory,
                                             SubdirectoryDerivationMode subdirectoryDerivationMode,
                                             bool hasAlbumNameInMetadata)      
    {
        TargetFileName = targetFileName;
        TargetSubDirectory = targetSubDirectory;
        SubdirectoryDerivationMode = subdirectoryDerivationMode;
        HasAlbumNameInMetadata = hasAlbumNameInMetadata;
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

        // Prüfe, ob ein Album in den Metadaten vorhanden ist (für die Ableitung des Kategorien-Pfads)
        var hasAlbumNameInMetadata = mediaFile.Metadata?.Album != null;

        // Versuche den Kategorien-Pfad aus den Metadaten zu lesen
        if (hasAlbumNameInMetadata)
        {
            return ProcessAlbumMetadata(mediaFile, rootDirectory, targetFileName, hasAlbumNameInMetadata);
        }

        // Wenn kein Album in den Metadaten vorhanden ist, wird der Kategorien-Pfad aus dem Quell-Verzeichnis abgeleitet
        var targetSubDirectoryFromSourcePath = GetTargetSubDirectoryFromSourcePath(mediaFile, rootDirectory);
        if (targetSubDirectoryFromSourcePath.IsFailure)
        {
            // Teile mit, dass kein Album in den Metadaten vorhanden ist und der Kategorie-Pfad aus dem Quell-Verzeichnis fehlschlägt
            var message = $"No album name in metadata. Error on creating target subdirectory from source path: {targetSubDirectoryFromSourcePath.Error}";
            return Result.Failure<MediaFileLibraryOrganizationInfo>(message);
        }

        // Hier konnte der Kategorien-Pfad aus dem Quell-Verzeichnis abgeleitet werden
        return new MediaFileLibraryOrganizationInfo(targetFileName.Value, targetSubDirectoryFromSourcePath.Value, SubdirectoryDerivationMode.SourcePath, hasAlbumNameInMetadata);

        static Result<MediaFileLibraryOrganizationInfo> ProcessAlbumMetadata(IMediaFileType mediaFile, string? rootDirectory, Result<FileNameInfo> targetFileName, bool hasAlbumNameInMetadata)
        {
            var targetSubDirectory = GetTargetSubDirectoryFromMetadata(mediaFile);
            if (targetSubDirectory.IsFailure)
            {
                // Wenn die Ableitung des Kategorien-Pfads aus den Metadaten fehlschlägt, wird versucht, den Kategorien-Pfad aus dem Quell-Verzeichnis zu lesen
                var targetSubDirectoryFromSourcePath = GetTargetSubDirectoryFromSourcePath(mediaFile, rootDirectory);

                if (targetSubDirectoryFromSourcePath.IsFailure)
                {
                    // Teile in der Fehlermeldung mit, dass der Kategorie-Pfad aus den Metadaten fehlschlägt und der Kategorie-Pfad aus dem Quell-Verzeichnis fehlschlägt
                    var message = $"Error on creating target subdirectory from metadata: {targetSubDirectory.Error}. Error on creating target subdirectory from source path: {targetSubDirectoryFromSourcePath.Error}";
                    return Result.Failure<MediaFileLibraryOrganizationInfo>(message);
                }
                else
                {
                    return new MediaFileLibraryOrganizationInfo(targetFileName.Value, targetSubDirectoryFromSourcePath.Value, SubdirectoryDerivationMode.SourcePath, hasAlbumNameInMetadata);
                }
            }

            // Hier konnte der Kategorien-Pfad aus den Metadaten abgeleitet werden
            return new MediaFileLibraryOrganizationInfo(targetFileName.Value, targetSubDirectory.Value, SubdirectoryDerivationMode.Metadata, hasAlbumNameInMetadata);
        }
    }

    private static Result<LibraryFileSubDirectoryPath?> GetTargetSubDirectoryFromSourcePath(IMediaFileType mediaFile, string? directoryPath)
    {
        // Wenn kein Kategorien-Pfad angegeben ist, wird der Root-Verzeichnis-Pfad zurückgegeben
        if (directoryPath == null)
            return null;

        // Prüfe ob eine Mediendatei angegeben ist
        if (mediaFile == null)
            return Result.Failure<LibraryFileSubDirectoryPath?>("Media file is null.");

        var path = LibraryFileSubDirectoryPath.Create(mediaFile.FilePath, directoryPath);
        if (path.IsFailure)
            return Result.Failure<LibraryFileSubDirectoryPath?>($"Error on creating subdirectory path: {path.Error}");

        return path.Value;
    }

    private static Result<LibraryFileSubDirectoryPath> GetTargetSubDirectoryFromMetadata(IMediaFileType? mediaFile)
    {
        // Der Kategorien-Pfad ist der Album-Name
        // Entferne Leerzeichen am Anfang und Ende des Album-Namens
        var categoryPathFromMetadata = DirectoryPathInfo.CreateTrimmed(mediaFile?.Metadata?.Album);
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

public enum SubdirectoryDerivationMode
{
    Undefinied,
    Metadata,
    SourcePath
}