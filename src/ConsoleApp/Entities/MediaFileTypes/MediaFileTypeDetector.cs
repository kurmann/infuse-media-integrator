using CSharpFunctionalExtensions;
using Kurmann.InfuseMediaIntegrator.Entities.Elementary;

namespace Kurmann.InfuseMediaIntegrator.Entities.MediaFileTypes;

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
            var notSupportedFile = NotSupportedFile.Create(path, "File type not supported");
            if (notSupportedFile.IsFailure)
                return Result.Failure<IMediaFileType>($"Error on reading file info: {notSupportedFile.Error}");
            return notSupportedFile.Value;

        }
        catch (Exception e)
        {
            return Result.Failure<IMediaFileType>($"Error on reading file info: {e.Message}");
        }
    }
}

[Obsolete("Use IMediaFileType instead.")]
public enum VideoFileType
{
    NotSupported,
    QuickTime,
    Mpeg4,
    Jpeg
}
