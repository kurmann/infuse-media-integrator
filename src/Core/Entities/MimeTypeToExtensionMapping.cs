using CSharpFunctionalExtensions;

namespace Kurmann.InfuseMediaIntegrator.Entities;

public class MimeTypeToExtensionMapping
{
    public string MimeType { get; }
    public string Extension { get; }

    private MimeTypeToExtensionMapping(string mimeType, string extension)
    {
        MimeType = mimeType;
        Extension = extension;
    }

    public static Result<MimeTypeToExtensionMapping> Create(string? mimeType)
    {
        if (string.IsNullOrWhiteSpace(mimeType))
            return Result.Failure<MimeTypeToExtensionMapping>("MIME-Typ ist nicht definiert.");

        string extension = mimeType.Split('/')[1];

        if (extension == "jpeg")
            extension = "jpg";
        if (extension == "tiff")
            extension = "tif";
        if (extension == "x-ms-bmp")
            extension = "bmp";
        if (extension == "x-icon")
            extension = "ico";

        return new MimeTypeToExtensionMapping(mimeType, extension);
    }
}