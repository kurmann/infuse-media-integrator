using CSharpFunctionalExtensions;
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

    /// <summary>
    /// Das Mpeg4Video-Objekt.
    /// </summary>
    public Mpeg4Video Mpeg4Video { get; }

    private MediaFileMetadata(Mpeg4Video mpeg4Video,
                                   string title,
                                   string titleSort,
                                   string description,
                                   uint? year,
                                   string album,
                                   byte[]? artwork,
                                   string? artworkMimeType,
                                   string? artworkExtension)
    {
        Mpeg4Video = mpeg4Video;
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
        var mpeg4Video = Mpeg4Video.Create(file);
        if (mpeg4Video.IsFailure)
            return Result.Failure<MediaFileMetadata>(mpeg4Video.Error);

        return Create(mpeg4Video.Value);
    }

    public static Result<MediaFileMetadata> Create(Mpeg4Video mpeg4Video)
    {
        try
        {
            // Erstelle ein TagLib-Objekt
            var tagLibFile = TagLib.File.Create(mpeg4Video.FilePath.FilePath);

            // Lies das Titelbild (Artwork) aus den Metadaten
            var tagLibPicture = tagLibFile.Tag.Pictures.ElementAtOrDefault(0);
            byte[]? artwork = tagLibPicture?.Data.Data;
            string? artworkMimeType = tagLibPicture?.MimeType;
            string? artworkExtension = MimeTypeToExtensionMapping.Create(artworkMimeType).GetValueOrDefault()?.Extension;

            // Lies die restlichen Metadaten aus und erstelle ein Mpeg4VideoWithMetadata-Objekt
            return new MediaFileMetadata(mpeg4Video: mpeg4Video,
                                              title: tagLibFile.Tag.Title,
                                              titleSort: tagLibFile.Tag.TitleSort,
                                              description: tagLibFile.Tag.Description,
                                              year: tagLibFile.Tag.Year,
                                              album: tagLibFile.Tag.Album,
                                              artwork: artwork,
                                              artworkMimeType: artworkMimeType,
                                              artworkExtension: artworkExtension);
        }
        catch (Exception ex)
        {
            return Result.Failure<MediaFileMetadata>(ex.Message);
        }
    }
}