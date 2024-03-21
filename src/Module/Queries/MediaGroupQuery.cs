using CSharpFunctionalExtensions;
using Kurmann.InfuseMediaIntegrator.Entities.MediaLibrary;

namespace Kurmann.InfuseMediaIntegrator.Module.Queries;

/// <summary>
/// Führt eine Abfrage auf die Mediathek durch und gibt ein Verzeichnis zurück, das eine Mediengruppe darstellt.
/// </summary>
public class MediaGroupQuery(string mediaLibraryPath)
{
    private readonly string _mediaLibraryPath = mediaLibraryPath;

    public ICanQueryMediaGroup ById(string id)
    {
        return new CanQueryMediaGroup(_mediaLibraryPath, id);
    }
}

public interface ICanQueryMediaGroup : IQueryService<MediaGroupDirectory?>
{
    
}

internal class CanQueryMediaGroup(string mediaLibraryPath, string id) : ICanQueryMediaGroup
{
    private readonly string _id = id;
    private readonly string _mediaLibraryPath = mediaLibraryPath;

    public Result<MediaGroupDirectory?> Execute()
    {
        // Prüfe ob das Infuse Media Library-Verzeichnis existiert
        if (!Directory.Exists(_mediaLibraryPath))
        {
            return Result.Failure<MediaGroupDirectory?>($"Library directory not found: {_mediaLibraryPath}");
        }

        // Prüfe, ob die Mediengruppen-ID valide ist
        var mediaGroupId = MediaGroupId.Create(_id);
        if (mediaGroupId.IsFailure)
        {
            return Result.Failure<MediaGroupDirectory?>(mediaGroupId.Error);
        }

        // Suche Dateien, die zur Mediengruppe gehören
        var mediaLibraryQuery = new MediaLibraryQuery(_mediaLibraryPath).ById(mediaGroupId.Value);
        var mediaFiles = mediaLibraryQuery.Execute();
        if (mediaFiles.IsFailure)
        {
            return Result.Failure<MediaGroupDirectory?>(mediaFiles.Error);
        }

        // Gib leer zurück, wenn keine Mediendateien gefunden wurden
        if (mediaFiles.Value.Count == 0)
        {
            return null;
        }

        // Prüfe, ob die Mediendateien alle im gleichen Verzeichnis liegen
        var mediaFilesInSameDirectory = mediaFiles.Value.All(x => x.FilePath.DirectoryPathInfo.DirectoryPath == mediaFiles.Value.First().FilePath.DirectoryPathInfo.DirectoryPath);
        if (!mediaFilesInSameDirectory)
        {
            return Result.Failure<MediaGroupDirectory?>("Media group files are not in the same directory. The ID is ambiguous.");
        }

        // Erstelle das Verzeichnisobjekt
        var directoryPath = mediaFiles.Value.First().FilePath.DirectoryPathInfo;
        var mediaGroupDirectory = MediaGroupDirectory.Create(directoryPath, mediaGroupId.Value);
        if (mediaGroupDirectory.IsFailure)
        {
            return Result.Failure<MediaGroupDirectory?>(mediaGroupDirectory.Error);
        }

        return mediaGroupDirectory.Value;
    }
}