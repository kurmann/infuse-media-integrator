using CSharpFunctionalExtensions;
using Kurmann.InfuseMediaIntegrator.Entities;
using Kurmann.InfuseMediaIntegrator.Queries;

namespace Kurmann.InfuseMediaIntegrator.Commands;

public class MetadataFromFileQuery(string? filePath) : IQueryService<MediaFileMetadata>
{
    public string? FilePath { get; } = filePath;

    IReadOnlyList<Result<IReadOnlyList<MediaFileMetadata>>> IQueryService<MediaFileMetadata>.Execute()
    {
        var result = GetMetadataWithArtworkImage(FilePath);
        if (result.IsSuccess)
            return [Result.Success<IReadOnlyList<MediaFileMetadata>>([result.Value])];
        else
            return [Result.Failure<IReadOnlyList<MediaFileMetadata>>(result.Error)];
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