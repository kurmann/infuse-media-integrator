using CSharpFunctionalExtensions;
using Kurmann.InfuseMediaIntegrator.Entities.Elementary;

namespace Kurmann.InfuseMediaIntegrator.Entities.MediaFileTypes
{
    public class JpegImage : IMediaFileType
    {
        public FilePathInfo FilePath { get; }

        public static readonly string[] FileExtensions = [".jpg", ".jpeg", ".jpe", ".jif", ".jfif", ".jfi"];

        private JpegImage(FilePathInfo filePath) => FilePath = filePath;

        public static Result<JpegImage> Create(string? path)
        {
            try
            {
                // Erstelle ein FilePathInfo-Objekt
                var fileInfo = FilePathInfo.Create(path);
                if (fileInfo.IsFailure)
                    return Result.Failure<JpegImage>($"Error on reading file info: {fileInfo.Error}");

                // Prüfe anhand der Dateiendung, ob es sich um eine JPEG-Datei handelt
                var extension = Path.GetExtension(fileInfo.Value.FilePath).ToLowerInvariant();
                if (!FileExtensions.Contains(extension))
                    return Result.Failure<JpegImage>("File is not a JPEG image.");

                return new JpegImage(fileInfo.Value);
            }
            catch (Exception e)
            {
                return Result.Failure<JpegImage>($"Error on reading file info: {e.Message}");
            }
        }
    }
}