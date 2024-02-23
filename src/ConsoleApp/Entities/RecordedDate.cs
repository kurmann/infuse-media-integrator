using System.Globalization;
using System.Text.RegularExpressions;
using CSharpFunctionalExtensions;

namespace Kurmann.InfuseMediaIntegrator.Entities;

/// <summary>
/// Klasse um ein Aufnahmedatum zu erstellen.
/// Enthält wesentliche Logik um ein Aufnahmedatum aus einer Schlagwortliste zu extrahieren.
/// </summary>
public class RecordedDate
{
    /// <summary>
    /// Das Aufnahmedatum.
    /// </summary>
    /// <value></value>
    public DateOnly Value { get; }

    /// <summary>
    /// Der Text, der als das Aufnahmedatum interpretiert wurde.
    /// Der Wert kann leer sein, wenn kein Datum gefunden wurde und das Erstellungsdatum der Datei verwendet wurde.
    /// </summary>
    public Maybe<string> MatchedString { get; }

    private RecordedDate(DateOnly value, Maybe<string> matchedString)
    {
        Value = value;
        MatchedString = matchedString;
    }

    public static Result<RecordedDate> CreateFromKeywords(IEnumerable<string>? keywords)
    {
        // Prüfe ob die Schlagwortliste leer ist
        keywords ??= new List<string>();
        var extractedDates = ExtractDates(keywords);

        // Prüfe ob ein Datum gefunden wurde
        if (extractedDates.IsFailure)
        {
            return Result.Failure<RecordedDate>(extractedDates.Error);
        }

        // Wenn mehrere Daten gefunden wurden, dann verwende das älteste Datum
        if (extractedDates.Value != null && extractedDates.Value.Any())
        {
            var foundRecordedDate = extractedDates.Value.Min();
            // todo: return all matched strings
            return new RecordedDate(foundRecordedDate, Maybe<string>.None);
        }
        else
        {
            return Result.Failure<RecordedDate>("No recorded dates found in keyword list");
        }
    }

    /// <summary>
    /// Extrahiert das Aufnahmedatum aus dem Dateinamen.
    /// Beispiel: 2021-06-07 Ausflug nach Bern.mp4
    /// Das Aufnahmedatum kann auch nur eine Jahreszahl enthalten.
    /// Beispiel: 2021 Rückblick Familie.mp4.
    /// In diesem Fall wird der 31.12. des Jahres als Aufnahmedatum verwendet.
    /// </summary>
    /// <param name="fileInfo"></param>
    /// <returns></returns>
    public static Result<RecordedDate> CreateFromFilename(FileInfo? fileInfo)
    {
        // Prüfe ob der Dateiname leer ist
        if (fileInfo == null)
        {
            return Result.Failure<RecordedDate>("Filename is empty");
        }

        // Versuche ein ISO-Datum aus dem Dateinamen zu extrahieren
        (var isoDate, var matchedIsoDateString) = TryParseIsoDate(fileInfo.Name);
        if (isoDate.HasValue)
        {
            return new RecordedDate(isoDate.Value, matchedIsoDateString);
        }

        // Versuche ein deutsches Datum aus dem Dateinamen zu extrahieren
        (var germanDate, var matchedGermanIsoString) = TryParseGermanDate(fileInfo.Name);
        if (germanDate.HasValue)
        {
            return new RecordedDate(germanDate.Value, matchedGermanIsoString);
        }

        // Versuche ein Monat aus dem Dateinamen zu extrahieren
        (var month, var matchedMonthString) = TryParseMonth(fileInfo.Name);
        if (month.HasValue)
        {
            return new RecordedDate(month.Value, matchedMonthString);
        }

        // Versuche eine Jahreszeit aus dem Dateinamen zu extrahieren
        (var season, var matchedSeasonString) = TryParseSeason(fileInfo.Name);
        if (season.HasValue)
        {
            return new RecordedDate(season.Value, matchedSeasonString);
        }

        // Versuche ein Jahr aus dem Dateinamen zu extrahieren
        (var year, var matchedYearString) = TryParseYear(fileInfo.Name);
        if (year.HasValue)
        {
            return new RecordedDate(year.Value, matchedYearString);
        }

        // Wenn kein Datum gefunden wurde, dann verwende das Erstellungsdatum der Datei
        var createdDate = fileInfo.CreationTime.Date;
        var dateOnly = new DateOnly(createdDate.Year, createdDate.Month, createdDate.Day);
        return new RecordedDate(dateOnly, Maybe<string>.None);
    }

    internal static Result<RecordedDate> CreateFromDateTime(DateTime? dateTime)
    {
        // Prüfe ob das Erstellungsdatum leer ist
        if (dateTime == null)
        {
            return Result.Failure<RecordedDate>("DateTime is empty");
        }

        // Erstelle ein Aufnahmedatum aus dem Erstellungsdatum
        var dateOnly = new DateOnly(dateTime.Value.Year, dateTime.Value.Month, dateTime.Value.Day);
        return new RecordedDate(dateOnly, dateTime.Value.ToString()); // In diesem Fall wird der Wert des Aufnahmedatums als Text verwendet
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