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
    public string? Description { get; }

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

    private MediaFileMetadata(string title, string titleSort, string description, uint year, string album, byte[]? artwork, string? artworkMimeType, string? artworkExtension)
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

    public static Result<MediaFileMetadata> Create(string title, string? titleSort, string? description, uint? year, string? album, byte[]? artwork, string? artworkMimeType, string? artworkExtension)
    {
        // Prüfe, ob der Titel leer ist
        if (string.IsNullOrWhiteSpace(title))
            return Result.Failure<MediaFileMetadata>("Title is empty.");

        return new MediaFileMetadata(title, titleSort, description, year, album, artwork, artworkMimeType, artworkExtension);
    }
}