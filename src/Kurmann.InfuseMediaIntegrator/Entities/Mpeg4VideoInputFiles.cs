using CSharpFunctionalExtensions;

namespace Kurmann.InfuseMediaIntegrator.Entities;

/// <summary>
/// Repräsentiert eine Sammlung von MPEG4-Video-Dateien mit eingebetteten Metadaten.
/// </summary>
public class Mpeg4VideoInputFiles(IEnumerable<Mpeg4Video> mpeg4VideoFiles)
{
    public IReadOnlyList<Mpeg4Video> Mpeg4VideoFiles { get; } = mpeg4VideoFiles.ToList();

    public static Result<Mpeg4VideoInputFiles> Create(string directoryPath)
    {
        try
        {
            // Erstelle ein DirectoryInfo-Objekt
            var directoryInfo = new DirectoryInfo(directoryPath);

            // Prüfe, ob das Verzeichnis existiert
            if (!directoryInfo.Exists)
                return Result.Failure<Mpeg4VideoInputFiles>("Directory not found.");

            // Lese alle Dateien im Verzeichnis
            var files = directoryInfo.GetFiles();

            // Filtere die MPEG4-Dateien
            var mpeg4VideoFiles = new List<Mpeg4Video>();
            foreach (var file in files)
            {
                // Erstelle ein Mpeg4VideoFileWithEmbeddedMetadata-Objekt
                var mpeg4VideoFile = Mpeg4Video.Create(file.FullName);

                // Prüfe, ob das Objekt erstellt werden konnte, falls nicht, ignoriere die Datei
                if (mpeg4VideoFile.IsFailure)
                    continue;

                // Füge das Objekt der Liste hinzu
                mpeg4VideoFiles.Add(mpeg4VideoFile.Value);
            }

            // Rückgabe des Mpeg4VideoInputFiles-Objekts
            return Result.Success(new Mpeg4VideoInputFiles(mpeg4VideoFiles));
        }
        catch (Exception e)
        {
            return Result.Failure<Mpeg4VideoInputFiles>($"Error on reading directory info: {e.Message}");
        }
    }
}