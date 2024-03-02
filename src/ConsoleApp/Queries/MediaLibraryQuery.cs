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

    /// <summary>
    /// Diese Klasse bietet eine Methode zur rekursiven Suche nach Dateien in einem Verzeichnisbaum.
    /// Die Suche basiert auf einem Startverzeichnis und einem Text, mit dem die Dateinamen beginnen sollen.
    /// Wir verwenden die Methode `EnumerateDirectories` aus dem `System.IO`-Namespace, um durch die Verzeichnisse zu iterieren.
    /// </summary>
    /// <remarks>
    /// `EnumerateDirectories` wird gegenüber `GetDirectories` bevorzugt, da es eine effizientere Art der Iteration bietet.
    /// Statt alle Verzeichnispfade sofort in den Speicher zu laden, liefert `EnumerateDirectories` einen Enumerator,
    /// der die Verzeichnisse nach und nach durchläuft. Dies ist besonders nützlich für die Arbeit mit großen Dateisystemen,
    /// da es den Speicherverbrauch reduziert und die Performance verbessert, indem es die Verzeichnisse verzögert lädt.
    /// So kann die Anwendung mit Verzeichnisstrukturen arbeiten, ohne dass der Speicherbedarf stark ansteigt oder die Anwendung verlangsamt wird.
    /// </remarks>
    static IEnumerable<string> SearchFiles(string startDirectory, string searchText)
    {
        Queue<string> directories = new Queue<string>();
        directories.Enqueue(startDirectory);

        while (directories.Count > 0)
        {
            string currentDirectory = directories.Dequeue();
            IEnumerable<string> files;
            try
            {
                files = Directory.EnumerateFiles(currentDirectory, $"{searchText}*", SearchOption.TopDirectoryOnly);
            }
            catch (UnauthorizedAccessException)
            {
                continue;
            }
            catch (DirectoryNotFoundException)
            {
                continue;
            }

            foreach (var file in files)
            {
                yield return file;
            }

            try
            {
                var subdirectories = Directory.EnumerateDirectories(currentDirectory);
                foreach (var subdir in subdirectories)
                {
                    directories.Enqueue(subdir);
                }
            }
            catch (UnauthorizedAccessException)
            {
                continue;
            }
            catch (DirectoryNotFoundException)
            {
                continue;
            }
        }
    }
}
