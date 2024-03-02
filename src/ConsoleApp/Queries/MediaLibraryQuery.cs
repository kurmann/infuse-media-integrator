using CSharpFunctionalExtensions;
using Kurmann.InfuseMediaIntegrator.Entities.Elementary;
using Kurmann.InfuseMediaIntegrator.Entities.MediaFileTypes;

namespace Kurmann.InfuseMediaIntegrator.Queries;

public interface ICanDecideBetweenIdAndSpecificProperties
{
    ICanQueryById ById(string id);
    ICanSetProperty ByProperties();
}

public class MediaLibraryQuery(string mediaLibraryPath) : ICanDecideBetweenIdAndSpecificProperties
{
    private string MediaLibraryPath { get; set; } = mediaLibraryPath;

    private MediaLibraryQuery() : this(string.Empty) { }

    public ICanQueryById ById(string id)
    {
        return new CanQueryById(MediaLibraryPath, id);
    }

    public ICanSetProperty ByProperties()
    {
        return new CanSetProperty(MediaLibraryPath);
    }
}

internal class CanDecideBetweenIdAndSpecificProperties(string mediaLibraryPath) : ICanDecideBetweenIdAndSpecificProperties
{
    private string MediaLibraryPath { get; set; } = mediaLibraryPath;

    /// <summary>
    /// Setzt die eindeutige ID des Mediums, nach dem gesucht werden soll.
    /// Besteht aus einem ISO-Datumsformat (YYYY-MM-DD) und einem Titel.
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public ICanQueryById ById(string id)
    {
        return new CanQueryById(MediaLibraryPath, id);
    }

    /// <summary>
    /// Setzt die spezifischen Eigenschaften des Mediums, nach dem gesucht werden soll.
    /// </summary>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public ICanSetProperty ByProperties()
    {
        return new CanSetProperty(MediaLibraryPath);
    }
}

public interface ICanSetProperty
{
    ICanSetProperty WithTitle(string title);
    ICanSetProperty WithDate(DateOnly date);
    ICanQueryMediaLibraryByProperties Query();
}

internal class CanSetProperty : ICanSetProperty
{
    private string MediaLibraryPath { get; set; }
    private string? Title { get; set; }
    private DateOnly? Date { get; set; }

    internal CanSetProperty(string mediaLibraryPath)
    {
        MediaLibraryPath = mediaLibraryPath;
    }

    public ICanSetProperty WithTitle(string title)
    {
        Title = title;
        return this;
    }

    public ICanSetProperty WithDate(DateOnly date)
    {
        Date = date;
        return this;
    }

    public ICanQueryMediaLibraryByProperties Query()
    {
        return new CanQueryMediaLibraryByProperties(MediaLibraryPath, Date, Title);
    }

}

public interface ICanQueryById
{
    Result<List<IMediaFileType>> Execute();
}

internal class CanQueryById(string mediaLibraryPath, string id) : ICanQueryById, IQueryService<List<IMediaFileType>>
{
    private string MediaLibraryPath { get; set; } = mediaLibraryPath;
    private string Id { get; set; } = id;

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
        var searchAtBeginningOnly = true;
        return MediaLibraryQueryBase.SearchMediaLibrary(mediaLibraryDirectory.Value, Id, searchAtBeginningOnly);
    }
}

public interface ICanQueryMediaLibraryByProperties
{
    Result<List<IMediaFileType>> Execute();
}

internal class CanQueryMediaLibraryByProperties(string mediaLibraryPath, DateOnly? date = null, string? title = null) :
    ICanQueryMediaLibraryByProperties, IQueryService<List<IMediaFileType>>
{
    private string MediaLibraryPath { get; set; } = mediaLibraryPath;
    private DateOnly? Date { get; set; } = date;
    private string? Title { get; set; } = title;

    public Result<List<IMediaFileType>> Execute()
    {
        // Prüfen, ob das Medienverzeichnis angegeben ist.
        if (string.IsNullOrWhiteSpace(MediaLibraryPath))
            return Result.Failure<List<IMediaFileType>>("Media library path is empty.");

        // Prüfe, ob das Verzeichnis gültig ist
        var mediaLibraryDirectory = DirectoryPathInfo.Create(MediaLibraryPath);
        if (mediaLibraryDirectory.IsFailure)
            return Result.Failure<List<IMediaFileType>>($"An error occurred while searching the media library: {mediaLibraryDirectory.Error}");

        // Prüfe ob Datum oder Titel angegeben sind.
        if (Date is null && string.IsNullOrWhiteSpace(Title))
            return Result.Failure<List<IMediaFileType>>("Date and title are empty. At least one of them must be set.");

        // Wenn beide Eigenschaften angegeben sind, suche nach beiden.
        if (Date is not null && !string.IsNullOrWhiteSpace(Title))
        {
            // Suche nach Medien-Dateien im Medienverzeichnis.
            var dateSearchText = Date.Value.ToString("yyyy-MM-dd");
            var titleSearchText = Title;
            var andSearchText = new List<string> { dateSearchText, titleSearchText };

            // Suche nach Medien-Dateien im Medienverzeichnis.
            var searchResult = SearchDirectoriesQuery.SearchFiles(mediaLibraryDirectory.Value, andSearchText);

            // Gib die gefundenen Medien-Dateien zurück.
            return MediaFileTypeDetector.GetMediaFiles(searchResult);
        }

        // Das Datum wird nur am Anfang des Dateinamens gesucht.
        if (Date is not null)
        {
            // Suche nach Medien-Dateien im Medienverzeichnis.
            var dateSearchText = Date.Value.ToString("yyyy-MM-dd");
            var dateSearchResult = SearchDirectoriesQuery.SearchFiles(mediaLibraryDirectory.Value, dateSearchText, true);

            // Gib die gefundenen Medien-Dateien zurück.
            return MediaFileTypeDetector.GetMediaFiles(dateSearchResult);
        }

        // Der Titel wird im Dateinamen gesucht.
        if (!string.IsNullOrWhiteSpace(Title))
        {
            // Suche nach Medien-Dateien im Medienverzeichnis.
            var titleSearchResult = SearchDirectoriesQuery.SearchFiles(mediaLibraryDirectory.Value, Title, false);

            // Gib die gefundenen Medien-Dateien zurück.
            return MediaFileTypeDetector.GetMediaFiles(titleSearchResult);
        }

        // Wenn keine der Bedingungen zutrifft, gib eine leere Liste zurück.
        return new List<IMediaFileType>();
    }
}

internal abstract class MediaLibraryQueryBase
{
    /// <summary>
    /// Sucht nach Medien-Dateien im Medienverzeichnis.
    /// </summary>
    /// <param name="mediaLibraryDirectory"></param>
    /// <param name="id"></param>
    /// <param name="searchAtBeginningOnly"></param>
    /// <returns></returns>
    internal static Result<List<IMediaFileType>> SearchMediaLibrary(DirectoryPathInfo? mediaLibraryDirectory, string? id, bool searchAtBeginningOnly = false)
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
            // Suche nach Medien-Dateien im Medienverzeichnis.
            var iMediaFiles = SearchDirectoriesQuery.SearchFiles(mediaLibraryDirectory, id, searchAtBeginningOnly);

            // Gib die gefundenen Medien-Dateien zurück.
            return MediaFileTypeDetector.GetMediaFiles(iMediaFiles);
        }
        catch (Exception ex)
        {
            return Result.Failure<List<IMediaFileType>>($"An error occurred while searching the media library: {ex.Message}");
        }
    }
}