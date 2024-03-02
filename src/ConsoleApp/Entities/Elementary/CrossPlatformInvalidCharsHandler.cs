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

    static CrossPlatformInvalidCharsHandler()
    {
        InvalidChars =
        [
            '<', '>', ':', '"', '/', '\\', '|', '?', '*' // Windows-spezifische ungültige Zeichen
        ];

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
}
