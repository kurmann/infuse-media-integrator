using System.Text.RegularExpressions;
using CSharpFunctionalExtensions;

namespace Kurmann.InfuseMediaIntegrator.Entities;

/// <summary>
/// Definiert ein Mapping von einer Quelldatei zu einem Zielpfad basierend auf den Metadaten der Datei.
/// Die Metadaten werden aus dem Dateinamen extrahiert, der ein spezifisches Format haben muss.
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
    public string Category { get; }

    /// <summary>
    /// Das Jahr der Datei.
    /// </summary>
    public int Year { get; }

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

        var targetPath = GenerateTargetPath(category, year, filePath);

        return new FileMappingInfo(category, year, filePath, targetPath, mediaType);
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

        // Prüft, ob es sich um ein JPG-Bild handelt (berücksichtigte Dateiendungen: jpg, jpeg mit Gross- und Kleinschreibung)
        // Aktuell werden Bilder als FanartImage behandelt, da Infuse das Cover-Bild aus den Metadaten der Videodatei extrahiert.
        if (Path.GetExtension(fileName) == ".jpg" || Path.GetExtension(fileName) == ".jpeg")
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
    private static string GenerateTargetPath(string category, int year, string fileName)
    {
        // Beispoldateipfad: Familie\2024\2024-21-03 Ausflug nach Willisau.m4v
        return Path.Combine(category, year.ToString(), fileName);
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
