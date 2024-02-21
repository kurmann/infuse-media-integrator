using System.Text.RegularExpressions;
using CSharpFunctionalExtensions;

namespace Kurmann.InfuseMediaIntegrator.Entities;

/// <summary>
/// Definiert ein Mapping von einer Quelldatei zu einem Zielpfad basierend auf den Metadaten der Datei.
/// Die Metadaten werden aus dem Dateinamen extrahiert, der ein spezifisches Format haben muss.
/// </summary>
public partial class FileMappingInfo
{
    public string Category { get; }
    public int Year { get; }
    public string SortingTitle { get; }
    public string TargetPath { get; }

    private FileMappingInfo(string category, int year, string sortingTitle, string targetPath)
    {
        Category = category;
        Year = year;
        SortingTitle = sortingTitle;
        TargetPath = targetPath;
    }

    /// <summary>
    /// Erstellt eine Instanz von FileMappingInfo basierend auf der Kategorie und dem Dateinamen.
    /// </summary>
    /// <param name="category">Die Kategorie der Datei.</param>
    /// <param name="fileName">Der Name der Quelldatei, der dem Format '{{ISO-Datum}} {{Titel}}.{{Extension}}' entsprechen muss.</param>
    /// <returns>Ein Result-Objekt, das bei Erfolg eine Instanz von FileMappingInfo enthält.</returns>
    public static Result<FileMappingInfo> Create(string category, string fileName)
    {
        if (string.IsNullOrWhiteSpace(category))
        {
            return Result.Failure<FileMappingInfo>("Category cannot be null or whitespace.");
        }

        if (string.IsNullOrWhiteSpace(fileName) || !TryParseFileName(fileName, out var year, out var sortingTitle))
        {
            return Result.Failure<FileMappingInfo>("File name does not match the expected format '{{ISO-Datum}} {{Titel}}.{{Extension}}'.");
        }

        var targetPath = GenerateTargetPath(category, year, sortingTitle, fileName);

        return new FileMappingInfo(category, year, sortingTitle, targetPath);
    }

    /// <summary>
    /// Versucht, das Jahr und den sortierten Titel aus dem Dateinamen zu extrahieren.
    /// </summary>
    /// <param name="fileName">Der Dateiname.</param>
    /// <param name="year">Das extrahierte Jahr.</param>
    /// <param name="sortingTitle">Der extrahierte sortierte Titel.</param>
    /// <returns>True, wenn die Extraktion erfolgreich war; andernfalls False.</returns>
    private static bool TryParseFileName(string fileName, out int year, out string? sortingTitle)
    {
        year = 0;
        sortingTitle = null;

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

        sortingTitle = match.Groups["title"].Value;

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
    private static string GenerateTargetPath(string category, int year, string sortingTitle, string fileName)
    {
        var extension = Path.GetExtension(fileName);
        return Path.Combine(category, year.ToString(), $"{sortingTitle}{extension}");
    }

    [GeneratedRegex(@"^(?<year>\d{4})(-(?<month>\d{2})(-(?<day>\d{2}))?)? (?<title>.+)\.\w+$")]
    private static partial Regex YearMonthAndDateWithFilenameRegex();
}
