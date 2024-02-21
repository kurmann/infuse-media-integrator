using CSharpFunctionalExtensions;

namespace Kurmann.InfuseMediaIntegrator.Entities;

/// <summary>
/// Repräsentiert eine Sammlung von MPEG4-Video-Dateien mit eingebetteten Metadaten.
/// </summary>
public class VideoIntegrationDirectory(IEnumerable<Mpeg4Video> mpeg4VideoFiles,
                                       IEnumerable<QuickTimeVideo> quickTimeVideoFiles,
                                       IEnumerable<NotSupportedFile> notSupportedFiles,
                                       IEnumerable<FileInfo> jpegFiles)
{
    public IReadOnlyList<Mpeg4Video> Mpeg4VideoFiles { get; } = mpeg4VideoFiles.ToList();
    public IReadOnlyList<QuickTimeVideo> QuickTimeVideoFiles { get; } = quickTimeVideoFiles.ToList();
    public IReadOnlyList<NotSupportedFile> NotSupportedFiles { get; } = notSupportedFiles.ToList();
    public IReadOnlyList<FileInfo> JpegFiles { get; } = jpegFiles.ToList();

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
            var jpegFiles = new List<FileInfo>();
            foreach (var file in files)
            {
                // Prüfe, ob die Datei existiert
                if (!file.Exists)
                {
                    return Result.Failure<VideoIntegrationDirectory>($"File not found: {file.FullName}");
                }

                // Ermittle den unterstützten Video-Dateityp
                var fileType = SupportedVideoFileType.Create(file.FullName);
                if (fileType.IsFailure)
                {
                    return Result.Failure<VideoIntegrationDirectory>($"Error on reading file info: {fileType.Error}");
                }

                // Füge das Video-Objekt der entsprechenden Liste hinzu anhand des Dateityps
                switch (fileType.Value.Type)
                {
                    case VideoFileType.Mpeg4:
                        Mpeg4Video.Create(file.FullName).Tap(mpeg4VideoFiles.Add);
                        break;
                    case VideoFileType.QuickTime:
                        QuickTimeVideo.Create(file.FullName).Tap(quickTimeVideoFiles.Add);
                        break;
                    case VideoFileType.Jpeg:
                        jpegFiles.Add(file);
                        break;
                    case VideoFileType.NotSupported:
                        // Retourniere eine NotSupportedFile-Instanz mit der Datei und dem Grund, dass die Dateiendung nicht unterstützt wird
                        NotSupportedFile.Create(file.FullName, $"File extension not supported.").Tap(notSupportedFiles.Add);
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
