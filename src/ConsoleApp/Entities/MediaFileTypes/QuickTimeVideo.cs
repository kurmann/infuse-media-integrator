using CSharpFunctionalExtensions;
using Kurmann.InfuseMediaIntegrator.Entities.Elementary;

namespace Kurmann.InfuseMediaIntegrator.Entities.MediaFileTypes;

/// <summary>
/// Repräsentiert eine QuickTime-Video-Datei mit eingebetteten Metadaten.
/// </summary>
public class QuickTimeVideo : IMediaFileType
{
    /// <summary>
    /// Der Dateipfad.
    /// </summary>
    public FilePathInfo FilePath { get; }

    /// <summary>
    /// Die Metadaten des Videos.
    /// </summary>
    public MediaFileMetadata? Metadata { get; }

    /// <summary>
    /// Die zugehörigen Dateiendungen.
    /// </summary>
    public static readonly string[] FileExtensions = [".mov", ".qt"];

    private QuickTimeVideo(FilePathInfo filePath, MediaFileMetadata? mediaFileMetadata)
        => (FilePath, Metadata) = (filePath, mediaFileMetadata);

    public static Result<QuickTimeVideo> Create(string path, MediaFileMetadata? metadata = null)
    {
        try
        {
            // Erstelle ein FilePathInfo-Objekt
            var fileInfo = FilePathInfo.Create(path);
            if (fileInfo.IsFailure)
                return Result.Failure<QuickTimeVideo>($"Error on reading file info: {fileInfo.Error}");

            // Prüfe anhand der Dateiendung, ob es sich um eine QuickTime-Datei handelt
            var extension = Path.GetExtension(fileInfo.Value.FilePath).ToLowerInvariant();
            if (!FileExtensions.Contains(extension))
                return Result.Failure<QuickTimeVideo>("File is not a QuickTime video.");

            // Rückgabe des FileInfo-Objekts
            return new QuickTimeVideo(fileInfo.Value, metadata);
        }
        catch (Exception e)
        {
            return Result.Failure<QuickTimeVideo>($"Error on reading file info: {e.Message}");
        }
    }
}
