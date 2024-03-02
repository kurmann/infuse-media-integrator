using Kurmann.InfuseMediaIntegrator.Entities.Elementary;
using Kurmann.InfuseMediaIntegrator.Entities.MediaLibrary;

namespace Kurmann.InfuseMediaIntegrator.Entities.MediaFileTypes;

public interface IMediaFileType
{
    FilePathInfo FilePath { get; }

    MediaFileMetadata? Metadata { get; }
}