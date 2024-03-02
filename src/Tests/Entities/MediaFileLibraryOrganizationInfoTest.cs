using Kurmann.InfuseMediaIntegrator.Entities;
using Kurmann.InfuseMediaIntegrator.Entities.MediaFileTypes;

namespace Kurmann.InfuseMediaIntegrator.Tests.Entities;

[TestClass]
public class MediaFileLibraryOrganizationInfoTests
{
    private const string InputDirectoryPath = "Data/Input/Testcase 1";

    [TestMethod] // Teste ob das Unterverzeichnis aus dem Album-Tag gelesen wird und korrekt zurückgegeben wird
    public void Create_ShouldReturnSuccess_WhenAlbumNameIsSet()
    {
        // Arrange
        var metadata = MediaFileMetadata.Create("Zwillinge Testvideo").Value.WithAlbum("Familie Kurmann-Glück");
        var path = Path.Combine(InputDirectoryPath, "Zwillinge Testvideo.m4v");
        var mediaFile = Mpeg4Video.Create(path, metadata).Value;

        // Act
        var result = MediaFileLibraryOrganizationInfo.Create(mediaFile, InputDirectoryPath);

        // Assert
        Assert.IsTrue(result.IsSuccess);
        Assert.AreEqual("Familie Kurmann-Glück", result.Value.TargetSubDirectory?.ToString());
    }
}