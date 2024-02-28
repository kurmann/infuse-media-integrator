using CSharpFunctionalExtensions;
using Kurmann.InfuseMediaIntegrator.Entities.Elementary;

namespace Kurmann.InfuseMediaIntegrator.Entities.MediaFileTypes;

/// <summary>
/// Repräsentiert eine nicht unterstützte Datei. Diese Dateien sind in der Mediathek nicht nutzbar,
/// werden aber dennoch in der Mediathek abgelegt.
/// </summary>
public class NotSupportedFile : IMediaFileType
{
    /// <summary>
    /// Der Dateipfad.
    /// </summary>
    public FilePathInfo FileInfo { get; }

    private NotSupportedFile(FilePathInfo fileInfo) => FileInfo = fileInfo;

    public static Result<NotSupportedFile> Create(string path, string reason)
    {
        try
        {
            // Erstelle ein FilePathInfo-Objekt
            var fileInfo = FilePathInfo.Create(path);
            if (fileInfo.IsFailure)
                return Result.Failure<NotSupportedFile>($"Error on reading file info: {fileInfo.Error}");

            // Rückgabe des FileInfo-Objekts
            return new NotSupportedFile(fileInfo);
        }
        catch (Exception e)
        {
            return Result.Failure<NotSupportedFile>($"Error on reading file info: {e.Message}");
        }
    }
}
