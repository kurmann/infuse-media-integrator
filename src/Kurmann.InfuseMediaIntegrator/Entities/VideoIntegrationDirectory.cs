using CSharpFunctionalExtensions;

namespace Kurmann.InfuseMediaIntegrator.Entities;

/// <summary>
/// Repräsentiert eine Sammlung von MPEG4-Video-Dateien mit eingebetteten Metadaten.
/// </summary>
public class VideoIntegrationDirectory(IEnumerable<Mpeg4Video> mpeg4VideoFiles, IEnumerable<QuickTimeVideo> quickTimeVideoFiles)
{
    public IReadOnlyList<Mpeg4Video> Mpeg4VideoFiles { get; } = mpeg4VideoFiles.ToList();
    public IReadOnlyList<QuickTimeVideo> QuickTimeVideoFiles { get; } = quickTimeVideoFiles.ToList();

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
            foreach (var file in files)
            {
                // Erstelle ein Mpeg4VideoFileWithEmbeddedMetadata-Objekt
                var mpeg4VideoFile = Mpeg4Video.Create(file.FullName);

                // Prüfe, ob das Objekt erstellt werden konnte, falls nicht, ignoriere die Datei
                if (mpeg4VideoFile.IsFailure)
                    continue;

                // Füge das Objekt der Liste hinzu
                mpeg4VideoFiles.Add(mpeg4VideoFile.Value);

                // Erstelle ein QuickTimeVideoFileWithEmbeddedMetadata-Objekt
                var quickTimeVideoFile = QuickTimeVideo.Create(file.FullName);

                // Prüfe, ob das Objekt erstellt werden konnte, falls nicht, ignoriere die Datei
                if (quickTimeVideoFile.IsFailure)
                    continue;

                // Füge das Objekt der Liste hinzu
                quickTimeVideoFiles.Add(quickTimeVideoFile.Value);
            }

            // Rückgabe des VideoIntegrationDirectory-Objekts
            return Result.Success(new VideoIntegrationDirectory(mpeg4VideoFiles, quickTimeVideoFiles));
        }
        catch (Exception e)
        {
            return Result.Failure<VideoIntegrationDirectory>($"Error on reading directory info: {e.Message}");
        }
    }
}
