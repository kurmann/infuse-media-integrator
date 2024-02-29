using CSharpFunctionalExtensions;
using Kurmann.InfuseMediaIntegrator.Entities.Elementary;
using Kurmann.InfuseMediaIntegrator.Entities.MediaFileTypes;

namespace Kurmann.InfuseMediaIntegrator.Entities;

/// <summary>
/// Repräsentiert eine MPEG4-Video-Datei mit eingebetteten Metadaten.
/// Enthält die Logik um die Metadaten auszulesen.
/// </summary>
public class MediaFileMetadata
{
    /// <summary>
    /// Der Titel des Videos.
    /// </summary>
    public string Title { get; }

    /// <summary>
    /// Der sortierbare Titel des Videos.
    /// Oft mit dem Iso-Datumsformat (YYYY-MM-DD) vorangestellt.
    /// </summary>
    public string TitleSort { get; }

    /// <summary>
    /// Die Beschreibung des Videos.
    /// </summary>
    public string Description { get; }

    /// <summary>
    /// Das Jahr des Videos. Bei den meisten Videos handelt es sich um das Aufnahmejahr.
    /// </summary>
    public uint? Year { get; }

    /// <summary>
    /// Das Album des Videos.
    /// </summary>
    public string Album { get; }

    /// <summary>
    /// Das Titelbild (Artwork) des Videos.
    /// </summary>
    public byte[]? Artwork { get; }

    /// <summary>
    /// Der MIME-Typ des Titelbilds (Artwork) des Videos.
    /// </summary>
    public string? ArtworkMimeType { get; }

    /// <summary>
    /// Die Dateiendung des Titelbilds (Artwork) des Videos.
    /// </summary>
    public string? ArtworkExtension { get; }

    private MediaFileMetadata(string title,
                              string titleSort,
                              string description,
                              uint? year,
                              string album,
                              byte[]? artwork,
                              string? artworkMimeType,
                              string? artworkExtension)
    {
        Title = title;
        TitleSort = titleSort;
        Description = description;
        Year = year;
        Album = album;
        Artwork = artwork;
        ArtworkMimeType = artworkMimeType;
        ArtworkExtension = artworkExtension;
    }

    public static Result<MediaFileMetadata> Create(string? file)
    {
        // Prüfe, ob der Pfad leer ist
        if (string.IsNullOrWhiteSpace(file))
            return Result.Failure<MediaFileMetadata>("Path is empty.");

        // Prüfe, ob die Datei existiert
        var fileInfo = FilePathInfo.Create(file);
        if (fileInfo.IsFailure)
            return Result.Failure<MediaFileMetadata>($"Error on reading file info: {fileInfo.Error}");

        // Ermittle den Medientyp der Datei
        var fileType = MediaFileTypeDetector.GetMediaFile(fileInfo.Value);
        if (fileType.IsFailure)
            return Result.Failure<MediaFileMetadata>($"Error on reading file info: {fileType.Error}");

        // Unterscheide anhand des Medientyps, wie die Metadaten gelesen werden sollen
        return fileType.Value switch
        {
            Mpeg4Video mpeg4Video => Create(mpeg4Video),
            QuickTimeVideo quickTimeVideo => Create(quickTimeVideo),
            JpegImage _ => Result.Failure<MediaFileMetadata>("Metadata reading from image file not supported."),
            _ => Result.Failure<MediaFileMetadata>("File type not supported.")
        };
    }

    /// <summary>
    /// Erstellt ein Mpeg4VideoWithMetadata-Objekt aus einem Mpeg4Video-Objekt.
    /// </summary>
    /// <param name="mpeg4Video"></param>
    /// <returns></returns>
    public static Result<MediaFileMetadata> Create(Mpeg4Video mpeg4Video) => GetMetadataWithArtworkImage(mpeg4Video.FilePath.FilePath);

    public static Result<MediaFileMetadata> Create(QuickTimeVideo quickTimeVideo) => GetMetadataWithArtworkImage(quickTimeVideo.FilePath.FilePath);

    private static Result<MediaFileMetadata> GetMetadataWithArtworkImage(string? filePath)
    {
        // Prüfe, ob der Pfad leer ist
        if (string.IsNullOrWhiteSpace(filePath))
            return Result.Failure<MediaFileMetadata>("Path is empty.");

        // Erstelle ein TagLib-Objekt
        var tagLibFile = TagLib.File.Create(filePath);

        // Lies das Titelbild (Artwork) aus den Metadaten
        var tagLibPicture = tagLibFile.Tag.Pictures.ElementAtOrDefault(0);
        byte[]? artwork = tagLibPicture?.Data.Data;
        string? artworkMimeType = tagLibPicture?.MimeType;
        string? artworkExtension = MimeTypeToExtensionMapping.Create(artworkMimeType).GetValueOrDefault()?.Extension;

        // Lies die restlichen Metadaten aus und erstelle ein Mpeg4VideoWithMetadata-Objekt
        return new MediaFileMetadata(title: tagLibFile.Tag.Title,
                                     titleSort: tagLibFile.Tag.TitleSort,
                                     description: tagLibFile.Tag.Description,
                                     year: tagLibFile.Tag.Year,
                                     album: tagLibFile.Tag.Album,
                                     artwork: artwork,
                                     artworkMimeType: artworkMimeType,
                                     artworkExtension: artworkExtension);
    }
}