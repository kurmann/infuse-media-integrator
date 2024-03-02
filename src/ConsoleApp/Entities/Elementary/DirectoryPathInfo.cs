using System.ComponentModel;
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
    /// Prüft ob der Pfad unzulässige Zeichen enthält für Windows-Pfade.
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    public static Result<DirectoryPathInfo> CreateWindowsPath(string? path)
    {
        // Prüfe ob der Pfad leer ist
        if (string.IsNullOrWhiteSpace(path))
        {
            return Result.Failure<DirectoryPathInfo>("Path is null or empty");
        }

        // Erstelle eine Instanz von DirectoryPathInfo, wenn der gegebene Pfad gültig ist
        var pathInfo = Create(path);
        if (pathInfo.IsFailure)
        {
            return pathInfo;
        }

        // Prüfe auf unzulässige Zeichen im Pfad
        if (CrossPlatformInvalidCharsHandler.ContainsInvalidPathCharsInWindowsPath(path))
        {
            return Result.Failure<DirectoryPathInfo>("Path contains invalid characters for Windows paths: " + string.Join(", ", CrossPlatformInvalidCharsHandler.InvalidCharsForWindowsPaths));
        }

        return pathInfo;
    }

    public static Result<DirectoryPathInfo> CreateUnixPath(string? path)
    {
        // Prüfe ob der Pfad leer ist
        if (string.IsNullOrWhiteSpace(path))
        {
            return Result.Failure<DirectoryPathInfo>("Path is null or empty");
        }

        // Erstelle eine Instanz von DirectoryPathInfo, wenn der gegebene Pfad gültig ist
        var pathInfo = Create(path);
        if (pathInfo.IsFailure)
        {
            return pathInfo;
        }

        // Prüfe auf unzulässige Zeichen im Pfad
        if (CrossPlatformInvalidCharsHandler.ContainsInvalidPathCharsInUnixPath(path))
        {
            return Result.Failure<DirectoryPathInfo>("Path contains invalid characters for Unix paths: " + string.Join(", ", CrossPlatformInvalidCharsHandler.InvalidCharsForUnixPaths));
        }

        return pathInfo;
    }

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

    /// <summary>
    /// Erstellt eine Instanz von DirectoryPathInfo, wenn der gegebene Pfad gültig ist.
    /// Entfernt Leerzeichen am Anfang jedes Verzeichnisses.
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    public static Result<DirectoryPathInfo> CreateTrimmed(string? path)
    {
        if (string.IsNullOrWhiteSpace(path))
        {
            return Result.Failure<DirectoryPathInfo>("Path is null or empty");
        }

        // Lies alle Verzeichnisse aus dem Pfad und entferne bei jedem Verzeichnis Leerzeichen am Anfang und am Ende
        var directories = path.Split(Path.DirectorySeparatorChar).Select(d => d.Trim()).ToArray();

        // Erstelle einen neuen Pfad aus den Verzeichnissen
        var newPath = string.Join(Path.DirectorySeparatorChar, directories);

        return Create(newPath);
    }

    public override string ToString() => DirectoryPath;

    public static implicit operator string(DirectoryPathInfo directoryPathInfo) => directoryPathInfo.DirectoryPath;
}