using CSharpFunctionalExtensions;
using Kurmann.InfuseMediaIntegrator.Entities.Elementary;

namespace Kurmann.InfuseMediaIntegrator.Entities.MediaLibrary;

/// <summary>
/// Repräsentiert einen gültigen Dateinamen eines Videos in der Mediathek.
/// Entspricht dem Format "{Aufnahmedatum im ISO-Format} {Titel}.{Dateierweiterung}".
/// </summary>
public class LibraryVideoFileName
{
    /// <summary>
    /// Der Dateiname.
    /// </summary>
    public FileNameInfo FileName { get; }

    /// <summary>
    /// Das Aufnahmedatum, das im Dateinamen enthalten ist.
    /// </summary>
    public DateOnly RecordingDate { get; }

    private LibraryVideoFileName(FileNameInfo fileName, DateOnly recordingDate)
    {
        FileName = fileName;
        RecordingDate = recordingDate;
    }

    /// <summary>
    /// Erstellt ein LibraryVideoFileName-Objekt.
    /// </summary>
    /// <param name="fileName"></param>
    /// <returns></returns>
    public static Result<LibraryVideoFileName> Create(string? fileName)
    {
        // Erstelle ein FileNameInfo-Objekt
        var fileNameInfo = FileNameInfo.Create(fileName);
        if (fileNameInfo.IsFailure)
            return Result.Failure<LibraryVideoFileName>($"Error on reading file info: {fileNameInfo.Error}");

        return Create(fileNameInfo.Value);
    }

    /// <summary>
    /// Erstellt ein LibraryVideoFileName-Objekt.
    /// </summary>
    /// <param name="fileName"></param>
    /// <returns></returns>
    public static Result<LibraryVideoFileName> Create(FileNameInfo fileName)
    {
        // Lies das Datum aus dem Dateinamen
        var fileNameWithDateInfo = FileNameWithDateInfo.Create(fileName);
        if (fileNameWithDateInfo.IsFailure)
            return Result.Failure<LibraryVideoFileName>($"Error on reading file info: {fileNameWithDateInfo.Error}");

        // Ermittle das Zielformat des Dateinamens
        var targetFileName = GetTargetFileName(fileNameWithDateInfo.Value);
        if (targetFileName.IsFailure)
            return Result.Failure<LibraryVideoFileName>($"Error on reading file info: {targetFileName.Error}");

        return new LibraryVideoFileName(targetFileName.Value, fileNameWithDateInfo.Value.Date);
    }

    public static Result<LibraryVideoFileName> Create(FileNameInfo fileName, DateOnly recordingDate)
    {
        var fileNameWithDateInfo = FileNameWithDateInfo.Create(fileName.FileName);
        if (fileNameWithDateInfo.IsFailure)
            return Result.Failure<LibraryVideoFileName>($"Error on reading file info: {fileNameWithDateInfo.Error}");

        return new LibraryVideoFileName(fileName, recordingDate);
    }

    private static Result<FileNameInfo> GetTargetFileName(FileNameWithDateInfo fileNameWithDateInfo)
    {
        // Wenn das Datum zu Beginn oder am Ende des Dateinamens steht
        if (fileNameWithDateInfo.IsDateAtStart || fileNameWithDateInfo.IsDateAtEnd)
        {
            // Entferne das Datum aus dem Dateinamen indem der gefundene Datums-Sting entfernt wird
            var foundDateString = fileNameWithDateInfo.DateString;
            var titleWithoutDate = fileNameWithDateInfo.FileName.FileName.Replace(foundDateString, string.Empty);

            // Erstelle ein FileNameInfo-Objekt mit dem getrimmten Dateinamen
            var fileNameInfo = FileNameInfo.Create(titleWithoutDate.Trim());
            if (fileNameInfo.IsFailure)
                throw new InvalidOperationException($"Error on reading file info: {fileNameInfo.Error}");

            // Retourniere den Titel ohne das Datum
            return fileNameInfo.Value;
        }

        // Wenn das Datum in der Mitte des Dateinamens steht, dann lasse das Datum im Titel
        return fileNameWithDateInfo.FileName;
    }
}