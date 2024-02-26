using CSharpFunctionalExtensions;

namespace Kurmann.InfuseMediaIntegrator.Entities.Elementary;

/// <summary>
/// Repräsentiert Informationen über einen Datei-Pfad, ohne direkt vom Dateisystem abhängig zu sein. Diese Klasse ist unveränderlich.
/// Prüft auf das Vorhandensein von unzulässigen Zeichen im Pfad und Dateinamen.
/// Prüft auf das Vorhandensein eines Dateinamens und eines Verzeichnisses.
/// </summary>
public class FilePathInfo
{
    /// <summary>
    /// Der vollständige Pfad.
    /// </summary>
    public string Path { get; }

    public DirectoryPathInfo DirectoryPathInfo => DirectoryPathInfo.Create(Path).Value;

    /// <summary>
    /// Informationen über den Dateinamen.
    /// </summary>
    public FileNameInfo FileName => FileNameInfo.Create(Path).Value;

    private FilePathInfo(string path) => Path = path;

    /// <summary>
    /// Erstellt eine Instanz von PathInfo, wenn der gegebene Pfad gültig ist.
    /// </summary>
    /// <param name="path">Der Pfad, der validiert und gespeichert werden soll.</param>
    /// <returns>Ein Result-Objekt, das entweder eine PathInfo-Instanz oder einen Fehler enthält.</returns>
    public static Result<FilePathInfo> Create(string path)
    {
        if (string.IsNullOrWhiteSpace(path))
        {
            return Result.Failure<FilePathInfo>("Path is null or empty");
        }

        // Prüfe auf unzulässige Zeichen im Pfad
        char[] invalidPathChars = System.IO.Path.GetInvalidPathChars();
        if (path.Any(c => invalidPathChars.Contains(c)))
        {
            return Result.Failure<FilePathInfo>("Path contains invalid characters: " + string.Join(", ", invalidPathChars));
        }

        // Prüft, ob der Pfad nicht nur ein Verzeichnis ist
        if (System.IO.Path.HasExtension(path))
        {
            return Result.Failure<FilePathInfo>("Path is not a file");
        }

        // Prüfe auf unzulässige Zeichen im Dateinamen, falls ein Dateiname vorhanden ist
        string fileName = System.IO.Path.GetFileName(path);
        if (!string.IsNullOrEmpty(fileName))
        {
            char[] invalidFileNameChars = System.IO.Path.GetInvalidFileNameChars();
            if (fileName.Any(c => invalidFileNameChars.Contains(c)))
            {
                return Result.Failure<FilePathInfo>("File name contains invalid characters: " + string.Join(", ", invalidFileNameChars));
            }
        }

        return new FilePathInfo(path);
    }
}
