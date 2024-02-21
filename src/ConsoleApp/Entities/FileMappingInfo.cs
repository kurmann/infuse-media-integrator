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

    private FileMappingInfo(string category, int year, string sourcePath, string targetPath)
    {
        Category = category;
        Year = year;
        SourcePath = sourcePath;
        TargetPath = targetPath;
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

        if (string.IsNullOrWhiteSpace(filePath) || !TryParseFileName(filePath, out var year))
        {
            return Result.Failure<FileMappingInfo>("File name does not match the expected format '{{ISO-Datum}} {{Titel}}.{{Extension}}'.");
        }

        var targetPath = GenerateTargetPath(category, year, filePath);

        return new FileMappingInfo(category, year, filePath, targetPath);
    }

    /// <summary>
    /// Versucht, das Jahr und den sortierten Titel aus dem Dateinamen zu extrahieren.
    /// </summary>
    /// <param name="fileName">Der Dateiname.</param>
    /// <param name="year">Das extrahierte Jahr.</param>
    /// <returns>True, wenn die Extraktion erfolgreich war; andernfalls False.</returns>
    private static bool TryParseFileName(string fileName, out int year)
    {
        year = 0;

        var match = YearMonthAndDateWithFilenameRegex().Match(fileName);
        if (!match.Success)
        {
            return false;
        }

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
        var extension = Path.GetExtension(fileName);

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
