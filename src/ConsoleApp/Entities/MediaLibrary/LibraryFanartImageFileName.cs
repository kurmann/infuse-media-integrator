using CSharpFunctionalExtensions;
using Kurmann.InfuseMediaIntegrator.Entities.Elementary;

namespace Kurmann.InfuseMediaIntegrator.Entities.MediaLibrary;

/// <summary>
/// Repräsentiert einen gültigen Dateinamen eines Fanart-Bildes in der Mediathek.
/// Entspricht dem Format "{Titel}-fanart.{Dateierweiterung}".
/// </summary>
public class LibraryFanartImageFileName
{
    /// <summary>
    /// Der Dateiname.
    /// </summary>
    public FileNameInfo FileName { get; }

    /// <summary>
    /// Das Suffix, das an den Dateinamen angehängt wird, um ein Fanart-Bild zu kennzeichnen.
    /// </summary>
    public const string FanartPrefix = "-fanart";

    private LibraryFanartImageFileName(FileNameInfo fileName) => FileName = fileName;

    public static Result<LibraryFanartImageFileName> Create(string? fileName)
    {
        // Erstelle ein FileNameInfo-Objekt
        var fileNameInfo = FileNameInfo.Create(fileName);
        if (fileNameInfo.IsFailure)
            return Result.Failure<LibraryFanartImageFileName>($"Error on reading file info: {fileNameInfo.Error}");

        return Create(fileNameInfo.Value);
    }

    public static Result<LibraryFanartImageFileName> Create(FileNameInfo fileName)
    {
        // Prüfe, ob der Dateiname mit "-fanart" endet
        if (!fileName.FileName.ToLowerInvariant().EndsWith("-fanart"))
            return Result.Failure<LibraryFanartImageFileName>("File name does not end with '-fanart'.");

        return new LibraryFanartImageFileName(fileName);
    }

    public static Result<LibraryFanartImageFileName> CreateWithAddedFanartPrefix(string fileName)
    {
        // Erstelle ein FileNameInfo-Objekt
        var fileNameInfo = FileNameInfo.Create(fileName);
        if (fileNameInfo.IsFailure)
            return Result.Failure<LibraryFanartImageFileName>($"Error on reading file info: {fileNameInfo.Error}");

        // Wenn der Dateiname bereits mit "-fanart" endet, wird der Dateiname unverändert zurückgegeben
        if (fileNameInfo.Value.FileNameWithoutExtension.ToLowerInvariant().EndsWith("-fanart"))
            return new LibraryFanartImageFileName(fileNameInfo.Value);

        // Füge das "-fanart"-Suffix hinzu
        var newFileName = fileNameInfo.Value.FileNameWithoutExtension + FanartPrefix + fileNameInfo.Value.Extension;
        return Create(newFileName);
    }
}