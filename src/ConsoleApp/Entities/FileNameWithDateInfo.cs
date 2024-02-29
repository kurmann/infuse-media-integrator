using System.Globalization;
using System.Text.RegularExpressions;
using CSharpFunctionalExtensions;
using Kurmann.InfuseMediaIntegrator.Entities.Elementary;

namespace Kurmann.InfuseMediaIntegrator.Entities;

/// <summary>
/// Klasse um ein Aufnahmedatum zu erstellen.
/// Enthält wesentliche Logik um ein Aufnahmedatum aus einer Schlagwortliste zu extrahieren.
/// </summary>
public class FileNameWithDateInfo
{
    /// <summary>
    /// Der Dateiname.
    /// </summary>
    public FileNameInfo FileName { get; }

    /// <summary>
    /// Das Aufnahmedatum.
    /// </summary>
    /// <value></value>
    public DateOnly Date { get; }

    /// <summary>
    /// Der Text, der als das Aufnahmedatum interpretiert wurde.
    /// </summary>
    public string DateString { get; }

    /// <summary>
    /// Gibt an, ob das gefundene Datum am Anfang des Dateinamens steht.
    /// Prüft ob der DateString am Anfang des Dateinamens steht.
    /// Berücksichgt etwaige Leerzeichen zu Beginn des Dateinamens.
    /// </summary>
    public bool IsDateAtStart => FileName.FileNameWithoutExtension.StartsWith(DateString.TrimStart());

    /// <summary>
    /// Gibt an, ob das gefundene Datum am Anfang des Dateinamens steht.
    /// Berücksichtigt etwaige Leerzeichen am Ende des Dateinamens.
    /// </summary>
    public bool IsDateAtEnd => FileName.FileNameWithoutExtension.EndsWith(DateString.TrimEnd());

    private FileNameWithDateInfo(DateOnly date, string dateString, FileNameInfo fileNameInfo)
    {
        Date = date;
        DateString = dateString;
        FileName = fileNameInfo;
    }

    public static Result<FileNameWithDateInfo> Create(string? fileName)
    {
        // Erstelle ein FileNameInfo-Objekt
        var fileNameInfo = FileNameInfo.Create(fileName);
        if (fileNameInfo.IsFailure)
        {
            return Result.Failure<FileNameWithDateInfo>(fileNameInfo.Error);
        }

        return Create(fileNameInfo.Value);
    }

    /// <summary>
    /// Extrahiert das Aufnahmedatum aus dem Dateinamen.
    /// Beispiel: 2021-06-07 Ausflug nach Bern.mp4
    /// Das Aufnahmedatum kann auch nur eine Jahreszahl enthalten.
    /// Beispiel: 2021 Rückblick Familie.mp4.
    /// In diesem Fall wird der 31.12. des Jahres als Aufnahmedatum verwendet.
    /// </summary>
    /// <param name="fileNameInfo"></param>
    /// <returns></returns>
    public static Result<FileNameWithDateInfo> Create(FileNameInfo fileNameInfo)
    {
        // Versuche ein ISO-Datum aus dem Dateinamen zu extrahieren
        (var isoDate, var matchedIsoDateString) = TryParseIsoDate(fileNameInfo.FileName);
        if (isoDate.HasValue)
        {
            return new FileNameWithDateInfo(isoDate.Value, matchedIsoDateString.Value, fileNameInfo);
        }

        // Versuche ein deutsches Datum aus dem Dateinamen zu extrahieren
        (var germanDate, var matchedGermanIsoString) = TryParseGermanDate(fileNameInfo.FileName);
        if (germanDate.HasValue)
        {
            return new FileNameWithDateInfo(germanDate.Value, matchedGermanIsoString.Value, fileNameInfo);
        }

        // Versuche ein Monat aus dem Dateinamen zu extrahieren
        (var month, var matchedMonthString) = TryParseMonth(fileNameInfo.FileName);
        if (month.HasValue)
        {
            return new FileNameWithDateInfo(month.Value, matchedMonthString.Value, fileNameInfo);
        }

        // Versuche ein Jahr aus dem Dateinamen zu extrahieren
        (var year, var matchedYearString) = TryParseYear(fileNameInfo.FileName);
        if (year.HasValue)
        {
            return new FileNameWithDateInfo(year.Value, matchedYearString.Value, fileNameInfo);
        }

        // Wenn kein Datum gefunden wurde, gib einen Fehler zurück
        return Result.Failure<FileNameWithDateInfo>("No date found in file name");
    }

