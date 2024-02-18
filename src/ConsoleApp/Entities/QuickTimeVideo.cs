using CSharpFunctionalExtensions;

namespace Kurmann.InfuseMediaIntegrator.Entities;

/// <summary>
/// Repräsentiert eine QuickTime-Video-Datei mit eingebetteten Metadaten.
/// </summary>
public class QuickTimeVideo
{
    private static readonly string[] quickTimeFileExtensions = [".mov", ".qt"];

    public FileInfo FileInfo { get; }

    private QuickTimeVideo(FileInfo fileInfo) => FileInfo = fileInfo;

    public static Result<QuickTimeVideo> Create(string path)
    {
        try
        {
            // Erstelle ein FileInfo-Objekt
            var fileInfo = new FileInfo(path);

            // Prüfe, ob die Datei existiert
            if (!fileInfo.Exists)
                return Result.Failure<QuickTimeVideo>("File not found.");

            // Prüfe, ob die Datei eine QuickTime-Datei ist
            if (!quickTimeFileExtensions.Contains(fileInfo.Extension))
                return Result.Failure<QuickTimeVideo>("File is not a QuickTime file.");

            // Rückgabe des FileInfo-Objekts
            return new QuickTimeVideo(fileInfo);
        }
        catch (Exception e)
        {
            return Result.Failure<QuickTimeVideo>($"Error on reading file info: {e.Message}");
        }
    }
}
