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

    [TestMethod] // Fehlermeldung wenn Datei kein Iso-8601-Datum im Namen hat
    public void Create_ShouldReturnFailure_WhenInvalidMediaFile()
    {
        // Arrange
        string mediaFilePath = "Local/Video-Export/Regenbogenfeuer.m4v";
        string mediaLibraryPath = "Volume/Kurmann/Infuse Mediathek";
        string subDirectoryPath = "Familie";

        // Act
        var result = MediaFileLibraryDestinationMapping.Create(mediaFilePath, mediaLibraryPath, subDirectoryPath);

        // Assert
        Assert.IsTrue(result.IsFailure);
        Assert.AreEqual("Error while creating media group ID from file name", result.Error);
    }

    [TestMethod] // Fehlermeldung wenn Medienbibliothek-Pfad ungültige Zeichen enthält
    public void Create_ShouldReturnFailure_WhenInvalidMediaLibraryPath()
    {
        // Arrange
        string mediaFilePath = "Local/Video-Export/2024-03-10 Regenbogenfeuer.m4v";
        string mediaLibraryPath = "Volume/Kurmann/Infuse Mediathe<>!!";
        string subDirectoryPath = "Familie";

        // Act
        var result = MediaFileLibraryDestinationMapping.Create(mediaFilePath, mediaLibraryPath, subDirectoryPath);

        // Assert
        Assert.IsTrue(result.IsFailure);
        StringAssert.Contains(result.Error, "contains invalid characters");
    }

    [TestMethod] // Nicht angegebenes Unterverzeichnis wird nicht berücksichtigt
    public void Create_ShouldReturnSuccess_WhenNoSubDirectory()
    {
        // Arrange
        string mediaFilePath = "Local/Video-Export/2024-03-10 Regenbogenfeuer.m4v";
        string mediaLibraryPath = "Volume/Kurmann/Infuse Mediathek";
        string? subDirectoryPath = null;
        string expectedTargetDirectory = "Volume/Kurmann/Infuse Mediathek/2024-03-10 Regenbogenfeuer";

        // Act
        var result = MediaFileLibraryDestinationMapping.Create(mediaFilePath, mediaLibraryPath, subDirectoryPath);

        // Assert
        Assert.IsTrue(result.IsSuccess);
        Assert.IsNotNull(result.Value);
        Assert.AreEqual(mediaFilePath, result.Value.Source.FilePath);
        Assert.AreEqual(expectedTargetDirectory, result.Value.TargetDirectory);
    }

    [TestMethod] // Unterverzeichnis mit mehreren Ebenen wird korrekt berücksichtigt
    public void Create_ShouldReturnSuccess_WhenSubDirectoryWithMultipleLevels()
    {
        // Arrange
        string mediaFilePath = "Local/Video-Export/2024-03-10 Regenbogenfeuer.m4v";
        string mediaLibraryPath = "Volume/Kurmann/Infuse Mediathek";
        string subDirectoryPath = "Familie/2024";
        string expectedTargetDirectory = "Volume/Kurmann/Infuse Mediathek/Familie/2024/2024-03-10 Regenbogenfeuer";

        // Act
        var result = MediaFileLibraryDestinationMapping.Create(mediaFilePath, mediaLibraryPath, subDirectoryPath);

        // Assert
        Assert.IsTrue(result.IsSuccess);
        Assert.IsNotNull(result.Value);
        Assert.AreEqual(mediaFilePath, result.Value.Source.FilePath);
        Assert.AreEqual(expectedTargetDirectory, result.Value.TargetDirectory);
    }
}