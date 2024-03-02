using Kurmann.InfuseMediaIntegrator.Entities;
using Kurmann.InfuseMediaIntegrator.Entities.MediaFileTypes;

namespace Kurmann.InfuseMediaIntegrator.Tests.Entities;

[TestClass]
public class MediaFileLibraryOrganizationInfoTests
{
    private const string InputDirectoryPath = "Data/Input/Testcase 1";

    [TestMethod] // Teste ob das Unterverzeichnis aus dem Album-Tag gelesen wird und korrekt zurückgegeben wird
    public void Create_WithAlbumTag_ReturnsCorrectSubDirectory()
    {
        // Arrange
        var metadata = MediaFileMetadata.Create("Testvideo").Value.WithAlbum("Familie Kurmann");
        var path = Path.Combine(InputDirectoryPath, "Testvideo.m4v");
        var mediaFile = Mpeg4Video.Create(path, metadata).Value;

        // Act
        var result = MediaFileLibraryOrganizationInfo.Create(mediaFile, InputDirectoryPath);

        // Assert
        Assert.IsTrue(result.IsSuccess);
        Assert.AreEqual("Familie Kurmann", result.Value.TargetSubDirectory?.ToString());
        Assert.IsTrue(result.Value.HasAlbumNameInMetadata);
        Assert.AreEqual(SubdirectoryDerivationMode.Metadata, result.Value.SubdirectoryDerivationMode);
    }

    [TestMethod] // Teste ob Albumnamen mit Sonderzeichen eine Ableitung des Zielverzeichnisses aus dem Quellverzeichnis erzwingen
    public void Create_WithAlbumTagWithSpecialCharacters_ReturnsError()
    {
        // Arrange
        var metadata = MediaFileMetadata.Create("Testvideo").Value.WithAlbum("Familie Kurmann X ?");
        var path = Path.Combine(InputDirectoryPath, "Testvideo.m4v");
        var mediaFile = Mpeg4Video.Create(path, metadata).Value;

        // Act
        var result = MediaFileLibraryOrganizationInfo.Create(mediaFile, InputDirectoryPath);

        // Assert
        Assert.IsTrue(result.IsSuccess);
        Assert.IsTrue(result.Value.HasAlbumNameInMetadata); // Albumname ist vorhanden
        Assert.AreEqual(SubdirectoryDerivationMode.SourcePath, result.Value.SubdirectoryDerivationMode); // Albumname enthält Sonderzeichen, deshalb wird der SourcePath verwendet
        Assert.AreEqual("/", result.Value.TargetSubDirectory?.ToString());
    }

    [TestMethod] // Teste ob Album-Namen mit Schrägstrichen korrekt als Verzeichnis-Trennzeichen interpretiert werden
    public void Create_WithAlbumTagWithSlashes_ReturnsCorrectSubDirectory()
    {
        // Arrange
        var metadata = MediaFileMetadata.Create("Testvideo").Value.WithAlbum("Familie Kurmann/2021");
        var path = Path.Combine(InputDirectoryPath, "Testvideo.m4v");
        var mediaFile = Mpeg4Video.Create(path, metadata).Value;

        // Act
        var result = MediaFileLibraryOrganizationInfo.Create(mediaFile, InputDirectoryPath);

        // Assert
        Assert.IsTrue(result.IsSuccess);
        Assert.AreEqual("Familie Kurmann/2021", result.Value.TargetSubDirectory?.ToString());
        Assert.IsTrue(result.Value.HasAlbumNameInMetadata);
        Assert.AreEqual(SubdirectoryDerivationMode.Metadata, result.Value.SubdirectoryDerivationMode);
    }

    [TestMethod] // Teste ob leere Album-Tags eine Ableitung des Zielverzeichnisses aus dem Quellverzeichnis erzwingen
    public void Create_WithEmptyAlbumTag_ReturnsCorrectTagetPath()
    {
        // Arrange
        var metadata = MediaFileMetadata.Create("Testvideo").Value.WithAlbum(string.Empty);
        var path = Path.Combine(InputDirectoryPath, "Testvideo.m4v");
        var mediaFile = Mpeg4Video.Create(path, metadata).Value;

        // Act
        var result = MediaFileLibraryOrganizationInfo.Create(mediaFile, InputDirectoryPath);

        // Assert
        Assert.IsTrue(result.IsSuccess);
        Assert.IsTrue(result.Value.HasAlbumNameInMetadata); // Albumname ist vorhanden
        Assert.AreEqual(SubdirectoryDerivationMode.SourcePath, result.Value.SubdirectoryDerivationMode); // Albumname ist leer, deshalb wird der SourcePath verwendet
        Assert.AreEqual("/", result.Value.TargetSubDirectory?.ToString());
    }

    [TestMethod] // Teste ob bei fehlendem Album-Tag das Zielverzeichnis aus dem Quellverzeichnis abgeleitet wird
    public void Create_WithoutAlbumTag_ReturnsCorrectSubDirectory()
    {
        // Arrange
        var metadata = MediaFileMetadata.Create("Testvideo").Value;
        var path = Path.Combine(InputDirectoryPath, "New Project/Testvideo.m4v");
        var mediaFile = Mpeg4Video.Create(path, metadata).Value;

        // Act
        var result = MediaFileLibraryOrganizationInfo.Create(mediaFile, InputDirectoryPath);

        // Assert
        Assert.IsTrue(result.IsSuccess);
        Assert.IsFalse(result.Value.HasAlbumNameInMetadata);
        Assert.AreEqual(SubdirectoryDerivationMode.SourcePath, result.Value.SubdirectoryDerivationMode);
        Assert.AreEqual("/New Project", result.Value.TargetSubDirectory?.ToString());
    }
}