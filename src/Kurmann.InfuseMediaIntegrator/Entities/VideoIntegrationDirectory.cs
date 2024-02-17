using CSharpFunctionalExtensions;

namespace Kurmann.InfuseMediaIntegrator.Entities;

/// <summary>
/// Repräsentiert eine Sammlung von MPEG4-Video-Dateien mit eingebetteten Metadaten.
/// </summary>
public class VideoIntegrationDirectory(IEnumerable<Mpeg4Video> mpeg4VideoFiles,
                                       IEnumerable<QuickTimeVideo> quickTimeVideoFiles,
                                       IEnumerable<NotSupportedFile> notSupportedFiles)
{
    public IReadOnlyList<Mpeg4Video> Mpeg4VideoFiles { get; } = mpeg4VideoFiles.ToList();
    public IReadOnlyList<QuickTimeVideo> QuickTimeVideoFiles { get; } = quickTimeVideoFiles.ToList();
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
                return Result.Failure<VideoIntegrationDirectory>($"Directory {directoryInfo.FullName} not found.");
            }

            // Lese alle Dateien im Verzeichnis
            var files = directoryInfo.GetFiles();

            // Filtere die MPEG4-Dateien
            var mpeg4VideoFiles = new List<Mpeg4Video>();
            var quickTimeVideoFiles = new List<QuickTimeVideo>();
            var notSupportedFiles = new List<NotSupportedFile>();
            foreach (var file in files)
            {
                // Erstelle ein Mpeg4VideoFileWithEmbeddedMetadata-Objekt
                var mpeg4VideoFile = Mpeg4Video.Create(file.FullName);

                // Prüfe, ob das Objekt erstellt werden konnte, und füge es der Liste hinzu
                if (mpeg4VideoFile.IsFailure)
                {
                    if (mpeg4VideoFile.IsSuccess)
                    {

                        mpeg4VideoFiles.Add(mpeg4VideoFile.Value);     
                    }
                
                // Erstelle ein QuickTimeVideo-Objekt
                var quickTimeVideoFile = QuickTimeVideo.Create(file.FullName);

                // Prüfe, ob das Objekt erstellt werden konnte, und füge es der Liste hinzu
                if (quickTimeVideoFile.IsFailure)
                {
                    if (quickTimeVideoFile.IsSuccess)
                        quickTimeVideoFiles.Add(quickTimeVideoFile.Value);
                }

                // Erstelle ein NotSupportedFile-Objekt
                var notSupportedFile = NotSupportedFile.Create(file.FullName, $"File '{file.Name}' is not supported.");

                // Prüfe, ob das Objekt erstellt werden konnte, und füge es der Liste hinzu
                if (notSupportedFile.IsFailure)
                {
                    if (notSupportedFile.IsSuccess)
                        notSupportedFiles.Add(notSupportedFile.Value);
                }
                
                // Wenn das NotSupportedFile-Objekt nicht erstellt werden konnte, gib eine Fehlermeldung aus
                if (notSupportedFile.IsFailure)
                    return Result.Failure<VideoIntegrationDirectory>($"Error on reading file info: {notSupportedFile.Error}");

            }

            // Rückgabe des VideoIntegrationDirectory-Objekts
            return Result.Success(new VideoIntegrationDirectory(mpeg4VideoFiles, quickTimeVideoFiles, notSupportedFiles));
        }
        catch (Exception e)
        {
            return Result.Failure<VideoIntegrationDirectory>($"Error on reading directory info: {e.Message}");
        }
    }
}
