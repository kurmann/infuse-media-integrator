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

    public static bool ContainsInvalidChars(string name)
    {
        return InvalidChars.Any(name.Contains);
    }

    public static bool ContainsInvalidPathChars(string path, bool isWindowsPath)
    {
        var invalidChars = isWindowsPath ? InvalidCharsForWindowsPaths : InvalidCharsForUnixPaths;
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