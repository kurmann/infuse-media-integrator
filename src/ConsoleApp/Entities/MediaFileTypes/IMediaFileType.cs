using Kurmann.InfuseMediaIntegrator.Entities.Elementary;

namespace Kurmann.InfuseMediaIntegrator.Entities.MediaFileTypes;

public interface IMediaFileType
{
    FilePathInfo FilePath { get; }
}