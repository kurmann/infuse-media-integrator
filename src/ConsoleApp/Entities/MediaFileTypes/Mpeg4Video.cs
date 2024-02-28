using CSharpFunctionalExtensions;
using Kurmann.InfuseMediaIntegrator.Entities.Elementary;

namespace Kurmann.InfuseMediaIntegrator.Entities.MediaFileTypes;

/// <summary>
/// Repräsentiert eine MPEG4-Video-Datei mit eingebetteten Metadaten.
/// </summary>
public class Mpeg4Video : IMediaFileType
{
    /// <summary>
    /// Der Dateipfad.
    /// </summary>
    public FilePathInfo FilePath { get; }

    /// <summary>
    /// Die zugehörigen Dateiendungen.
    /// </summary>
    public static readonly string[] FileExtensions = [".mp4", ".m4v"];

    private Mpeg4Video(FilePathInfo filePath) => FilePath = filePath;

    public static Result<Mpeg4Video> Create(string? path)
    {
        try
        {
            // Prüfe, ob der Pfad leer ist
            if (string.IsNullOrWhiteSpace(path))
                return Result.Failure<Mpeg4Video>("Path is empty.");

            // Erstelle ein FilePathInfo-Objekt
            var fileInfo = FilePathInfo.Create(path);
            if (fileInfo.IsFailure)
                return Result.Failure<Mpeg4Video>($"Error on reading file info: {fileInfo.Error}");

            // Prüfe anhand der Dateiendung, ob es sich um eine MPEG4-Datei handelt
            var extension = Path.GetExtension(fileInfo.Value.FilePath).ToLowerInvariant();
            if (!FileExtensions.Contains(extension))
                return Result.Failure<Mpeg4Video>("File is not a MPEG4 video.");

            // Rückgabe des FileInfo-Objekts
            return new Mpeg4Video(fileInfo.Value);
        }
        catch (Exception e)
        {
            return Result.Failure<Mpeg4Video>($"Error on reading file info: {e.Message}");
        }
    }
}