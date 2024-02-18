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
        // Erstelle ein Mpeg4VideoWithMetadata-Objekt
        var mpeg4VideoWithMetadata = Mpeg4VideoWithMetadata.Create(Mpeg4VideoPath);
        if (mpeg4VideoWithMetadata.IsFailure)
        {
            return Result.Failure(mpeg4VideoWithMetadata.Error);
        }

        // Erstelle ein Fanart-Infuse-Image
        // todo: Implementierung

        // Rückgabe des Ergebnisses
        return Result.Success();
    }
}
