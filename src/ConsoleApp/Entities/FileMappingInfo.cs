using System.Text.RegularExpressions;
using CSharpFunctionalExtensions;

namespace Kurmann.InfuseMediaIntegrator.Entities;

/// <summary>
/// Definiert ein Mapping von einer Quelldatei zu einem Zielpfad basierend auf den Metadaten der Datei.
/// Die Zuteilung erfolgt anhand gegebenen Informationen. Die Metadaten selbst werden nicht aus der Datei ausgelesen.
/// </summary>
public partial class FileMappingInfo
{
    /// <summary>
    /// Der Pfad der Quelldatei.
    /// </summary>
    public string SourcePath { get; }

    /// <summary>
    /// Der Pfad des Ziels, zu dem die Quelldatei verschoben werden soll.
    /// </summary>
    public string TargetPath { get; }

    /// <summary>
    /// Der Titel der Datei.
    /// </summary>
    public string Title { get; }

    /// <summary>
    /// Der Kategorienpfad bestehehend aus Hauptkategorien und Unterkategorien.
    /// </summary>
    public CategoryPath? Category { get; }

    /// <summary>
    /// Das Jahr der Datei.
    /// </summary>
    [Obsolete("Use RecordingDate.Year instead.")]
    public int Year { get; }

    /// <summary>
    /// Das Aufnahmedatum der Datei.
    /// </summary>
    public DateOnly RecordingDate { get; }

    /// <summary>
    /// Das Suffix, das an den Dateinamen angehängt wird, um das Bild als Fanart zu kennzeichnen.
    /// </summary>
    private const string FanartImagePostfix = "-fanart";  // Infuse erkennt Bilder als Fanart, wenn der Dateiname das Suffix "-fanart" enthält

    /// <summary>
    /// Der Typ der Datei im Sinn von Infuse.
    /// </summary>
    public InfuseMediaType MediaType { get; }

    private FileMappingInfo(string title, CategoryPath category, DateOnly recordingDate, string sourcePath, string targetPath, InfuseMediaType mediaType)
    {
        Title = title;
        Category = category;
        RecordingDate = recordingDate;
        SourcePath = sourcePath;
        TargetPath = targetPath;
        MediaType = mediaType;
    }

    /// <summary>
    /// Erstellt eine Instanz von FileMappingInfo basierend auf der Kategorie und dem Dateinamen.
    /// </summary>
    /// <param name="category">Die Kategorie der Datei.</param>
    /// <param name="filePath">Der Pfad der Quelldatei</param>
    /// <returns>Ein Result-Objekt, das bei Erfolg eine Instanz von FileMappingInfo enthält.</returns>
    [Obsolete("Use Create(FileMappingInfoArgs args) instead.")]
    public static Result<FileMappingInfo> Create(string category, string filePath)
    {
        if (string.IsNullOrWhiteSpace(category))
        {
            return Result.Failure<FileMappingInfo>("Category cannot be null or whitespace.");
        }

        if (string.IsNullOrWhiteSpace(filePath) || !TryParseYear(filePath, out var year, out var mediaType))
        {
            return Result.Failure<FileMappingInfo>("File name does not match the expected format '{{ISO-Datum}} {{Titel}}.{{Extension}}'.");
        }

        var args = new FileMappingInfoArgs
        {
            SourceFilePath = filePath,
            CategoryPath = category
        };
        return Create(args);
    }

