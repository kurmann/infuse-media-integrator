using CSharpFunctionalExtensions;

namespace Kurmann.InfuseMediaIntegrator.Entities.Elementary;

/// <summary>
/// Repräsentiert Informationen über einen Pfad, ohne direkt vom Dateisystem abhängig zu sein. Diese Klasse ist unveränderlich.
/// </summary>
public class PathInfo
{
    /// <summary>
    /// Der vollständige Pfad.
    /// </summary>
    public string Path { get; }

    /// <summary>
    /// Informationen über den Dateinamen.
    /// </summary>
    public FileNameInfo? FileName { get; }

    private PathInfo(string path, FileNameInfo? fileNameInfo)
    {
        Path = path;
        FileName = fileNameInfo;
    }

    /// <summary>
    /// Erstellt eine Instanz von PathInfo, wenn der gegebene Pfad gültig ist.
    /// </summary>
    /// <param name="path">Der Pfad, der validiert und gespeichert werden soll.</param>
    /// <returns>Ein Result-Objekt, das entweder eine PathInfo-Instanz oder einen Fehler enthält.</returns>
    public static Result<PathInfo> Create(string path)
    {
        if (string.IsNullOrWhiteSpace(path))
        {
            return Result.Failure<PathInfo>("Path is null or empty");
        }

        // Prüfe auf unzulässige Zeichen im Pfad
        char[] invalidPathChars = System.IO.Path.GetInvalidPathChars();
        if (path.Any(c => invalidPathChars.Contains(c)))
        {
            return Result.Failure<PathInfo>("Path contains invalid characters: " + string.Join(", ", invalidPathChars));
        }

        // Prüfe auf unzulässige Zeichen im Dateinamen, falls ein Dateiname vorhanden ist
        string fileName = System.IO.Path.GetFileName(path);
        FileNameInfo? fileNameInfo = null;
        if (!string.IsNullOrEmpty(fileName))
        {
            char[] invalidFileNameChars = System.IO.Path.GetInvalidFileNameChars();
            if (fileName.Any(c => invalidFileNameChars.Contains(c)))
            {
                return Result.Failure<PathInfo>("File name contains invalid characters: " + string.Join(", ", invalidFileNameChars));
            }

            // Erstellen von FileNameInfo
            var fileNameInfoResult = FileNameInfo.Create(fileName);
            if (fileNameInfoResult.IsFailure)
            {
                return Result.Failure<PathInfo>(fileNameInfoResult.Error);
            }

            fileNameInfo = fileNameInfoResult.Value;
        }

        return new PathInfo(path, fileNameInfo);
    }
}
