using System.Text.RegularExpressions;
using CSharpFunctionalExtensions;

namespace Kurmann.InfuseMediaIntegrator.Entities;

/// <summary>
/// Stellt eine Kombination aus Titel und Aufnahmedatum dar.
/// Enthält die Logik um das Aufnahmedatum aus dem Titel zu extrahieren und weist aus, welche Teile des Titels als Aufnahmedatum interpretiert werden.
/// </summary>
public class TitleWithRecordedDate
{
    private TitleWithRecordedDate(string title, RecordedDate recordedDate, string? separator = null, PatternPosition patternPosition = PatternPosition.Missing)
    {
        Title = title;
        RecordedDate = recordedDate;
        Separator = separator;
        RecordedDatePosition = patternPosition;
    }

    /// <summary>
    /// Der Titel der Datei ohne das Aufnahmedatum.
    /// </summary>
    public string Title { get; }

    /// <summary>
    /// Das Aufnahmedatum der Datei, entweder aus dem Dateinamen oder dem Änderungsdatum der Datei.
    /// </summary>
    public RecordedDate RecordedDate { get; }

    /// <summary>
    /// Das Trennzeichen zwischen Titel und Aufnahmedatum, falls vorhanden.
    /// </summary>
    public string? Separator { get; }

    /// <summary>
    /// Die Position des Aufnahmedatums innerhalb des Dateinamens.
    /// </summary>
    public PatternPosition RecordedDatePosition { get;}

    public static Result<TitleWithRecordedDate> Create(FileInfo fileInfo)
    {
        // Prüfe, ob der Dateiname ein Aufnahmedatum enthält
        var recordedDateResult = RecordedDate.CreateFromFilename(fileInfo);
        if (recordedDateResult.IsFailure)
        {
            // wenn keines gefunden wird, dann wird das Änderungsdatum der Datei verwendet und eine Warnung geloggt
            var creationTime = fileInfo.CreationTimeUtc;
            recordedDateResult = RecordedDate.CreateFromDateTime(creationTime);
            if (recordedDateResult.IsFailure)
            {
                return Result.Failure<TitleWithRecordedDate>(recordedDateResult.Error);
            }
        }

        // Prüfe, ob überhaupt das Aufnahmedatum im Dateinamen enthalten ist
        if (recordedDateResult.Value.MatchedString.HasNoValue)
        {
            // In diesem Falle wird der Dateiname nicht weiter analysiert und der Titel ist der Dateiname
            return Result.Success(new TitleWithRecordedDate(fileInfo.Name, recordedDateResult.Value));
        }

        // Ermittle den ganzen Dateinamen ohne Dateiendung
        var filenameWithoutExtension = Path.GetFileNameWithoutExtension(fileInfo.Name);

        // Prüfe ob das Aufnahmedatum zu Beginn des Textes, in der Mitte oder am Ende steht und ermittele die Position
        var patternPosition = GetPatternPosition(filenameWithoutExtension, recordedDateResult.Value.MatchedString.Value);

        // Falls die Position zur Mitte ist, dann gib einen Fehler zurück, da dies nicht unterstützt wird
        if (patternPosition == PatternPosition.Middle)
        {
            return Result.Failure<TitleWithRecordedDate>($"The recorded date '{recordedDateResult.Value.MatchedString.Value}' is in the middle of the filename '{filenameWithoutExtension}'. This is not supported.");
        }

        // Entferne das Aufnahmedatum aus dem Dateinamen
        var textWithoutRecordedDate = filenameWithoutExtension.Replace(recordedDateResult.Value.MatchedString.Value, string.Empty).Trim();

        // Entferne alle nicht-alphanumerischen Zeichen zu Beginn und am Ende des Textes. Verwende den Regex \W, um alle nicht-alphanumerischen Zeichen zu finden.
        var textWithoutRecordedDateAndNonAlphanumericCharacters = Regex.Replace(textWithoutRecordedDate, @"^\W+|\W+$", string.Empty);

        // Interpretiere den verbleibenden Text als Titel
        var title = textWithoutRecordedDateAndNonAlphanumericCharacters;

        // Ermittle das Trennzeichen, indem vom Dateinamen der Titel und das Aufnahmedatum abgezogen wird
        var separator = filenameWithoutExtension.Replace(title, string.Empty).Replace(recordedDateResult.Value.MatchedString.Value, string.Empty);
        return Result.Success(new TitleWithRecordedDate(title, recordedDateResult.Value, separator, patternPosition));
    }

    private static PatternPosition GetPatternPosition(string text, string pattern)
    {
        return pattern switch
        {
            var p when text.StartsWith(p) => PatternPosition.Start,
            var p when text.EndsWith(p) => PatternPosition.End,
            var p when text.Contains(p) => PatternPosition.Middle,
            _ => PatternPosition.Missing
        };
    }
}

/// <summary>
/// Definiert die Position eines Musters in einem Text.
/// </summary>
public enum PatternPosition
{
    Missing,
    Start,
    Middle,
    End
}