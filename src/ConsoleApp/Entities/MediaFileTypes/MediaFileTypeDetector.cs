using CSharpFunctionalExtensions;

namespace Kurmann.InfuseMediaIntegrator.Entities.MediaFileTypes;


public class MediaFileTypeDetector
{
    private static readonly string[] quickTimeFileExtensions = [".mov", ".qt"];
    private static readonly string[] mpeg4FileExtensions = [".mp4", ".m4v"];
    private static readonly string[] jpegFileExtensions = [".jpg", ".jpeg"];

    public VideoFileType Type { get;}

    private MediaFileTypeDetector(VideoFileType type) => Type = type;

    public static Result<MediaFileTypeDetector> Create(string path)
    {
        try
        {
            // Erstelle ein FileInfo-Objekt
            var fileInfo = new FileInfo(path);

            // Bestimme den Dateityp anhand der Dateiendung und berücksichtige dabei die Groß-/Kleinschreibung nicht
            if (quickTimeFileExtensions.Contains(fileInfo.Extension, StringComparer.OrdinalIgnoreCase))
                return new MediaFileTypeDetector(VideoFileType.QuickTime);
            if (mpeg4FileExtensions.Contains(fileInfo.Extension, StringComparer.OrdinalIgnoreCase))
                return new MediaFileTypeDetector(VideoFileType.Mpeg4);
            if (jpegFileExtensions.Contains(fileInfo.Extension, StringComparer.OrdinalIgnoreCase))
                return new MediaFileTypeDetector(VideoFileType.Jpeg);
            return new MediaFileTypeDetector(VideoFileType.NotSupported);
        }
        catch (Exception e)
        {
            return Result.Failure<MediaFileTypeDetector>($"Error on reading file info: {e.Message}");
        }
    }
}

public enum VideoFileType
{
    NotSupported,
    QuickTime,
    Mpeg4,
    Jpeg
}
