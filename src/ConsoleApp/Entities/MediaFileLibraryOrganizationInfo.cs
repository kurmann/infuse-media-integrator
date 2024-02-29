using CSharpFunctionalExtensions;
using Kurmann.InfuseMediaIntegrator.Entities.Elementary;
using Kurmann.InfuseMediaIntegrator.Entities.MediaFileTypes;
using Kurmann.InfuseMediaIntegrator.Entities.MediaLibrary;

namespace Kurmann.InfuseMediaIntegrator.Entities;

/// <summary>
/// Enthält Informationen um eine Mediendatei in einer Mediathek zu organisieren.
/// Nimmt eine Mediendatei entgegen und gibt Informationen zurück, wie die Datei in der Mediathek organisiert werden soll.
/// Die Mediendatei hat Informationen über den Dateityp und die Metadaten.
/// Bei der Organisation wird der Dateityp und die Metadaten berücksichtigt, wobei die Metadaten bevorzugt werden.
/// </summary>
public class MediaFileLibraryOrganizationInfo
{
    /// <summary>
    /// Die Mediendatei.
    /// </summary>
    public IMediaFileType MediaFileType { get; }

    /// <summary>
    /// Der Dateiname der Zieldatei.
    /// </summary>
    public FileNameInfo TargetFileName { get; }

    /// <summary>
    /// Das Verzeichnis, in das die Datei verschoben werden soll.
    /// </summary>
    public DirectoryPathInfo TargetSubDirectory { get; }

    private MediaFileLibraryOrganizationInfo(IMediaFileType mediaFileType, FileNameInfo targetFileName, DirectoryPathInfo targetSubDirectory)
    {
        MediaFileType = mediaFileType;
        TargetFileName = targetFileName;
        TargetSubDirectory = targetSubDirectory;
    }

    /// <summary>
    /// Erstellt ein MediaFileLibraryOrganizationInfo-Objekt.
    /// </summary>
    /// <param name="mediaFile"></param>
    /// <param name="rootDirectory"></param>
    /// <returns></returns>
    public static Result<MediaFileLibraryOrganizationInfo> Create(IMediaFileType? mediaFile, string rootDirectory)
    {
        // Prüfe, ob die Mediendatei vorhanden ist
        if (mediaFile == null)
            return Result.Failure<MediaFileLibraryOrganizationInfo>("Media file is null.");

        // Lies in den Metadaten, ob ein Kategorien-Pfad vorhanden ist, bspw. "Family/2024".
        // In den Metadaten entspricht das dem Album.
        var categoryPath = mediaFile.Metadata?.Album;

        // Unterscheide zwischen Medientypen
        return mediaFile switch
        {
            Mpeg4Video mpeg4Video => Create(mpeg4Video, rootDirectory),
            QuickTimeVideo quickTimeVideo => Create(quickTimeVideo),
            JpegImage jpegImage => Create(jpegImage),
            _ => Result.Failure<MediaFileLibraryOrganizationInfo>("Media file type is not supported.")
        };
    }

    public static Result<MediaFileLibraryOrganizationInfo> Create(Mpeg4Video mpeg4Video, string rootDirectory)
    {
        // Ermittle das Zielformat des Dateinamens
        var targetFileName = LibraryVideoFileName.Create(mpeg4Video.FilePath.FileName);
        if (targetFileName.IsFailure)
            return Result.Failure<MediaFileLibraryOrganizationInfo>($"Error on reading file info: {targetFileName.Error}");

        // Ermittle das Zielverzeichnis
        var targetSubDirectory = LibraryFileSubDirectoryPath.Create(mpeg4Video.FilePath.FilePath, rootDirectory);
        if (targetSubDirectory.IsFailure)
            return Result.Failure<MediaFileLibraryOrganizationInfo>($"Error on reading file info: {targetSubDirectory.Error}");
    }

    public static Result<MediaFileLibraryOrganizationInfo> Create(QuickTimeVideo quickTimeVideo, string rootDirectory)
    {
        // Ermittle das Zielformat des Dateinamens
        var targetFileName = LibraryVideoFileName.Create(quickTimeVideo.FilePath.FileName);
        if (targetFileName.IsFailure)
            return Result.Failure<MediaFileLibraryOrganizationInfo>($"Error on reading file info: {targetFileName.Error}");

        // Ermittle das Zielverzeichnis
        var targetSubDirectory = LibraryFileSubDirectoryPath.Create(quickTimeVideo.FilePath.FilePath, rootDirectory);
        if (targetSubDirectory.IsFailure)
            return Result.Failure<MediaFileLibraryOrganizationInfo>($"Error on reading file info: {targetSubDirectory.Error}");
    }

    public static Result<MediaFileLibraryOrganizationInfo> Create(JpegImage jpegImage, string rootDirectory)
    {
        // todo: implement art work for jpeg images with "-fanart" in the file name
        var targetFileName = jpegImage.FilePath.FileName;

        // Ermittle das Zielverzeichnis
        var targetSubDirectory = LibraryFileSubDirectoryPath.Create(jpegImage.FilePath.FilePath, rootDirectory);
        if (targetSubDirectory.IsFailure)
            return Result.Failure<MediaFileLibraryOrganizationInfo>($"Error on reading file info: {targetSubDirectory.Error}");
    }
}