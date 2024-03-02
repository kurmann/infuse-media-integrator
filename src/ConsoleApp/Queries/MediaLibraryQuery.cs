using CSharpFunctionalExtensions;
using Kurmann.InfuseMediaIntegrator.Entities.Elementary;
using Kurmann.InfuseMediaIntegrator.Entities.MediaFileTypes;
using TagLib.Id3v2;

namespace Kurmann.InfuseMediaIntegrator.Queries;

public interface ICanDecideBetweenIdAndSpecificProperties
{
    ICanQuery ById(string id);
    ICanSetProperty ByProperties();
}

public class MediaLibraryQuery(string mediaLibraryPath) : ICanDecideBetweenIdAndSpecificProperties
{
    private string MediaLibraryPath { get; set; } = mediaLibraryPath;

    private MediaLibraryQuery() : this(string.Empty) { }

    public ICanQuery ById(string id)
    {
        return new CanQuery(MediaLibraryPath, id);
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
    public ICanQuery ById(string id)
    {
        return new CanQuery(MediaLibraryPath, id);
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
    ICanQueryMediaLibraryByProperties WithTitle(string title);
    ICanQueryMediaLibraryByProperties WithDate(DateOnly date);
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
            throw new NotImplementedException();
        }
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
            var iMediaFiles = SearchDirectoriesQuery.SearchFiles(mediaLibraryDirectory, id, searchAtBeginningOnly);

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