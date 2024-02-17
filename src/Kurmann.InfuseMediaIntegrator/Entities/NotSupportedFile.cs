using CSharpFunctionalExtensions;

namespace Kurmann.InfuseMediaIntegrator.Entities;

public class NotSupportedFile
{
    public FileInfo FileInfo { get; }

    private NotSupportedFile(FileInfo fileInfo) => FileInfo = fileInfo;

    public static Result<NotSupportedFile> Create(string path)
    {
        try
        {
            // Erstelle ein FileInfo-Objekt
            var fileInfo = new FileInfo(path);

            // Prüfe, ob die Datei existiert
            if (!fileInfo.Exists)
                return Result.Failure<NotSupportedFile>("File not found.");

            // Rückgabe des FileInfo-Objekts
            return new NotSupportedFile(fileInfo);
        }
        catch (Exception e)
        {
            return Result.Failure<NotSupportedFile>($"Error on reading file info: {e.Message}");
        }
    }
}
