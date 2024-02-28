using CSharpFunctionalExtensions;
using Kurmann.InfuseMediaIntegrator.Entities.MediaFileTypes;

namespace Kurmann.InfuseMediaIntegrator.Entities;

/// <summary>
/// Repräsentiert eine Sammlung von MPEG4-Video-Dateien mit eingebetteten Metadaten.
/// </summary>
public class VideoIntegrationDirectory(IEnumerable<Mpeg4Video> mpeg4VideoFiles,
                                       IEnumerable<QuickTimeVideo> quickTimeVideoFiles,
                                       IEnumerable<NotSupportedFile> notSupportedFiles,
                                       IEnumerable<JpegImage> jpegFiles)
{
    public IReadOnlyList<Mpeg4Video> Mpeg4VideoFiles { get; } = mpeg4VideoFiles.ToList();
    public IReadOnlyList<QuickTimeVideo> QuickTimeVideoFiles { get; } = quickTimeVideoFiles.ToList();
    public IReadOnlyList<JpegImage> JpegFiles { get; } = jpegFiles.ToList();
    public IReadOnlyList<NotSupportedFile> NotSupportedFiles { get; } = notSupportedFiles.ToList();
    public static Result<VideoIntegrationDirectory> Create(string directoryPath)
    {
        try
        {
            // Erstelle ein DirectoryInfo-Objekt
            var directoryInfo = new DirectoryInfo(directoryPath);

            // Prüfe, ob das Verzeichnis existiert
            if (!directoryInfo.Exists)
            {
                // Gib das nicht gefundene Verzeichnis zurück
                return Result.Failure<VideoIntegrationDirectory>($"Directory not found: {directoryInfo.FullName}");
            }

            // Lese alle Dateien im Verzeichnis
            var files = directoryInfo.GetFiles();

            // Filtere die MPEG4-Dateien
            var mpeg4VideoFiles = new List<Mpeg4Video>();
            var quickTimeVideoFiles = new List<QuickTimeVideo>();
            var notSupportedFiles = new List<NotSupportedFile>();
            var jpegFiles = new List<JpegImage>();
            foreach (var file in files)
            {
                // Prüfe, ob die Datei existiert
                if (!file.Exists)
                {
                    return Result.Failure<VideoIntegrationDirectory>($"File not found: {file.FullName}");
                }

                // Ermittle den Medientyp der Datei
                var fileType = MediaFileTypeDetector.GetMediaFile(file.FullName);
                if (fileType.IsFailure)
                {
                    return Result.Failure<VideoIntegrationDirectory>($"Error on reading file info: {fileType.Error}");
                }

                // Füge das Video-Objekt der entsprechenden Liste hinzu anhand des Dateityps
                switch (fileType.Value)
                {
                    case Mpeg4Video mpeg4Video:
                        mpeg4VideoFiles.Add(mpeg4Video);
                        break;
                    case QuickTimeVideo quickTimeVideo:
                        quickTimeVideoFiles.Add(quickTimeVideo);
                        break;
                    case JpegImage jpegImage:
                        jpegFiles.Add(jpegImage);
                        break;
                    case NotSupportedFile notSupportedFile:
                        notSupportedFiles.Add(notSupportedFile);
                        break;
                }
            }

            // Rückgabe des VideoIntegrationDirectory-Objekts
            return Result.Success(new VideoIntegrationDirectory(mpeg4VideoFiles, quickTimeVideoFiles, notSupportedFiles, jpegFiles));
        }
        catch (Exception e)
        {
            return Result.Failure<VideoIntegrationDirectory>($"Error on reading directory info: {e.Message}");
        }
    }
}
