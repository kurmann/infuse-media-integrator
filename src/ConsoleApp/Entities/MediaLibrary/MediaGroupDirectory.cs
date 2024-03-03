using CSharpFunctionalExtensions;
using Kurmann.InfuseMediaIntegrator.Entities.Elementary;

namespace Kurmann.InfuseMediaIntegrator.Entities.MediaLibrary;

/// <summary>
/// Stellt ein Verzeichnis dar, das eine Mediengruppe darstellt.
/// </summary>
public class MediaGroupDirectory
{
    public DirectoryPathInfo DirectoryPath { get; }
    public MediaGroupId MediaGroupId { get; }

    private MediaGroupDirectory(DirectoryPathInfo directoryPath, MediaGroupId mediaGroupId)
    {
        DirectoryPath = directoryPath;
        MediaGroupId = mediaGroupId;
    }

    public static Result<MediaGroupDirectory> Create(DirectoryPathInfo directoryPath, MediaGroupId mediaGroupId)
    {
        if (directoryPath == null)
        {
            return Result.Failure<MediaGroupDirectory>("Directory path is null");
        }

        if (mediaGroupId == null)
        {
            return Result.Failure<MediaGroupDirectory>("Media group ID is null");
        }

        return Result.Success(new MediaGroupDirectory(directoryPath, mediaGroupId));
    }
}