    /// <summary>
    /// Versucht aus dem <paramref name="text"/> ein ISO-Datum zu parsen.
    /// </summary>
    /// <param name="text"></param>
    /// <returns></returns>
    private static (Maybe<DateOnly> date, Maybe<string> matchedString) TryParseIsoDate(string text)
    {
        string isoPattern = @"\b\d{4}-\d{2}-\d{2}\b";  // ISO-Datum: 2023-06-07

        var isoMatch = Regex.Match(text, isoPattern);
        if (isoMatch.Success && DateOnly.TryParse(isoMatch.Value, out DateOnly isoDate))
        {
            return (isoDate, isoMatch.Value);
        }

        return (Maybe<DateOnly>.None, Maybe<string>.None);
    }

    /// <summary>
    /// Versucht aus dem <paramref name="text"/> ein deutsches Datum zu parsen.
    /// </summary>
    /// <param name="text"></param>
    /// <returns></returns>
    private static (Maybe<DateOnly>, Maybe<string>) TryParseGermanDate(string text)
    {
        string germanPattern = @"\b(\d{1,2}\.)?\d{1,2}\.(\d{2}|\d{4})\b";  // Deutsches Datum: 7.6.2023 oder 07.06.23

        var germanMatch = Regex.Match(text, germanPattern);
        if (germanMatch.Success)
        {
            var fromCulture = new CultureInfo("de-CH");
            if (DateOnly.TryParseExact(germanMatch.Value,
                                       [fromCulture.DateTimeFormat.ShortDatePattern, "dd.MM.yy", "d.M.yy", "dd.M.yy", "d.MM.yy"],
                                       fromCulture,
                                       DateTimeStyles.None,
                                       out var germanDate))
            {
                return (germanDate, germanMatch.Value);
            }
        }

        return (Maybe<DateOnly>.None, Maybe<string>.None);
    }

    /// <summary>
    /// Versucht aus dem <paramref name="text"/> ein Jahr zu parsen.
    /// </summary>
    /// <param name="text"></param>
    /// <returns></returns>
    private static (Maybe<DateOnly>, Maybe<string>) TryParseYear(string text)
    {
        string yearPattern = @"\b\d{4}\b";  // Jahr: 2023

        var yearMatch = Regex.Match(text, yearPattern);
        if (yearMatch.Success && int.TryParse(yearMatch.Value, out int year))
        {
            DateOnly endOfYearDate = new(year, 12, 31);
            return (endOfYearDate, yearMatch.Value);
        }

        return(Maybe<DateOnly>.None, Maybe<string>.None);
    }

    /// <summary>
    /// Versucht aus dem <paramref name="text"/> ein Datum zu parsen.
    /// Sucht nach ISO-Monatsangaben, z.B. 2021-06.
    /// Sucht nach deutschen und englischen Monatsnamen, z.B. Juni 2021 oder June 2021.
    /// Für den jeweiligen Monat wird der erste Tag des Monats als Datum verwendet.
    /// </summary>
    /// <param name="text"></param>
    /// <returns></returns>
    private static (Maybe<DateOnly>, Maybe<string>) TryParseMonth(string text)
    {
        string[] patterns =
        [
            @"\b\d{4}-\d{2}\b",  // ISO-Monat: 2021-06
            @"\b(January|February|March|April|May|June|July|August|September|October|November|December)\b \d{4}",  // Englischer Monatsname: June 2021
            @"\b(Januar|Februar|März|April|Mai|Juni|Juli|August|September|Oktober|November|Dezember)\b \d{4}",  // Deutscher Monatsname: Juni 2021
        ];

        foreach (string pattern in patterns)
        {
            var match = Regex.Match(text, pattern);
            if (match.Success && DateOnly.TryParse(match.Value, out DateOnly date))
            {
                return (date, match.Value);
            }
        }

        return (Maybe<DateOnly>.None, Maybe<string>.None);
    }
}