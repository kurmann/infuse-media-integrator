namespace Kurmann.InfuseMediaIntegrator.Commands;

using CSharpFunctionalExtensions;
using Kurmann.InfuseMediaIntegrator.Entities;

/// <summary>
/// Erstellt ein Fanart-Infuse-Image aus einem MP4-Video.
/// Diese haben den gleichen Dateinamen wie das MP4-Video, jedoch mit der Endung "-fanart.jpg".
/// </summary>
public class CreateFanartInfuseImageCommand(string? mpeg4VideoPath, string? outputDirectory) : ICommand
{
    public string? Mpeg4VideoPath { get; } = mpeg4VideoPath;
    public string? OutputDirectory { get; } = outputDirectory;

    public Result Execute()
    {
        // Prüfe ob das Ausgabeverzeichniss leer ist
        if (string.IsNullOrWhiteSpace(OutputDirectory))
        {
            return Result.Failure("Das Ausgabeverzeichnis ist nicht definiert.");
        }

        // Prüfe ob das Ausgabeverzeichniss existiert und erstelle es falls nicht
        if (!Directory.Exists(OutputDirectory))
        {
            try
            {
                Directory.CreateDirectory(OutputDirectory);
            }
            catch (Exception ex)
            {
                return Result.Failure($"Fehler beim Erstellen des Ausgabeverzeichnisses '{OutputDirectory}': {ex.Message}");
            }
        }

        // Erstelle ein Mpeg4VideoWithMetadata-Objekt
        var mpeg4VideoWithMetadata = Mpeg4VideoWithMetadata.Create(Mpeg4VideoPath);
        if (mpeg4VideoWithMetadata.IsFailure)
        {
            return Result.Failure(mpeg4VideoWithMetadata.Error);
        }

        // Lies den Bytestream und die Erweiterung des Titelbilds (Artwork) aus den Metadaten
        byte[]? artwork = mpeg4VideoWithMetadata.Value.Artwork;
        string? artworkExtension = mpeg4VideoWithMetadata.Value.ArtworkExtension;

        // Prüfe ob ein Titelbild (Artwork) vorhanden ist
        if (artwork == null)
        {
            return Result.Failure("Das Titelbild (Artwork) ist nicht vorhanden.");
        }

        // Ermittle den Zielpfad des Fanart-Infuse-Image im Zielverzeichnis mit dem gleichen Dateinamen wie das MP4 und der Endung "-fanart.xxx" 
        // Die Dateiendung wird aus dem MIME-Typ des Titelbilds (Artwork) abgeleitet
        var fanartInfuseImagePath = Path.Combine(OutputDirectory, $"{Path.GetFileNameWithoutExtension(Mpeg4VideoPath)}-fanart.{artworkExtension}");

        // Schreibe das Titelbild (Artwork) in das Zielverzeichnis
        File.WriteAllBytes(fanartInfuseImagePath, artwork);

        // Rückgabe des Ergebnisses
        return Result.Success();
    }
}
