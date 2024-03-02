using CSharpFunctionalExtensions;

namespace Kurmann.InfuseMediaIntegrator.Entities.Elementary;

/// <summary>
/// Repräsentiert Informationen über einen Dateinamen, ohne direkt vom Dateisystem abhängig zu sein. Diese Klasse ist unveränderlich.
/// </summary>
public class FileNameInfo
{
    /// <summary>
    /// Der Dateiname mit Dateierweiterung.
    /// </summary>
    public string FileName { get; }

    /// <summary>
    /// Der Dateiname ohne Dateierweiterung.
    /// </summary>
    public string FileNameWithoutExtension => Path.GetFileNameWithoutExtension(FileName);

    /// <summary>
    /// Die Dateierweiterung.
    /// </summary>
    public string Extension => Path.GetExtension(FileName);

    private FileNameInfo(string fileName) => FileName = Path.GetFileName(fileName);

    /// <summary>
    /// Erstellt ein FileNameInfo-Objekt.
    /// </summary>
    /// <param name="fileName"></param>
    /// <returns></returns>
    public static Result<FileNameInfo> Create(string? fileName)
    {
        if (string.IsNullOrWhiteSpace(fileName))
        {
            return Result.Failure<FileNameInfo>("File name is null or empty");
        }

        // Prüfe, ob der Dateiname nur ein Verzeichnis ist
        if (!Path.HasExtension(fileName))
        {
            return Result.Failure<FileNameInfo>("File name is not a file");
        }

        // Entferne den Pfad, falls vorhanden
        fileName = Path.GetFileName(fileName);

        // Prüfe auf unzulässige Zeichen im Dateinamen
        if (CrossPlatformInvalidCharsHandler.ContainsInvalidChars(fileName))
        {
            return Result.Failure<FileNameInfo>("File name contains invalid characters: " + string.Join(", ", CrossPlatformInvalidCharsHandler.InvalidChars));
        }

        return Result.Success(new FileNameInfo(fileName));
    }

    public override string ToString() => FileName;

    public static implicit operator string(FileNameInfo fileNameInfo) => fileNameInfo.FileName;
}