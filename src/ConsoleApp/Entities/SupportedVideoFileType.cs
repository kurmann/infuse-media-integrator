using CSharpFunctionalExtensions;

namespace Kurmann.InfuseMediaIntegrator;

/// <summary>
/// Nimmt eine Datei entgegen und prüft, ob es sich um eine unterstützte Video-Datei handelt und bestimmt den Video-Datentyp.
/// </summary>
public class SupportedVideoFileType
{
    private static readonly string[] quickTimeFileExtensions = [".mov", ".qt"];
    private static readonly string[] mpeg4FileExtensions = [".mp4", ".m4v"];

    public VideoFileType Type { get;}

    private SupportedVideoFileType(VideoFileType type) => Type = type;

    public static Result<SupportedVideoFileType> Create(string path)
    {
        try
        {
            // Erstelle ein FileInfo-Objekt
            var fileInfo = new FileInfo(path);

            // Prüfe, ob die Datei existiert
            if (!fileInfo.Exists)
                return Result.Failure<SupportedVideoFileType>("File not found.");

            // Bestimme den Dateityp anhand der Dateiendung und berücksichtige dabei die Groß-/Kleinschreibung nicht
            if (quickTimeFileExtensions.Contains(fileInfo.Extension, StringComparer.OrdinalIgnoreCase))
                return new SupportedVideoFileType(VideoFileType.QuickTime);
            if (mpeg4FileExtensions.Contains(fileInfo.Extension, StringComparer.OrdinalIgnoreCase))
                return new SupportedVideoFileType(VideoFileType.Mpeg4);
            return new SupportedVideoFileType(VideoFileType.NotSupportedFile);
        }
        catch (Exception e)
        {
            return Result.Failure<SupportedVideoFileType>($"Error on reading file info: {e.Message}");
        }
    }
}

public enum VideoFileType
{
    NotSupportedFile,
    QuickTime,
    Mpeg4
}
