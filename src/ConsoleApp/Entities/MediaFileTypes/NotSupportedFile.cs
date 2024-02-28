using CSharpFunctionalExtensions;

namespace Kurmann.InfuseMediaIntegrator.Entities.MediaFileTypes;

public class NotSupportedFile
{
    /// <summary>
    /// Repräsentiert eine nicht unterstützte Datei.
    /// </summary>
    public FileInfo FileInfo { get; }

    /// <summary>
    /// Der Grund, warum die Datei nicht unterstützt wird.
    /// </summary>
    public string Reason { get; }

    private NotSupportedFile(FileInfo fileInfo, string reason)
    {
        FileInfo = fileInfo;
        Reason = reason;
    }

    public static Result<NotSupportedFile> Create(string path, string reason)
    {
        try
        {
            // Erstelle ein FileInfo-Objekt
            var fileInfo = new FileInfo(path);

            // Prüfe, ob die Datei existiert
            if (!fileInfo.Exists)
                return Result.Failure<NotSupportedFile>("File not found.");

            // Rückgabe des FileInfo-Objekts
            return new NotSupportedFile(fileInfo, reason);
        }
        catch (Exception e)
        {
            return Result.Failure<NotSupportedFile>($"Error on reading file info: {e.Message}");
        }
    }
}