    /// <summary>
    /// Erstellt eine Instanz von FileMappingInfo basierend auf dem Dateipfad und den Metadaten der Datei.
    /// </summary>
    /// <param name="filePath">Der Pfad der Quelldatei.</param>
    /// <param name="metadata">Das FileMetadata-Objekt mit den Metadaten der Datei.</param>
    /// <returns>Ein Result-Objekt, das bei Erfolg eine Instanz von FileMappingInfo enthält.</returns>
    public static Result<FileMappingInfo> Create(FileMappingInfoArgs args)
    {
        // Prüfe, ob die Datei existiert
        var fileInfo = new FileInfo(args.SourceFilePath);
        if (!fileInfo.Exists)
        {
            return Result.Failure<FileMappingInfo>($"File '{args.SourceFilePath}' does not exist.");
        }

        // Prüfe, ob aus dem Dateinamen das Verzeichnis abgeleitet werden kann
        if (string.IsNullOrWhiteSpace(fileInfo.Directory?.FullName))
        {
            return Result.Failure<FileMappingInfo>($"Directory of file '{args.SourceFilePath}' cannot be read.");
        }

        // Versuche die für das Dateimanagement relevanten Informationen aus den Metadaten oder dem Dateinamen zu lesen
        var title = GetTitle(args.Title, fileInfo);
        var categories = GetCategoryPathOrEmpty(fileInfo, args.CategoryPath);
        var recordedDate = GetRecordedDateOrCurrentDate(fileInfo, args.RecordingDate);

        var targetPath = GenerateTargetPath(title, recordedDate.Year, fileInfo.Name, InfuseMediaType.MovieFile);
        if (targetPath.IsFailure)
        {
            return Result.Failure<FileMappingInfo>(targetPath.Error);
        }

        return new FileMappingInfo(title, categories, recordedDate, args.SourceFilePath, targetPath.Value, InfuseMediaType.MovieFile);
    }

    /// <summary>
    /// Versucht einen Titel aus den Metadaten oder dem Dateinamen zu lesen.
    /// </summary>
    /// <param name="metadata"></param>
    /// <param name="fileInfo"></param>
    /// <returns></returns>
    private static string GetTitle(string? titleFromMetadata, FileInfo fileInfo)
    {
        // Prüfe ob der Titel in den Metadaten gesetzt ist und ob dieser für das Dateisystem gültig ist
        string title;
        if (!string.IsNullOrWhiteSpace(titleFromMetadata))
        {
            var directoryOrFilename = DirectoryOrFilename.Create(titleFromMetadata);
            if (directoryOrFilename.IsSuccess)
            {
                title = directoryOrFilename.Value.Name;
            }
            else
            {
                title = fileInfo.Name;
            }

            return title;
        }

        // Versuche den Titel aus dem Dateinamen zu lesen, indem das Aufnahmedatum entfernt wird
        var titleWithRecordedDate = TitleWithRecordedDate.Create(fileInfo);
        if (titleWithRecordedDate.IsSuccess)
        {
            title = titleWithRecordedDate.Value.Title;

            // Wenn der Titel aus dem Dateinamen gelesen wurde, dann prüfe ob dieser für das Dateisystem gültig ist
            var titleFromFilename = DirectoryOrFilename.Create(title);
            if (titleFromFilename.IsSuccess)
            {
                title = titleFromFilename.Value.Name;
            }
            else
            {
                title = fileInfo.Name;
            }

            return title;
        }
        else
        {
            title = fileInfo.Name;
        }

        return title;
    }

    private static CategoryPath? GetCategoryPathOrEmpty(FileInfo fileInfo, string? categoryPath)
    {
        // Versuche einen Kategorienpfad aus den Metadaten zu lesen
        var categoryPathFromMetadata = CategoryPath.Create(categoryPath);
        if (categoryPathFromMetadata.IsSuccess)
        {
            return categoryPathFromMetadata.Value;
        }

        // Versuche einen Kategorienpfad aus dem Dateinamen zu lesen
        var categoryPathFromFilename = CategoryPath.Create(fileInfo.Directory?.FullName);
        if (categoryPathFromFilename.IsSuccess)
        {
            return categoryPathFromFilename.Value;
        }

        // Wenn weder in den Metadaten noch im Dateinamen ein Kategorienpfad gefunden wurde, dann gib einen leeren Kategorienpfad zurück
        return null;
    }

