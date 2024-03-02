using CSharpFunctionalExtensions;
using Kurmann.InfuseMediaIntegrator.Entities.Elementary;
using Kurmann.InfuseMediaIntegrator.Entities.MediaFileTypes;

namespace Kurmann.InfuseMediaIntegrator.Entities.MediaLibrary;

/// <summary>
/// Repräsentiert eine Gruppe von Medien-Dateien. Die Mediengruppe besteht typsicherweise aus:
/// - Einem Video
/// - Einem oder mehreren Bildern
/// - Einer Metadaten-Datei
/// Sie wird erkannt als eine Gruppe von Medien-Dateien, die den gleichen Dateinamen enthalten wie die Mediendatei.
/// Beispiel:
/// - Video: "2023-10-10 Wanderung auf den Napf.m4v"
/// - Bild: "2023-10-10 Wanderung auf den Napf.jpg"
/// - Bild: "2023-10-10 Wanderung auf den Napf-fanart.jpg"
/// - Metadaten: "2023-10-10 Wanderung auf den Napf.nfo"
/// </summary>
public class MediaGroup
{
    /// <summary>
    /// Die Medien-Dateien.
    /// </summary>
    public IReadOnlyList<IMediaFileType> MediaFiles { get; }

    private MediaGroup(IReadOnlyList<IMediaFileType> mediaFiles)
    {
        MediaFiles = mediaFiles;
        DirectoryPath = directoryPath;
    }

    /// <summary>
    /// Erstellt eine Mediengruppe, indem es die Medien-Dateien aus dem gegebenen Verzeichnis liest
    /// und anhand des Dateinamens gruppiert.
    /// </summary>
    /// <param name="directoryPath"></param>
    /// <returns></returns>
    public static Result<MediaGroup> Create(string directoryPath)
    {
        try
        {
            // Erstelle ein DirectoryPathInfo-Objekt
            var directoryPathInfo = DirectoryPathInfo.Create(directoryPath);
            if (directoryPathInfo.IsFailure)
                return Result.Failure<MediaGroup>($"Error on reading directory info: {directoryPathInfo.Error}");

            // Rückgabe des MediaGroup-Objekts
            return new MediaGroup(mediaFiles, directoryPathInfo.Value);
        }
        catch (Exception e)
        {
            return Result.Failure<MediaGroup>($"Error on reading directory info: {e.Message}");
        }
    }
}
