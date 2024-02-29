using CSharpFunctionalExtensions;
using Kurmann.InfuseMediaIntegrator.Entities.Elementary;

namespace Kurmann.InfuseMediaIntegrator.Entities.MediaFileTypes;

/// <summary>
/// Repräsentiert eine nicht unterstützte Datei. Diese Dateien sind in der Mediathek nicht nutzbar,
/// werden aber dennoch in der Mediathek abgelegt.
/// </summary>
public class NotSupportedFile : IMediaFileType
{
    /// <summary>
    /// Der Dateipfad.
    /// </summary>
    public FilePathInfo FilePath { get; }

    /// <summary>
    /// Die Metadaten der Datei, sofern vorhanden.
    /// </summary>
    public MediaFileMetadata? Metadata { get; }

    private NotSupportedFile(FilePathInfo filePath) => FilePath = filePath;

    public static Result<NotSupportedFile> Create(string path)
    {
        try
        {
            // Erstelle ein FilePathInfo-Objekt
            var filePath = FilePathInfo.Create(path);
            if (filePath.IsFailure)
                return Result.Failure<NotSupportedFile>($"Error on reading file info: {filePath.Error}");

            // Rückgabe des FileInfo-Objekts
            return new NotSupportedFile(filePath.Value);
        }
        catch (Exception e)
        {
            return Result.Failure<NotSupportedFile>($"Error on reading file info: {e.Message}");
        }
    }

    public override string ToString() => FilePath.FilePath;
}