    /// <summary>
    /// Liest das Aufnahmedatum aus den Metadaten oder dem Dateinamen.
    /// Gibt das aktuelle Datum zurück, wenn weder in den Metadaten noch im Dateinamen ein Aufnahmedatum gefunden wurde.
    /// </summary>
    /// <param name="metadata"></param>
    /// <param name="fileInfo"></param>
    /// <returns></returns>
    private static DateOnly GetRecordedDateOrCurrentDate(FileInfo fileInfo, DateOnly? recordingDate)
    {
        // Übernimm das Aufnahmedatum aus den Metadaten, wenn es gesetzt
        if (recordingDate != null)
        {
            return recordingDate.Value;
        }

        // Versuche das Datum aus dem Dateinamen zu lesen
        var recordedDateResult = RecordedDate.CreateFromFilename(fileInfo);
        if (recordedDateResult.IsSuccess)
        {
            recordingDate = recordedDateResult.Value.Value;
        }

        // Wenn weder ein Aufnahmedatum in den Meta-Daten noch im Dateinamen gefunden wurde, dann verwende das aktuelle Datum
        if (recordingDate == null)
        {
            recordingDate = DateOnly.FromDateTime(DateTime.Now);
        }

        // Gib das Aufnahmedatum zurück
        return recordingDate.Value;
    }

    /// <summary>
    /// Versucht, das Jahr und den sortierten Titel aus dem Dateinamen zu extrahieren.
    /// </summary>
    /// <param name="fileName">Der Dateiname.</param>
    /// <param name="year">Das extrahierte Jahr.</param>
    /// <param name="mediaType">Der Typ der Datei im Sinn von Infuse.</param>
    /// <returns>True, wenn die Extraktion erfolgreich war; andernfalls False.</returns>
    /// Hinweis: Auch bei JPG-Bildern wird versucht, das Jahr zu extrahieren, weil die JPG-Datei den gleichen Namen wie das Video hat.
    private static bool TryParseYear(string fileName, out int year, out InfuseMediaType mediaType)
    {
        year = 0;
        // Standardwert für den Medientyp ist MovieFile
        mediaType = InfuseMediaType.MovieFile;

        // Prüfen, ob der Dateiname dem erwarteten Format entspricht
        var match = YearMonthAndDateWithFilenameRegex().Match(fileName);
        if (!match.Success)
        {
            return false;
        }

        // Prüft, ob es sich um ein JPG-Bild handelt (berücksichtigte Dateiendungen: jpg, jpeg und berücksichtigt Gross- und Kleinschreibung)
        // Aktuell werden Bilder als FanartImage behandelt, da Infuse das Cover-Bild aus den Metadaten der Videodatei extrahiert.
        if (fileName.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase) || fileName.EndsWith(".jpeg", StringComparison.OrdinalIgnoreCase))
        {
            mediaType = InfuseMediaType.FanartImage;
        }

        // Liest das Jahr aus und prüft, ob es gültig ist
        year = int.Parse(match.Groups["year"].Value);
        if (year < 1900)
        {
            return false;
        }

