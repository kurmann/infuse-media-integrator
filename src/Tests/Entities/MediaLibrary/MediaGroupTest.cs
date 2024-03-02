using Kurmann.InfuseMediaIntegrator.Entities.MediaFileTypes;
using Kurmann.InfuseMediaIntegrator.Entities.MediaLibrary;

namespace Kurmann.InfuseMediaIntegrator.Tests.Entities.MediaLibrary;

[TestClass]
public class MediaGroupTest
{
    [TestMethod] // Teste ob ein MediaGroup-Objekt korrekt erstellt wird
    public void Create_ShouldReturnSuccess_WhenPathIsValidDirectory()
    {
        // Arrange
        string path = "/workspaces/infuse-media-integrator/src/ConsoleApp/Entities";
        var mediaFiles = new List<IMediaFileType>
        {
            Mpeg4Video.Create("Testvideo.m4v").Value,
            QuickTimeVideo.Create("Testvideo.mov").Value
        };

        // Act
        var result = MediaGroup.Create(mediaFiles, path);

        // Assert
        Assert.IsTrue(result.IsSuccess);
        Assert.IsNotNull(result.Value);
        Assert.AreEqual(path, result.Value.DirectoryPath);
        Assert.AreEqual(2, result.Value.MediaFiles.Count);
    }
}