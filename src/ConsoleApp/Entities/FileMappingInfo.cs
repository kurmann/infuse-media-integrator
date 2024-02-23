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
    /// Die Kategorie der Datei.
    /// </summary>
    [Obsolete("Use Categories instead")]
    public string Category { get; }

    /// <summary>
    /// Die Kategorien der Datei (oft nur eine)
    /// </summary>
    public List<string> Categories { get; } = [];

    /// <summary>
    /// Der Titel der Datei.
    /// </summary>
    public string Title { get; }

    /// <summary>
    /// Das Jahr der Datei.
    /// </summary>
    public int Year { get; }

    /// <summary>
    /// Das Suffix, das an den Dateinamen angehängt wird, um das Bild als Fanart zu kennzeichnen.
    /// </summary>
    private const string FanartImagePostfix = "-fanart";  // Infuse erkennt Bilder als Fanart, wenn der Dateiname das Suffix "-fanart" enthält

    /// <summary>
    /// Der Typ der Datei im Sinn von Infuse.
    /// </summary>
    public InfuseMediaType MediaType { get; }

    private FileMappingInfo(string category, int year, string sourcePath, string targetPath, InfuseMediaType mediaType)
    {
        Category = category;
        Year = year;
        SourcePath = sourcePath;
        TargetPath = targetPath;
        MediaType = mediaType;
    }

    private FileMappingInfo(List<string> categories, int year, string title, string sourcePath, string targetPath, InfuseMediaType mediaType)
    {
        Categories = categories ?? [];
        Year = year;
        Title = title;
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

        var targetPath = GenerateTargetPath(category, year, filePath, mediaType);
        if (targetPath.IsFailure)
        {
            return Result.Failure<FileMappingInfo>(targetPath.Error);
        }

        return new FileMappingInfo(category, year, filePath, targetPath.Value, mediaType);
    }

    /// <summary>
    /// Erstellt eine Instanz von FileMappingInfo basierend auf dem Dateipfad und den Metadaten der Datei.
    /// </summary>
    /// <param name="filePath">Der Pfad der Quelldatei.</param>
    /// <param name="metadata">Das FileMetadata-Objekt mit den Metadaten der Datei.</param>
    /// <returns>Ein Result-Objekt, das bei Erfolg eine Instanz von FileMappingInfo enthält.</returns>
    public static Result<FileMappingInfo> Create(string filePath, FileMetadata metadata)
    {
        // Prüfe, ob die Datei existiert
        var fileInfo = new FileInfo(filePath);
        if (!fileInfo.Exists)
        {
            return Result.Failure<FileMappingInfo>($"File '{filePath}' does not exist.");
        }

        // Prüfe, ob aus dem Dateinamen das Verzeichnis abgeleitet werden kann
        if (string.IsNullOrWhiteSpace(fileInfo.Directory?.FullName))
        {
            return Result.Failure<FileMappingInfo>($"Directory of file '{filePath}' does not exist.");
        }

        // Versuche die für das Dateimanagement relevanten Informationen aus den Metadaten oder dem Dateinamen zu lesen
        var title = GetTitle(metadata, fileInfo);
        var categories = GetCategoryInfoOrEmptyList(metadata, fileInfo, fileInfo.Directory);
        var recordedDate = GetRecordedDateOrCurrentDate(metadata, fileInfo);

        var targetPath = GenerateTargetPath(title, categories, recordedDate);
        if (targetPath.IsFailure)
        {
            return Result.Failure<FileMappingInfo>(targetPath.Error);
        }

        return new FileMappingInfo(categories, recordedDate.Year, title, filePath, targetPath.Value, InfuseMediaType.MovieFile);
    }

    /// <summary>
    /// Versucht einen Titel aus den Metadaten oder dem Dateinamen zu lesen.
    /// </summary>
    /// <param name="metadata"></param>
    /// <param name="fileInfo"></param>
    /// <returns></returns>
    private static string GetTitle(FileMetadata metadata, FileInfo fileInfo)
    {
        // Prüfe ob der Titel in den Metadaten gesetzt ist und ob dieser für das Dateisystem gültig ist
        string title;
        if (metadata != null && !string.IsNullOrWhiteSpace(metadata.Title))
        {
            var titleFromMetadata = DirectoryOrFilename.Create(metadata.Title);
            if (titleFromMetadata.IsSuccess)
            {
                title = titleFromMetadata.Value.Name;
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

    /// <summary>
    /// Versucht die Kategorie aus den Metadaten oder dem Dateipfad zu lesen.
    /// </summary>
    /// <param name="metadata"></param>
    /// <param name="filePath"></param>
    /// <param name="rootSourceDirectory"></param>
    /// <returns></returns>
    private static List<string> GetCategoryInfoOrEmptyList(FileMetadata metadata, FileInfo filePath, DirectoryInfo rootSourceDirectory)
    {
        // Wenn die Kategorie in den Metadaten gesetzt ist, dann prüfe ob jede Kategorie einen Namen hat, der als Verzeichnisname gültig ist
        // Die Kateorienlisten werden durch Kommas getrennt
        var categories = new List<string>();
        if (metadata != null && !string.IsNullOrWhiteSpace(metadata.CommaSeparatedCategories))
        {
            var categoryInfoByMetadata = CategoryInfo.CreateFromCommaSeparatedList(metadata.CommaSeparatedCategories);
            if (categoryInfoByMetadata.IsSuccess)
            {
                return categoryInfoByMetadata.Value.Categories;
            }
        }

        // Wenn die Kategorie in den Metadaten nicht gesetzt ist versuche die Kategorie aus dem Dateipfad zu lesen
        var categoryInfoByDirectoryStructure = CategoryInfo.CreateFromDirectoryStructure(rootSourceDirectory.FullName, filePath.DirectoryName);
        if (categoryInfoByDirectoryStructure.IsSuccess)
        {
            return categoryInfoByDirectoryStructure.Value.Categories;
        }

        // Wenn weder in den Metadaten noch im Dateipfad eine Kategorie gefunden wurde, gib eine leere Liste zurück
        return categories;
    }

    /// <summary>
    /// Liest das Aufnahmedatum aus den Metadaten oder dem Dateinamen.
    /// Gibt das aktuelle Datum zurück, wenn weder in den Metadaten noch im Dateinamen ein Aufnahmedatum gefunden wurde.
    /// </summary>
    /// <param name="metadata"></param>
    /// <param name="fileInfo"></param>
    /// <returns></returns>
    private static DateOnly GetRecordedDateOrCurrentDate(FileMetadata metadata, FileInfo fileInfo)
    {
        // Übernimm das Aufnahmedatum aus den Metadaten, wenn es gesetzt ist und sonst versuche es aus dem Dateinamen zu lesen
        DateOnly? recordedDate = metadata.RecordingDate;
        if (metadata.RecordingDate == null)
        {
            // Versuche das Datum aus dem Dateinamen zu lesen
            var recordedDateResult = RecordedDate.CreateFromFilename(fileInfo);
            if (recordedDateResult.IsSuccess)
            {
                recordedDate = recordedDateResult.Value.Value;
            }
        }

        // Wenn weder ein Aufnahmedatum in den Meta-Daten noch im Dateinamen gefunden wurde, dann verwende das aktuelle Datum
        if (recordedDate == null)
        {
            recordedDate = DateOnly.FromDateTime(DateTime.Now);
        }

        // Gib das Aufnahmedatum zurück
        return recordedDate.Value;
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
/// Umfasst Metadaten einer Datei, die einen Einfluss auf das Mapping der Datei haben.
/// </summary>
/// <param name="RecordingDate">Das Aufnahmedatum</param>
/// <param name="CommaSeparatedCategories">Die Kategorien, die durch Kommas getrennt sind</param>
/// <param name="Title">Der Titel</param>
public record FileMetadata(DateOnly? RecordingDate, string CommaSeparatedCategories, string? Title);