        return true;
    }

    /// <summary>
    /// Generiert den Zielpfad basierend auf den übergebenen Metadaten und dem Dateinamen.
    /// </summary>
    /// <param name="category">Die Kategorie der Datei.</param>
    /// <param name="year">Das Jahr der Datei.</param>
    /// <param name="sortingTitle">Der sortierte Titel der Datei.</param>
    /// <param name="fileName">Der Name der Quelldatei.</param>
    /// <returns>Der generierte Zielpfad.</returns>
    /// Hinweis: Bei Fanart-Bildern wird das Suffix "-fanart" vor der Dateiendung hinzugefügt.
    /// Hinweis: Beispielpfad: Familie\2024\2024-21-03 Ausflug nach Willisau.m4v
    /// Hinweis: Beispielpfad: Familie\2024\2024-21-03 Ausflug nach Willisau-fanart.jpg
    /// Wenn die JPG-Datei bereits das Suffix "-fanart" enthält, wird es nicht nochmals hinzugefügt.
    private static Result<string> GenerateTargetPath(string category, int year, string fileName, InfuseMediaType mediaType = InfuseMediaType.MovieFile)
    {
        try
        {
            // Bei Fanart-Bildern wird das Suffix "-fanart" vor der Dateiendung hinzugefügt, ausser wenn es bereits am Ende des Dateinamens steht (ohne Erweiterung)
            if (mediaType == InfuseMediaType.FanartImage && !Path.GetFileNameWithoutExtension(fileName).EndsWith(FanartImagePostfix, StringComparison.OrdinalIgnoreCase))
            {
                fileName = Path.GetFileNameWithoutExtension(fileName) + FanartImagePostfix + Path.GetExtension(fileName);
            }

            // Beispoldateipfad: Familie\2024\2024-21-03 Ausflug nach Willisau.m4v
            return Result.Success(Path.Combine(category, year.ToString(), fileName));
        }
        catch (Exception ex)
        {
            return Result.Failure<string>($"Fehler beim Generieren des Zielpfads: {ex.Message}");
        }
    }

        /// <summary>
    /// Generiert den Zielpfad basierend auf den übergebenen Metadaten und dem Dateinamen.
    /// </summary>
    /// <param name="fileInfo">Das FileInfo-Objekt der Quelldatei.</param>
    /// <param name="title">Der Titel der Datei.</param>
    /// <param name="categories">Die Kategorien der Datei.</param>
    /// <param name="recordedDate">Das Aufnahmedatum der Datei.</param>
    /// <returns>Der generierte Zielpfad.</returns>
    /// Hinweis: Bei Fanart-Bildern wird das Suffix "-fanart" vor der Dateiendung hinzugefügt.
    /// Hinweis: Beispielpfad: Familie\2024\2024-21-03 Ausflug nach Willisau.m4v
    /// Hinweis: Beispielpfad: Familie\2024\2024-21-03 Ausflug nach Willisau-fanart.jpg
    /// Hinweis: Mehrere Kategorien führen zu einem Verzeichnisbaum, wobei die Kategorien durch Backslashes getrennt sind.
    /// Wenn die JPG-Datei bereits das Suffix "-fanart" enthält, wird es nicht nochmals hinzugefügt.
    private static Result<string> GenerateTargetPath(string title, List<string> categories, DateOnly recordedDate)
    {
        // erstelle den Dateinamen
        // Hinweis: Der Dateiname muss das Aufnahmedatum als Präfix enthalten, damit die Dateien in Infuse in der richtigen Reihenfolge sortiert werden
        try
        {
            var targetFilePath = Path.Combine(string.Join("/", categories), recordedDate.Year.ToString(), recordedDate.ToString("yyyy-MM-dd") + " " + title);
            return Result.Success(targetFilePath);
        }
        catch (Exception ex)
        {
            return Result.Failure<string>($"Fehler beim Generieren des Zielpfads: {ex.Message}");
        }
    }

    [GeneratedRegex(@"^(?<year>\d{4})(-(?<month>\d{2})(-(?<day>\d{2}))?)? (?<title>.+)\.\w+$")]
    private static partial Regex YearMonthAndDateWithFilenameRegex();
}

public enum InfuseMediaType
{
    /// <summary>
    /// Die Datei ist ein Film.
    /// </summary>
    MovieFile,

    /// <summary>
    /// Die Datei ist ein Titelbild, das als Cover für einen Film verwendet wird und von Infuse als solches erkannt wird.
    /// </summary>
    CoverImage,

    /// <summary>
    /// Die Datei ist ein Hintergrundbild, das von Infuse als solches erkannt wird.
    /// </summary>
    FanartImage,
}

/// <summary>
/// Enthält Informationen, die für das Mapping einer Datei benötigt werden.
/// </summary>
/// <param name="sourceFilePath"></param>
/// <param name="RecordingDate"></param>
/// <param name="CategoryPath"></param>
/// <param name="Title"></param>
public record FileMappingInfoArgs
{
    public string SourceFilePath { get; init; }
    public DateOnly? RecordingDate { get; init; }
    public string? CategoryPath { get; init; }
    public string? Title { get; init; }
}