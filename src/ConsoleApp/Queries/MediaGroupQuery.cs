using CSharpFunctionalExtensions;
using Kurmann.InfuseMediaIntegrator.Entities.Elementary;
using Kurmann.InfuseMediaIntegrator.Entities.MediaLibrary;

namespace Kurmann.InfuseMediaIntegrator.Queries;

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

public interface ICanQueryMediaGroup : IQueryService<DirectoryPathInfo>
{
    
}

internal class CanQueryMediaGroup(string mediaLibraryPath, string id) : ICanQueryMediaGroup
{
    private readonly string _id = id;
    private readonly string _mediaLibraryPath = mediaLibraryPath;

    public Result<DirectoryPathInfo> Execute()
    {
        // Prüfe ob das Infuse Media Library-Verzeichnis existiert
        if (!Directory.Exists(_mediaLibraryPath))
        {
            return Result.Failure<DirectoryPathInfo>($"Directory not found: {_mediaLibraryPath}");
        }

        // Prüfe, ob die Mediengruppen-ID valide ist
        var mediaGroupId = MediaGroupId.Create(_id);
        if (mediaGroupId.IsFailure)
        {
            return Result.Failure<DirectoryPathInfo>(mediaGroupId.Error);
        }

        // Suche Dateien, die zur Mediengruppe gehören
        var mediaLibraryQuery = new MediaLibraryQuery(_mediaLibraryPath).ById(mediaGroupId.Value);
        var mediaFiles = mediaLibraryQuery.Execute();

        // Prüfe, ob die Mediendateien alle im gleichen Verzeichnis liegen
        var mediaFilesInSameDirectory = mediaFiles.Value.All(x => x.FilePath.DirectoryPathInfo.DirectoryPath == mediaFiles.Value.First().FilePath.DirectoryPathInfo.DirectoryPath);
        if (!mediaFilesInSameDirectory)
        {
            return Result.Failure<DirectoryPathInfo>("Media group files are not in the same directory. The ID is ambiguous.");
        }

        // Gebe das Verzeichnis zurück, in dem die Mediendateien liegen. Das ist das Verzeichnis der Mediengruppe.
        return Result.Success(mediaFiles.Value.First().FilePath.DirectoryPathInfo);
    }
}