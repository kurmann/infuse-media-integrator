using CSharpFunctionalExtensions;
using Kurmann.InfuseMediaIntegrator.Entities;
using Kurmann.InfuseMediaIntegrator.Entities.MediaLibrary;
using Kurmann.InfuseMediaIntegrator.Module.Queries;

namespace Kurmann.InfuseMediaIntegrator.Module.Queries;

public class MetadataFromFileQuery(string? filePath) : IQueryService<MediaFileMetadata>
{
    private string? FilePath { get; } = filePath;

    public Result<MediaFileMetadata> Execute()
    {
        return GetMetadataWithArtworkImage(FilePath);
    }

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
        return MediaFileMetadata.Create(
            tagLibFile.Tag.Title,
            tagLibFile.Tag.TitleSort,
            tagLibFile.Tag.Description,
            tagLibFile.Tag.Year,
            tagLibFile.Tag.Album,
            artwork,
            artworkMimeType,
            artworkExtension
        );
    }
}