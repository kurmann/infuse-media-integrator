namespace Kurmann.InfuseMediaIntegrator.Entities.Elementary;

/// <summary>
/// Verwaltet ungültige Zeichen für Datei- und Verzeichnisnamen.
/// Im Gegegensatz zu Path.GetInvalidPathChars() und Path.GetInvalidFileNameChars() enthält diese Klasse
/// ungültige Zeichen für alle Windows- und Unix-Dateisysteme.
/// Unter Windows sind die ungültigen Zeichen für Datei- und Verzeichnisnamen in der Regel:
/// < (Kleiner als)
/// > (Größer als)
/// : (Doppelpunkt)
/// " (Anführungszeichen)
/// / (Schrägstrich)
/// \ (Rückwärtsschrägstrich)
/// | (Senkrechter Strich)
/// ? (Fragezeichen)
/// * (Sternchen)
/// Zusätzlich sind Zeichen mit ASCII-Werten von 0 bis 31 (Steuerzeichen) ebenfalls ungültig.
/// </summary>
public class CrossPlatformInvalidCharsHandler
{
    /// <summary>
    /// Liste der ungültigen Zeichen für Datei- und Verzeichnisnamen.
    /// </summary>
    public static readonly List<char> InvalidChars;

    /// <summary>
    /// Liste der ungültigen Zeichen für Datei- und Verzeichnisnamen, die druckbar sind.
    /// </summary>
    public static List<char> PrintableInvalidChars => InvalidChars.Where(c => c >= 32).ToList();

    static CrossPlatformInvalidCharsHandler()
    {
        // Windows-spezifische ungültige Zeichen
        InvalidChars =
        [
            '<', '>', ':', '"', '/', '\\', '|', '?', '*' 
        ];

        // Füge Steuerzeichen hinzu
        for (int i = 0; i < 32; i++)
        {
            InvalidChars.Add((char)i);
        }
    }

    /// <summary>
    /// Prüft, ob der gegebene Name ungültige Zeichen enthält.
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public static bool ContainsInvalidChars(string name)
    {
        foreach (char c in InvalidChars)
        {
            if (name.Contains(c))
            {
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// Ersetzt ungültige Zeichen im gegebenen Namen durch das gegebene Zeichen.
    /// </summary>
    /// <param name="name"></param>
    /// <param name="replacementChar"></param>
    /// <returns></returns>
    public static string ReplaceInvalidChars(string name, char replacementChar)
    {
        foreach (char c in InvalidChars)
        {
            name = name.Replace(c, replacementChar);
        }
        return name;
    }

    /// <summary>
    /// Ersetzt ungültige Zeichen durch Leerzeichen, um den Namen für die Verwendung in einem Dateisystem zu bereinigen.
    /// Wenn das ungültige Zeichen zu Beginn oder am Ende des Namens steht, wird es entfernt.
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public static string ReplaceInvalidCharsWithSpaces(string name)
    {
        foreach (char c in InvalidChars)
        {
            name = name.Replace(c, ' ');
        }
        return name.Trim();
    }
}
