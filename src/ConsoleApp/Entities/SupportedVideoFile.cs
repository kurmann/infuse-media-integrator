using CSharpFunctionalExtensions;

namespace Kurmann.InfuseMediaIntegrator;

/// <summary>
/// Repräsentiert eine unterstützte Video-Datei.
/// Unterstützte Dateitypen sind QuickTime und MPEG4.
/// Erkennung anhand der Dateiendung.
/// </summary>
public class SupportedVideoFile
{
    private static readonly string[] quickTimeFileExtensions = [".mov", ".qt"];
    private static readonly string[] mpeg4FileExtensions = [".mp4", ".m4v"];

    public VideoFileType Type { get;}

    public FileInfo FileInfo { get; }

    private SupportedVideoFile(FileInfo fileInfo, VideoFileType type)
    {
        FileInfo = fileInfo;
        Type = type;
    }

    public static Result<SupportedVideoFile> Create(string path)
    {
        try
        {
            // Erstelle ein FileInfo-Objekt
            var fileInfo = new FileInfo(path);

            // Prüfe, ob die Datei existiert
            if (!fileInfo.Exists)
                return Result.Failure<SupportedVideoFile>("File not found.");

            // Bestimme den Dateityp anhand der Dateiendung und berücksichtige dabei die Groß-/Kleinschreibung nicht
            if (quickTimeFileExtensions.Contains(fileInfo.Extension, StringComparer.OrdinalIgnoreCase))
                return new SupportedVideoFile(fileInfo, VideoFileType.QuickTime);
            if (mpeg4FileExtensions.Contains(fileInfo.Extension, StringComparer.OrdinalIgnoreCase))
                return new SupportedVideoFile(fileInfo, VideoFileType.Mpeg4);
            return Result.Failure<SupportedVideoFile>("File is not a supported video file.");
        }
        catch (Exception e)
        {
            return Result.Failure<SupportedVideoFile>($"Error on reading file info: {e.Message}");
        }
    }
}

public enum VideoFileType
{
    QuickTime,
    Mpeg4
}
