using CSharpFunctionalExtensions;
using Kurmann.InfuseMediaIntegrator.Entities.Elementary;
using Kurmann.InfuseMediaIntegrator.Entities.MediaFileTypes;

namespace Kurmann.InfuseMediaIntegrator.Queries;

public class MediaLibraryQuery(string mediaLibraryPath) : IQueryService<List<IMediaFileType>>
{
    private string MediaLibraryPath { get; set; } = mediaLibraryPath;
    private string? Id { get; set; }
    private MediaLibrarySpecificProperties? SpecificProperties { get; set; }

    /// <summary>
    /// Führt die Abfrage aus.
    /// </summary>
    /// <returns></returns>
    public Result<List<IMediaFileType>> Execute()
    {
        // Prüfen, ob das Medienverzeichnis angegeben ist.
        if (string.IsNullOrWhiteSpace(MediaLibraryPath))
            return Result.Failure<List<IMediaFileType>>("Media library path is empty.");

        // Prüfe, ob das Verzeichnis gültig ist
            var mediaLibraryDirectory = DirectoryPathInfo.Create(MediaLibraryPath);
            if (mediaLibraryDirectory.IsFailure)
                return Result.Failure<List<IMediaFileType>>($"An error occurred while searching the media library: {mediaLibraryDirectory.Error}");

        // Wenn eine ID angegeben ist, suche nach Medien-Dateien im Medienverzeichnis.
        if (!string.IsNullOrWhiteSpace(Id))
        {
            // Suche nach Medien-Dateien im Medienverzeichnis.
            return SearchMediaLibrary(mediaLibraryDirectory.Value, Id);
        }

        // Wenn spezifische Eigenschaften angegeben sind, suche nach Medien-Dateien im Medienverzeichnis.
        if (SpecificProperties is not null)
        {
        }

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

    public MediaLibrarySpecificProperties Specific()
    {
        SpecificProperties = new MediaLibrarySpecificProperties();
        return SpecificProperties;
    }

    private static Result<List<IMediaFileType>> SearchMediaLibrary(DirectoryPathInfo? mediaLibraryDirectory, DateOnly dateOnly)
    {
        // Suche nach Medien-Dateien im Medienverzeichnis.
        return SearchMediaLibrary(mediaLibraryDirectory, dateOnly.ToString());
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
            var iMediaFiles = SearchFiles(mediaLibraryDirectory, id);

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
    /// <exception cref="ArgumentNullException">Das Startverzeichnis ist `null` oder leer.</exception>
    /// <exception cref="ArgumentException">Das Startverzeichnis ist nicht vorhanden oder nicht zugänglich.</exception>
    /// <exception cref="UnauthorizedAccessException">Der Zugriff auf das Startverzeichnis wurde verweigert.</exception>
    /// <exception cref="DirectoryNotFoundException">Das Startverzeichnis wurde nicht gefunden.</exception>
    static IEnumerable<string> SearchFiles(string startDirectory, string searchText)
    {
        Queue<string> directories = new();
        directories.Enqueue(startDirectory);

        while (directories.Count > 0)
        {
            string currentDirectory = directories.Dequeue();

            foreach (var file in Directory.EnumerateFiles(currentDirectory, $"{searchText}*", SearchOption.TopDirectoryOnly))
            {
                yield return file;
            }

            foreach (var subdir in Directory.EnumerateDirectories(currentDirectory))
            {
                directories.Enqueue(subdir);
            }
        }
    }
}

public class MediaLibrarySpecificProperties
{
    private string? Title { get; set; }
    private DateOnly? Date { get; set; }

    /// <summary>
    /// Setzt den Titel des Mediums nach dem gesucht werden soll.
    /// </summary>
    /// <param name="title"></param>
    /// <returns></returns>
    public void WithTitle(string title)
    {
        Title = title;
    }

    /// <summary>
    /// Setzt das Aufnahmedatum des Mediums nach dem gesucht werden soll.
    /// </summary>
    /// <param name="date"></param>
    /// <returns></returns>
    public void WithDate(DateOnly date)
    {
        Date = date;
    }
}
