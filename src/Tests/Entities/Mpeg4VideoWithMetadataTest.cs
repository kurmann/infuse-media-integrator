using Kurmann.InfuseMediaIntegrator.Entities;

namespace Kurmann.InfuseMediaIntegrator.Tests.Entities;

[TestClass]
public class Mpeg4VideoWithMetadataTests
{
    private const string InputDirectoryPath = "Data/Input/Testcase 1";

    /// <summary>
    /// Testet die Methode <see cref="Mpeg4VideoWithMetadata.Create(Mpeg4Video)"/>.
    /// Ausganssituation ist eine MPEG4-Datei mit eingebetteten Metadaten.
    /// </summary>
    [TestMethod]
    public void Create_ShouldReturnSuccess_WhenMpeg4VideoWithMetadataExists()
    {
        // Arrange
        var videoPath = new FileInfo(Path.Combine(InputDirectoryPath, "Zwillinge Testvideo.m4v"));
        
        // Act
        var mpeg4VideoWithMetadata = Mpeg4VideoWithMetadata.Create(videoPath.FullName);
        
        // Assert
        Assert.IsTrue(mpeg4VideoWithMetadata.IsSuccess);
        Assert.AreEqual("Zwillinge Testvideo", mpeg4VideoWithMetadata.Value.Title);
        Assert.AreEqual("2024-02-11 Zwillinge Testvideo", mpeg4VideoWithMetadata.Value.TitleSort);
        Assert.AreEqual("Aufnahme der Zwillinge, die nach draussen wollen (use) als Testaufnahme für die Videoverarbeitung.", mpeg4VideoWithMetadata.Value.Description);
        Assert.AreEqual((uint)2024, mpeg4VideoWithMetadata.Value.Year);
        Assert.AreEqual("Familie Kurmann-Glück", mpeg4VideoWithMetadata.Value.Album);
    }
}