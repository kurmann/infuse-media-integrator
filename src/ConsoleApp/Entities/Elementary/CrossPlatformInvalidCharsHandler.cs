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
    public static readonly List<char> InvalidChars;
    public static readonly List<char> InvalidCharsForWindowsPaths = ['\\', ':', '*', '?', '"', '<', '>', '|'];
    public static readonly List<char> InvalidCharsForUnixPaths = ['/'];

    static CrossPlatformInvalidCharsHandler()
    {
        InvalidChars =
        [
            '<', '>', ':', '"', '|', '?', '*', // Gemeinsam für Datei- und Verzeichnisnamen
        ];

        for (int i = 0; i < 32; i++) // Steuerzeichen
        {
            InvalidChars.Add((char)i);
        }

        // '/' fügen wir nicht zu InvalidChars hinzu, da es in Unix-Pfaden gültig ist
    }

    /// <summary>
    /// Gibt zurück, ob der gegebene Datei- oder Verzeichnisname ungültige Zeichen enthält.
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public static bool ContainsInvalidChars(string name)
    {
        return InvalidChars.Any(name.Contains);
    }

    /// <summary>
    /// Gibt zurück, ob der gegebene Pfad ungültige Zeichen enthält für Windows-Dateisysteme.
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    public static bool ContainsInvalidPathCharsInWindowsPath(string path)
    {
        var invalidChars = InvalidCharsForWindowsPaths;
        return invalidChars.Any(path.Contains);
    }

    /// <summary>
    /// Gibt zurück, ob der gegebene Pfad ungültige Zeichen enthält für Unix-Dateisysteme.
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    public static bool ContainsInvalidPathCharsInUnixPath(string path)
    {
        var invalidChars = InvalidCharsForUnixPaths;
        return invalidChars.Any(path.Contains);
    }

    public static string ReplaceInvalidChars(string name, char replacementChar)
    {
        foreach (char c in InvalidChars)
        {
            name = name.Replace(c, replacementChar);
        }
        return name;
    }

    public static string ReplaceInvalidPathChars(string path, char replacementChar, bool isWindowsPath)
    {
        var invalidChars = isWindowsPath ? InvalidCharsForWindowsPaths : InvalidCharsForUnixPaths;
        foreach (char c in invalidChars)
        {
            path = path.Replace(c, replacementChar);
        }
        return path;
    }
}