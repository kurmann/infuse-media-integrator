using CSharpFunctionalExtensions;

namespace Kurmann.InfuseMediaIntegrator.Entities;

/// <summary>
/// Repräsentiert eine MPEG4-Video-Datei mit eingebetteten Metadaten.
/// </summary>
public class Mpeg4VideoFileWithEmbeddedMetadata
{
    private static readonly string[] mpeg4FileExtensions = [".mp4", ".m4v"];

    public FileInfo FileInfo { get; }

    private Mpeg4VideoFileWithEmbeddedMetadata(FileInfo fileInfo) => FileInfo = fileInfo;

    public static Result<Mpeg4VideoFileWithEmbeddedMetadata> Create(string path)
    {
        try
        {
            // Erstelle ein FileInfo-Objekt
            var fileInfo = new FileInfo(path);

            // Prüfe, ob die Datei existiert
            if (!fileInfo.Exists)
                return Result.Failure<Mpeg4VideoFileWithEmbeddedMetadata>("File not found.");

            // Prüfe, ob die Datei eine MPEG4-Datei ist
            if (!mpeg4FileExtensions.Contains(fileInfo.Extension))
                return Result.Failure<Mpeg4VideoFileWithEmbeddedMetadata>("File is not a MPEG4 file.");

            // Rückgabe des FileInfo-Objekts
            return new Mpeg4VideoFileWithEmbeddedMetadata(fileInfo);
        }
        catch (Exception e)
        {
            return Result.Failure<Mpeg4VideoFileWithEmbeddedMetadata>($"Error on reading file info: {e.Message}");
        }
    }
}