using CSharpFunctionalExtensions;
using Kurmann.InfuseMediaIntegrator.Entities.Elementary;
using Kurmann.InfuseMediaIntegrator.Entities.MediaFileTypes;

namespace Kurmann.InfuseMediaIntegrator.Queries;

public class MediaLibraryQuery(string mediaLibraryPath) : ICanDecideBetweenIdAndSpecificProperties
{
    private string MediaLibraryPath { get; set; } = mediaLibraryPath;

    private MediaLibraryQuery() : this(string.Empty) { }

    ICanQueryMediaLibraryById ICanDecideBetweenIdAndSpecificProperties.ById(string id)
    {
        return new CanQueryMediaLibraryById(MediaLibraryPath, id);
    }

    ICanSetProperty ICanDecideBetweenIdAndSpecificProperties.ByProperties()
    {
        return new CanSetProperty(MediaLibraryPath);
    }
}

public interface ICanDecideBetweenIdAndSpecificProperties
{
    ICanQueryMediaLibraryById ById(string id);
    ICanSetProperty ByProperties();
}

internal class CanDecideBetweenIdAndSpecificProperties(string mediaLibraryPath) : ICanDecideBetweenIdAndSpecificProperties
{
    private string MediaLibraryPath { get; set; } = mediaLibraryPath;

    private string? Id { get; set; }

    /// <summary>
    /// Setzt die eindeutige ID des Mediums, nach dem gesucht werden soll.
    /// Besteht aus einem ISO-Datumsformat (YYYY-MM-DD) und einem Titel.
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public ICanQueryMediaLibraryById ById(string id)
    {
        Id = id;
        return new CanQueryMediaLibraryById(MediaLibraryPath, Id);
    }

    /// <summary>
    /// Setzt die spezifischen Eigenschaften des Mediums, nach dem gesucht werden soll.
    /// </summary>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public ICanSetProperty ByProperties()
    {
        throw new NotImplementedException();
    }
}

public interface ICanSetProperty
{
    ICanQueryMediaLibraryByProperties WithTitle(string title);
    ICanQueryMediaLibraryByProperties WithDate(DateOnly date);
}

internal class CanSetProperty : ICanSetProperty
{
    private string MediaLibraryPath { get; set; }

    internal CanSetProperty(string mediaLibraryPath)
    {
        MediaLibraryPath = mediaLibraryPath;
    }

    public ICanQueryMediaLibraryByProperties WithTitle(string title)
    {
        return new CanQueryMediaLibraryByProperties(MediaLibraryPath, null, title);
    }

    public ICanQueryMediaLibraryByProperties WithDate(DateOnly date)
    {
        return new CanQueryMediaLibraryByProperties(MediaLibraryPath, date, null);
    }
}

public interface ICanQueryMediaLibraryById
{
    Result<List<IMediaFileType>> Execute();
}

internal class CanQueryMediaLibraryById : ICanQueryMediaLibraryById, IQueryService<List<IMediaFileType>>
{
    private string MediaLibraryPath { get; set; }
    private string Id { get; set; }

    public CanQueryMediaLibraryById(string mediaLibraryPath, string id)
    {
        MediaLibraryPath = mediaLibraryPath;
        Id = id;
    }

    public Result<List<IMediaFileType>> Execute()
    {
        // Prüfen, ob das Medienverzeichnis angegeben ist.
        if (string.IsNullOrWhiteSpace(MediaLibraryPath))
            return Result.Failure<List<IMediaFileType>>("Media library path is empty.");

        // Prüfe, ob das Verzeichnis gültig ist
        var mediaLibraryDirectory = DirectoryPathInfo.Create(MediaLibraryPath);
        if (mediaLibraryDirectory.IsFailure)
            return Result.Failure<List<IMediaFileType>>($"An error occurred while searching the media library: {mediaLibraryDirectory.Error}");

        // Prüfen, ob die ID angegeben ist.
        if (string.IsNullOrWhiteSpace(Id))
            return Result.Failure<List<IMediaFileType>>("ID is empty.");

        // Suche nach Medien-Dateien im Medienverzeichnis.
        return SearchMediaLibrary(mediaLibraryDirectory.Value, Id);
    }

    private static Result<List<IMediaFileType>> SearchMediaLibrary(DirectoryPathInfo? mediaLibraryDirectory, string? id)
    {
        // Prüfen, ob das Medienverzeichnis existiert.
        if (mediaLibraryDirectory is null)
            return Result.Failure<List<IMediaFileType>>("Media library directory does not exist.");

        // Wenn keine ID angegeben ist, gib eine leere Liste zurück.
        if (id is null)
            return new List<IMediaFileType>();

        // Suche nach Medien-Dateien im Medienverzeichnis.
        try
        {
            var iMediaFiles = SearchDirectoriesQuery.SearchFiles(mediaLibraryDirectory, id);

            // Erstelle eine Liste von Medien-Dateien.
            var mediaFiles = new List<IMediaFileType>();

            // Initialisiere die Medien-Dateien und füge sie als Ergebnis hinzu.
            foreach (var iMediaFile in iMediaFiles)
            {
                var mediaFile = MediaFileTypeDetector.GetMediaFile(iMediaFile);
                if (mediaFile.IsFailure)
                {
                    return Result.Failure<List<IMediaFileType>>($"An error occurred while searching the media library: {mediaFile.Error}");
                }
                else
                {
                    mediaFiles.Add(mediaFile.Value);
                }
            }

            // Gib die Liste der Medien-Dateien zurück.
            return mediaFiles;
        }
        catch (Exception ex)
        {
            return Result.Failure<List<IMediaFileType>>($"An error occurred while searching the media library: {ex.Message}");
        }
    }
}

public interface ICanQueryMediaLibraryByProperties
{
    Result<List<IMediaFileType>> Execute();
}

internal class CanQueryMediaLibraryByProperties : ICanQueryMediaLibraryByProperties, IQueryService<List<IMediaFileType>>
{
    private string MediaLibraryPath { get; set; }
    private DateOnly? Date { get; set; }
    private string? Title { get; set; }

    public CanQueryMediaLibraryByProperties(string mediaLibraryPath, DateOnly? date = null, string? title = null)
    {
        MediaLibraryPath = mediaLibraryPath;
        Date = date;
        Title = title;
    }

    public Result<List<IMediaFileType>> Execute()
    {
        throw new NotImplementedException();
    }
}