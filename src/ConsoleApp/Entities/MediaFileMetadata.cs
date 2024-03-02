using CSharpFunctionalExtensions;

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
    public string? TitleSort { get; }

    /// <summary>
    /// Die Beschreibung des Videos.
    /// </summary>
    public string? Description { get; private set; }

    /// <summary>
    /// Das Jahr des Videos. Bei den meisten Videos handelt es sich um das Aufnahmejahr.
    /// </summary>
    public uint? Year { get; private set; }

    /// <summary>
    /// Das Album des Videos.
    /// </summary>
    public string? Album { get; private set; }

    /// <summary>
    /// Das Titelbild (Artwork) des Videos.
    /// </summary>
    public byte[]? Artwork { get; private set; }

    /// <summary>
    /// Der MIME-Typ des Titelbilds (Artwork) des Videos.
    /// </summary>
    public string? ArtworkMimeType { get; private set; }

    /// <summary>
    /// Die Dateiendung des Titelbilds (Artwork) des Videos.
    /// </summary>
    public string? ArtworkExtension { get; private set; }

    private MediaFileMetadata(string title, string? titleSort, string? description, uint? year, string? album, byte[]? artwork, string? artworkMimeType, string? artworkExtension)
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

    private MediaFileMetadata(string title) => Title = title;

    public static Result<MediaFileMetadata> Create(string title)
    {
        if (string.IsNullOrWhiteSpace(title))
            return Result.Failure<MediaFileMetadata>("Title is empty.");

        return new MediaFileMetadata(title);
    }

    public static Result<MediaFileMetadata> Create(string title, string? titleSort, string? description, uint? year, string? album, byte[]? artwork, string? artworkMimeType, string? artworkExtension)
    {
        // Prüfe, ob der Titel leer ist
        if (string.IsNullOrWhiteSpace(title))
            return Result.Failure<MediaFileMetadata>("Title is empty.");

        return new MediaFileMetadata(title, titleSort, description, year, album, artwork, artworkMimeType, artworkExtension);
    }

    public void WithDescription(string description) => Description = description;
    public void WithYear(uint year) => Year = year;
    public void WithAlbum(string album) => Album = album;
    public void WithArtwork(byte[] artwork, string mimeType, string extension)
    {
        Artwork = artwork;
        ArtworkMimeType = mimeType;
        ArtworkExtension = extension;
    }
}