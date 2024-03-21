using CSharpFunctionalExtensions;
using Kurmann.InfuseMediaIntegrator.Entities.Elementary;

namespace Kurmann.InfuseMediaIntegrator.Entities.MediaFileTypes;

/// <summary>
/// Erkennt den Medientyp einer Datei.
/// Gib ein IMediaFileType-Objekt zurück, das den Medientyp der Datei repräsentiert.
/// </summary>
public class MediaFileTypeDetector
{
    public static Result<IMediaFileType> GetMediaFile(string path)
    {
        try
        {
            // Erstelle ein FilePathInfo-Objekt
            var filePath = FilePathInfo.Create(path);
            if (filePath.IsFailure)
                return Result.Failure<IMediaFileType>($"Error on reading file info: {filePath.Error}");

            return GetMediaFileType(path, filePath);

        }
        catch (Exception e)
        {
            return Result.Failure<IMediaFileType>($"Error on reading file info: {e.Message}");
        }
    }

    public static Result<List<IMediaFileType>> GetMediaFile(IEnumerable<string> paths)
    {
        var mediaFiles = new List<IMediaFileType>();
        foreach (var path in paths)
        {
            var mediaFile = GetMediaFile(path);
            if (mediaFile.IsFailure)
                return Result.Failure<List<IMediaFileType>>($"Error on reading file info: {mediaFile.Error}");
            mediaFiles.Add(mediaFile.Value);
        }
        return mediaFiles;
    }

    public static Result<List<IMediaFileType>> GetMediaFiles(IEnumerable<string> paths)
    {
        var mediaFiles = new List<IMediaFileType>();
        foreach (var path in paths)
        {
            var mediaFile = GetMediaFile(path);
            if (mediaFile.IsFailure)
                return Result.Failure<List<IMediaFileType>>($"Error on reading file info: {mediaFile.Error}");
            mediaFiles.Add(mediaFile.Value);
        }
        return mediaFiles;
    }

    private static Result<IMediaFileType> GetMediaFileType(string path, Result<FilePathInfo> filePath)
    {
        // Lies die Dateiendung
        var extension = Path.GetExtension(filePath.Value.FilePath).ToLowerInvariant();

        // Gib das passende IMediaFileType-Objekt zurück
        if (Mpeg4Video.FileExtensions.Contains(extension))
        {
            var mpeg4Video = Mpeg4Video.Create(path);
            if (mpeg4Video.IsFailure)
                return Result.Failure<IMediaFileType>($"Error on reading file info: {mpeg4Video.Error}");
            return mpeg4Video.Value;
        }
        if (QuickTimeVideo.FileExtensions.Contains(extension))
        {
            var quickTimeVideo = QuickTimeVideo.Create(path);
            if (quickTimeVideo.IsFailure)
                return Result.Failure<IMediaFileType>($"Error on reading file info: {quickTimeVideo.Error}");
            return quickTimeVideo.Value;
        }
        if (JpegImage.FileExtensions.Contains(extension))
        {
            var jpegImage = JpegImage.Create(path);
            if (jpegImage.IsFailure)
                return Result.Failure<IMediaFileType>($"Error on reading file info: {jpegImage.Error}");
            return jpegImage.Value;
        }
        var notSupportedFile = NotSupportedFile.Create(path);
        if (notSupportedFile.IsFailure)
            return Result.Failure<IMediaFileType>($"Error on reading file info: {notSupportedFile.Error}");
        return notSupportedFile.Value;
    }
}
