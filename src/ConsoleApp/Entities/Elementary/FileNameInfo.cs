using CSharpFunctionalExtensions;

namespace Kurmann.InfuseMediaIntegrator.Entities;

/// <summary>
/// Repräsentiert Informationen über einen Dateinamen, ohne direkt vom Dateisystem abhängig zu sein. Diese Klasse ist unveränderlich.
/// </summary>
public class FileNameInfo
{
    /// <summary>
    /// Der Dateiname.
    /// </summary>
    public string Name { get; }

    private FileNameInfo(string fullPath) => Name = Path.GetFileName(fullPath);

    public static Result<FileNameInfo> Create(string? fileName)
    {
        if (string.IsNullOrWhiteSpace(fileName))
        {
            return Result.Failure<FileNameInfo>("File name is null or empty");
        }

        // Prüfe auf unzulässige Zeichen im Dateinamen
        char[] invalidFileNameChars = Path.GetInvalidFileNameChars();
        if (fileName.Any(c => invalidFileNameChars.Contains(c)))
        {
            return Result.Failure<FileNameInfo>("File name contains invalid characters: " + string.Join(", ", invalidFileNameChars));
        }

        return Result.Success(new FileNameInfo(fileName));
    }
}