using CSharpFunctionalExtensions;
using Kurmann.InfuseMediaIntegrator.Entities.Elementary;
using Kurmann.InfuseMediaIntegrator.Entities.MediaFileTypes;

namespace Kurmann.InfuseMediaIntegrator.Entities.MediaLibrary;

/// <summary>
/// Ein Mapping zwischen einer Mediendatei und dem Zielverzeichnis in der Medienbibliothek.
/// </summary>
public class MediaFileLibraryDestinationMapping
{
    public IMediaFileType Source { get; }
    public DirectoryPathInfo TargetDirectory { get; }

    private MediaFileLibraryDestinationMapping(IMediaFileType source, DirectoryPathInfo targetDirectory)
    {
        Source = source;
        TargetDirectory = targetDirectory;
    }

    public static Result<MediaFileLibraryDestinationMapping> Create(string mediaFilePath, string mediaLibraryPath, string? subDirectoryPath)
    {
        // Prüfe ob es eine gültige Mediendatei ist
        var mediaFile = MediaFileTypeDetector.GetMediaFile(mediaFilePath);
        if (mediaFile.IsFailure)
        {
            return Result.Failure<MediaFileLibraryDestinationMapping>(mediaFile.Error);
        }

        // Prüfe ob der Pfad zur Medienbibliothek gültig ist
        var mediaLibrary = DirectoryPathInfo.Create(mediaLibraryPath);
        if (mediaLibrary.IsFailure)
        {
            return Result.Failure<MediaFileLibraryDestinationMapping>(mediaLibrary.Error);
        }

        // Prüfe, ob das Unterverzeichnis gültig ist, wenn es angegeben wurde
        if (subDirectoryPath != null)
        {
            var subDirectory = DirectoryPathInfo.Create(subDirectoryPath);
            if (subDirectory.IsFailure)
            {
                return Result.Failure<MediaFileLibraryDestinationMapping>(subDirectory.Error);
            }
        }

        // Leite die Mediengruppen-ID vom Dateinamen ab
        var mediaGroupId = MediaGroupId.CreateFromFileName(mediaFile.Value.FilePath.FileName);
        if (mediaGroupId.IsFailure)
        {
            return Result.Failure<MediaFileLibraryDestinationMapping>("Error while creating media group ID from file name");
        }

        // Ermittle das Zielverzeichnis anhand der Kombination von Medienbibliothek-Verzeichnis, Unterverzeichnis und Mediengruppen-Verzeichnis (ID)
        var destinationDirectoryPath = subDirectoryPath != null
            ? Path.Combine(mediaLibrary.Value, subDirectoryPath, mediaGroupId.Value)
            : Path.Combine(mediaLibrary.Value, mediaGroupId.Value);

        // Erstelle das Objekt für das Zielverzeichnis
        var destinationDirectory = DirectoryPathInfo.Create(destinationDirectoryPath);
        if (destinationDirectory.IsFailure)
        {
            return Result.Failure<MediaFileLibraryDestinationMapping>(destinationDirectory.Error);
        }

        // Retourniere das Mapping
        return Result.Success(new MediaFileLibraryDestinationMapping(mediaFile.Value, destinationDirectory.Value));
    }
}