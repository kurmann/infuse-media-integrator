using CSharpFunctionalExtensions;
using Kurmann.InfuseMediaIntegrator.Entities.MediaFileTypes;

namespace Kurmann.InfuseMediaIntegrator.Queries;

/// <summary>
/// Liefert Medien-Dateien aus dem Medienverzeichnis.
/// </summary>
/// <param name="mediaLibraryPath"></param>
public class MediaLibraryQuery(string mediaLibraryPath) : IQueryService<List<IMediaFileType>>
{
    public string MediaLibraryPath { get; } = mediaLibraryPath;

    /// <summary>
    /// Die eindeutige ID des Medienverzeichnisses.
    /// Besteht aus einem ISO-Datumsformat (YYYY-MM-DD) und einem Titel.
    /// Schema: {YYYY-MM-DD} {Title}
    /// Entspricht in den meisten Fällen dem Dateinamen des Hauptmediums (Video) ohne Dateiendung.
    /// </summary>
    public string? Id { get; private set; }

    /// <summary>
    /// Das Aufnahmedatum nach ISO-8601.
    /// </summary>
    public string? Date { get; private set; }

    /// <summary>
    /// Der Titel des Medienverzeichnisses.
    /// </summary>
    public string? Title { get; private set; }

    /// <summary>
    /// Führt die Abfrage aus.
    /// </summary>
    /// <returns></returns>
    public Result<List<IMediaFileType>> Execute()
    {
        if (string.IsNullOrWhiteSpace(MediaLibraryPath))
            return Result.Failure<List<IMediaFileType>>("Media library path is empty.");

        return new Result<List<IMediaFileType>>();
    }

    /// <summary>
    /// Setzt die eindeutige ID des Mediums, nach dem gesucht werden soll.
    /// Besteht aus einem ISO-Datumsformat (YYYY-MM-DD) und einem Titel.
    /// Schema: {YYYY-MM-DD} {Title}
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public MediaLibraryQuery WithId(string id)
    {
        Id = id;
        return this;
    }

    /// <summary>
    /// Setzt den Titel des Mediums nach dem gesucht werden soll.
    /// </summary>
    /// <param name="title"></param>
    /// <returns></returns>
    public MediaLibraryQuery WithTitle(string title)
    {
        Title = title;
        return this;
    }

    /// <summary>
    /// Setzt das Aufnahmedatum des Mediums nach dem gesucht werden soll.
    /// </summary>
    /// <param name="date"></param>
    /// <returns></returns>
    public MediaLibraryQuery WithDate(string date)
    {
        Date = date;
        return this;
    }
}
