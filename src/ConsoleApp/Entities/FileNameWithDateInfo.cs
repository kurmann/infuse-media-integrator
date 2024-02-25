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
    /// Das Aufnahmedatum.
    /// </summary>
    /// <value></value>
    public DateOnly Date { get; }

    /// <summary>
    /// Der Text, der als das Aufnahmedatum interpretiert wurde.
    /// Der Wert kann leer sein, wenn kein Datum gefunden wurde und das Erstellungsdatum der Datei verwendet wurde.
    /// </summary>
    public Maybe<string> MatchedString { get; }

    private FileNameWithDateInfo(DateOnly date, Maybe<string> matchedString)
    {
        Date = date;
        MatchedString = matchedString;
    }

    /// <summary>
    /// Extrahiert das Aufnahmedatum aus dem Dateinamen.
    /// Beispiel: 2021-06-07 Ausflug nach Bern.mp4
    /// Das Aufnahmedatum kann auch nur eine Jahreszahl enthalten.
    /// Beispiel: 2021 Rückblick Familie.mp4.
    /// In diesem Fall wird der 31.12. des Jahres als Aufnahmedatum verwendet.
    /// </summary>
    /// <param name="fileName"></param>
    /// <returns></returns>
    public static Result<FileNameWithDateInfo> Create(string? fileName)
    {
        var fileNameInfo = FileNameInfo.Create(fileName);
        if (fileNameInfo.IsFailure)
        {
            return Result.Failure<FileNameWithDateInfo>(fileNameInfo.Error);
        }

        // Versuche ein ISO-Datum aus dem Dateinamen zu extrahieren
        (var isoDate, var matchedIsoDateString) = TryParseIsoDate(fileNameInfo.Value.Name);
        if (isoDate.HasValue)
        {
            return new FileNameWithDateInfo(isoDate.Value, matchedIsoDateString);
        }

        // Versuche ein deutsches Datum aus dem Dateinamen zu extrahieren
        (var germanDate, var matchedGermanIsoString) = TryParseGermanDate(fileNameInfo.Value.Name);
        if (germanDate.HasValue)
        {
            return new FileNameWithDateInfo(germanDate.Value, matchedGermanIsoString);
        }

        // Versuche ein Monat aus dem Dateinamen zu extrahieren
        (var month, var matchedMonthString) = TryParseMonth(fileNameInfo.Value.Name);
        if (month.HasValue)
        {
            return new FileNameWithDateInfo(month.Value, matchedMonthString);
        }

        // Versuche eine Jahreszeit aus dem Dateinamen zu extrahieren
        (var season, var matchedSeasonString) = TryParseSeason(fileNameInfo.Value.Name);
        if (season.HasValue)
        {
            return new FileNameWithDateInfo(season.Value, matchedSeasonString);
        }

        // Versuche ein Jahr aus dem Dateinamen zu extrahieren
        (var year, var matchedYearString) = TryParseYear(fileNameInfo.Value.Name);
        if (year.HasValue)
        {
            return new FileNameWithDateInfo(year.Value, matchedYearString);
        }

        // Wenn kein Datum gefunden wurde, gib einen Fehler zurück
        return Result.Failure<FileNameWithDateInfo>("No date found in file name");
    }

    internal static Result<FileNameWithDateInfo> CreateFromDateTime(DateTime? dateTime)
    {
        // Prüfe ob das Erstellungsdatum leer ist
        if (dateTime == null)
        {
            return Result.Failure<FileNameWithDateInfo>("DateTime is empty");
        }

        // Erstelle ein Aufnahmedatum aus dem Erstellungsdatum
        var dateOnly = new DateOnly(dateTime.Value.Year, dateTime.Value.Month, dateTime.Value.Day);
        return new FileNameWithDateInfo(dateOnly, dateTime.Value.ToString()); // In diesem Fall wird der Wert des Aufnahmedatums als Text verwendet
    }

    /// <summary>
    /// Extrahiert ein Datum aus einer Schlagwortliste.
    /// </summary>
    /// <param name="keywords"></param>
    /// <returns></returns>
    private static Result<List<DateOnly>> ExtractDates(IEnumerable<string> keywords)
    {
        try
        {
            var dates = new List<DateOnly>();

            // iterate over keywords
            foreach (var keyword in keywords)
            {
                // search for ISO date
                var (isoDate, matchedString) = TryParseIsoDate(keyword);
                if (isoDate.HasValue)
                {
                    dates.Add(isoDate.Value);
                    continue;
                }

                // search for german date
                (var germanDate, var foundGermanDateString) = TryParseGermanDate(keyword);
                if (germanDate.HasValue)
                {
                    dates.Add(germanDate.Value);
                }
            }

            return dates;
        }
        catch (Exception ex)
        {
            return Result.Failure<List<DateOnly>>($"Error on parsing date values from keyword list: {ex.Message}");
        }
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

    /// <summary>
    /// Versucht aus einem Text eine Jahreszeit zu parsen.
    /// Sucht nach deutschen und englischen Jahreszeiten, z.B. Sommer 2021 oder Summer 2021.
    /// Für die jeweilige Jahreszeit werden die folgenden Daten verwendet:
    /// Frühling: 1. April
    /// Sommer: 1. Juli
    /// Herbst: 1. Oktober
    /// Winter: 1. Januar
    /// </summary>
    /// <param name="text"></param>
    /// <returns></returns>
    private static (Maybe<DateOnly>, Maybe<string>) TryParseSeason(string text)
    {
        string[] patterns =
        [
            @"\b(Spring|Summer|Fall|Autumn|Winter)\b \d{4}",  // Englische Jahreszeit: Summer 2021
            @"\b(Frühling|Sommer|Herbst|Winter)\b \d{4}",  // Deutsche Jahreszeit: Sommer 2021
        ];

        foreach (string pattern in patterns)
        {
            var match = Regex.Match(text, pattern);
            if (match.Success)
            {
                int year = int.Parse(match.Groups[0].Value.Split(' ')[1]);
                switch (match.Groups[1].Value.ToLower())
                {
                    case "spring":
                    case "frühling":
                        return (new DateOnly(year, 4, 1), match.Value);
                    case "summer":
                    case "sommer":
                        return (new DateOnly(year, 7, 1), match.Value);
                    case "fall":
                    case "autumn":
                    case "herbst":
                        return (new DateOnly(year, 10, 1), match.Value);
                    case "winter":
                        return (new DateOnly(year, 1, 1), match.Value);
                }
            }
        }

        return (Maybe<DateOnly>.None, Maybe<string>.None);
    }
}