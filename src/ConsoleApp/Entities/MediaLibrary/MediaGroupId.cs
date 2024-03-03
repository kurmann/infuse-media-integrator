using CSharpFunctionalExtensions;
using Kurmann.InfuseMediaIntegrator.Entities.Elementary;

namespace Kurmann.InfuseMediaIntegrator.Entities.MediaLibrary;

/// <summary>
/// Die ID einer Mediengruppe.
/// Besteht aus einem ISO-8601-Datum und einem Titel, getrennt durch ein Leerzeichen.
/// Die ID kann als Dateiname verwendet werden.
/// Beispiel: "2022-01-01 Neujahrskonzert"
/// </summary>
public partial class MediaGroupId(string id)
{
    /// <summary>
    /// Die ID einer Mediengruppe.
    /// </summary>
    public string Id { get; private set;} = id;

    /// <summary>
    /// Liste aller Trennzeichen in einem Dateinamen, die bei der Ableitung einer ID aus dem Dateinamen ignoriert werden.
    /// Beispiele:
    /// - "2022-01-01 Neujahrskonzert-fanart.jpg" -> "2022-01-01 Neujahrskonzert" (ignoriert "-fanart.jpg", da Infuse-spezifisch)
    /// - "2022-01-01 Neujahrskonzert - 1.m4v" -> "2022-01-01 Neujahrskonzert" (ignoriert " - 1.m4v", da spezifisch für Final Cut Pro-Export bei mehreren Teilen)
    /// </summary>
    public static string[] IgnoredSeparators = ["-fanart", " - 1.m4v"];

    public static Result<MediaGroupId> Create(string id)
    {
        // Prüfe ob die ID leer ist
        if (string.IsNullOrWhiteSpace(id))
        {
            return Result.Failure<MediaGroupId>("ID is empty.");
        }

        // Prüfe ob die ID nach folgendem Muster "YYYY-MM-DD Titel" aufgebaut ist
        var isValid = IsValid(id);
        if (isValid.IsFailure)
        {
            return Result.Failure<MediaGroupId>(isValid.Error);
        }

        return Result.Success(new MediaGroupId(id));
    }

    /// <summary>
    /// Leitet die Erstellung einer MediaGroupId von einem Dateinamen ab.
    /// Ignoriert Zeichenketten nach den in IgnoredSeparators enthaltenen Trennzeichen.
    /// Beispiel: "2022-01-01 Neujahrskonzert-fanart.jpg" -> "2022-01-01 Neujahrskonzert"
    /// Beispiel: "2022-01-01 Neujahrskonzert - 1.m4v" -> "2022-01-01 Neujahrskonzert"
    /// </summary>
    /// <param name="fileNameInfo"></param>
    /// <returns></returns>
    public static Result<MediaGroupId> CreateFromFileName(FileNameInfo fileNameInfo)
    {
        // Entferne alle Zeichen, die in IgnoredSeparators enthalten sind
        var id = fileNameInfo.FileNameWithoutExtension;
        foreach (var separator in IgnoredSeparators)
        {
            id = id.Replace(separator, "");
        }

        return Create(id);
    }

    /// <summary>
    /// Prüft ob die ID nach folgendem Muster "YYYY-MM-DD Titel" aufgebaut ist.
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public static Result IsValid(string id)
    {
        // Prüfe ob die ID mit einem ISO-8601-Datum und einem Titel beginnt
        var hasIsoDateWithString = IsoDateWithString().IsMatch(id);
        if (!hasIsoDateWithString)
        {
            return Result.Failure("ID has not the correct format. The ID must be in the format 'YYYY-MM-DD Title'.");
        }

        // Prüfe ob die ID ein Dateiname sein kann
        var containsInvalidChars = CrossPlatformInvalidCharsHandler.ContainsInvalidChars(id);
        if (containsInvalidChars)
        {
            return Result.Failure("ID contains invalid characters that cannot be used for a filename.");
        }

        return Result.Success();
    }

    [System.Text.RegularExpressions.GeneratedRegex(@"^\d{4}-\d{2}-\d{2} .+$")]
    private static partial System.Text.RegularExpressions.Regex IsoDateWithString();
}
