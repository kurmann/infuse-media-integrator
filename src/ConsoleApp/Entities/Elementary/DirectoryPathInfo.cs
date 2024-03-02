using CSharpFunctionalExtensions;

namespace Kurmann.InfuseMediaIntegrator.Entities.Elementary;

/// <summary>
/// Repräsentiert Informationen über einen Verzeichnispfad, ohne direkt vom Dateisystem abhängig zu sein. Diese Klasse ist unveränderlich.
/// Unterscheidet sich von FilePathInfo, da es keine Informationen über den Dateinamen enthält.
/// </summary>
public class DirectoryPathInfo
{
    /// <summary>
    /// Der vollständige Pfad.
    /// </summary>
    public string DirectoryPath { get; }

    private DirectoryPathInfo(string directoryPath) => DirectoryPath = directoryPath;

    /// <summary>
    /// Liste der Verzeichnisse. Die oberste Kategorie ist an erster Stelle, gefolgt von den Unterkategorien.
    /// Entfernt leere Verzeichnisse und Leerzeichen.
    /// </summary>
    public List<DirectoryNameInfo> Directories => DirectoryPath
        .Split(Path.DirectorySeparatorChar)
        .Select(d => DirectoryNameInfo.Create(d).Value)
        .Where(d => !string.IsNullOrWhiteSpace(d.DirectoryName))
        .ToList();

    /// <summary>
    /// Erstellt eine Instanz von DirectoryPathInfo, wenn der gegebene Pfad gültig ist.
    /// </summary>
    /// <param name="path">Der Pfad, der validiert und gespeichert werden soll.</param>
    /// <returns>Ein Result-Objekt, das entweder eine DirectoryPathInfo-Instanz oder einen Fehler enthält.</returns>
    public static Result<DirectoryPathInfo> Create(string? path)
    {
        if (string.IsNullOrWhiteSpace(path))
        {
            return Result.Failure<DirectoryPathInfo>("Path is null or empty");
        }

        // Prüfe ob der Pfad eine reguläre Datei ist
        var isFile = Path.HasExtension(path);

        // Entferne die Datei im Pfad
        if (isFile)
            path = Path.GetDirectoryName(path);

        // Gib eine Fehlermeldung zurück, wenn der Pfad kein Verzeichnis ist
        if (string.IsNullOrWhiteSpace(path))
        {
            return Result.Failure<DirectoryPathInfo>("Path is not a directory");
        }

        // Prüfe auf unzulässige Zeichen im Pfad
        if (CrossPlatformInvalidCharsHandler.ContainsInvalidPathCharsInWindowsPath(path))
        {
            return Result.Failure<DirectoryPathInfo>("Path contains invalid characters for Windows paths: " + string.Join(", ", CrossPlatformInvalidCharsHandler.InvalidCharsForWindowsPaths));
        }
        if (CrossPlatformInvalidCharsHandler.ContainsInvalidPathCharsInUnixPath(path))
        {
            return Result.Failure<DirectoryPathInfo>("Path contains invalid characters for Unix paths: " + string.Join(", ", CrossPlatformInvalidCharsHandler.InvalidCharsForUnixPaths));
        }

        // Prüfe jedes Verzeichnis auf unzulässige Zeichen
        var directories = path.Split(Path.DirectorySeparatorChar);
        foreach (var directory in directories)
        {
            if (CrossPlatformInvalidCharsHandler.ContainsInvalidChars(directory))
            {
                return Result.Failure<DirectoryPathInfo>($"Directory '{directory}' contains invalid characters: " + string.Join(", ", CrossPlatformInvalidCharsHandler.InvalidChars));
            }
        }

        return Result.Success(new DirectoryPathInfo(path));
    }

    public override string ToString() => DirectoryPath;

    public static implicit operator string(DirectoryPathInfo directoryPathInfo) => directoryPathInfo.DirectoryPath;
}