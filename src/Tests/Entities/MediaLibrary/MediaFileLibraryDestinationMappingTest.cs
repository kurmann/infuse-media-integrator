using Kurmann.InfuseMediaIntegrator.Entities.MediaLibrary;

namespace Kurmann.InfuseMediaIntegrator.Tests.Entities.MediaLibrary;

[TestClass]
public class MediaFileLibraryDestinationMappingTests
{
    [TestMethod]
    public void Create_ShouldReturnSuccess_WhenValidMediaFileAndMediaLibraryPath()
    {
        // Arrange
        string mediaFilePath = "Local/Video-Export/2024-03-10 Regenbogenfeuer.m4v";
        string mediaLibraryPath = "Volume/Kurmann/Infuse Mediathek";
        string subDirectoryPath = "Familie";
        string expectedTargetDirectory = "Volume/Kurmann/Infuse Mediathek/Familie/2024-03-10 Regenbogenfeuer";

        // Act
        var result = MediaFileLibraryDestinationMapping.Create(mediaFilePath, mediaLibraryPath, subDirectoryPath);

        // Assert
        Assert.IsTrue(result.IsSuccess);
        Assert.IsNotNull(result.Value);
        Assert.AreEqual(mediaFilePath, result.Value.Source.FilePath);
        Assert.AreEqual(expectedTargetDirectory, result.Value.TargetDirectory);
    }
}