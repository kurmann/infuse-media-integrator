using CSharpFunctionalExtensions;

namespace Kurmann.InfuseMediaIntegrator.Entities;

public class Mpeg4VideoFileWithEmbeddedMetadata
{
    private static readonly string[] mpeg4FileExtensions = [".mp4", ".m4v"];

    public FileInfo FileInfo { get; }

    private Mpeg4VideoFileWithEmbeddedMetadata(FileInfo fileInfo) => FileInfo = fileInfo;

    public static Result<FileInfo> Create(string path)
    {
        try
        {
            // Erstelle ein FileInfo-Objekt
            var fileInfo = new FileInfo(path);

            // Prüfe, ob die Datei existiert
            if (!fileInfo.Exists)
                return Result.Failure<FileInfo>("File not found.");

            // Prüfe, ob die Datei eine MPEG4-Datei ist
            if (!mpeg4FileExtensions.Contains(fileInfo.Extension))
                return Result.Failure<FileInfo>("File is not a MPEG4 file.");

            // Rückgabe des FileInfo-Objekts
            return Result.Success(fileInfo);
        }
        catch (Exception e)
        {
            return Result.Failure<FileInfo>($"Error on reading file info: {e.Message}");
        }
    }
}