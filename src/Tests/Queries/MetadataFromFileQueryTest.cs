using Kurmann.InfuseMediaIntegrator.Commands;

namespace Kurmann.InfuseMediaIntegrator.Tests.Queries;

[TestClass]
public class MetadataFromFileQueryTests
{
    private const string InputPathTestCase1 = "Data/Input/Testcase 1";

    [TestMethod]
    public void Execute_ShouldReturnMetadata_WhenFilePathIsValid()
    {
        // Arrange
        string filePath = Path.Combine(InputPathTestCase1, "Zwillinge Testvideo.m4v");
        var query = new MetadataFromFileQuery(filePath);

        // Act
        var result = query.Execute();

        // Assert
        Assert.IsTrue(result.IsSuccess);
        Assert.IsNotNull(result.Value);
        Assert.AreEqual("Zwillinge Testvideo", result.Value.Title);
        Assert.AreEqual("2024-02-11 Zwillinge Testvideo", result.Value.TitleSort);
        Assert.AreEqual("Aufnahme der Zwillinge, die nach draussen wollen (use) als Testaufnahme für die Videoverarbeitung.", result.Value.Description);
        Assert.AreEqual("Familie Kurmann-Glück", result.Value.Album);
        Assert.AreEqual((uint)2024, result.Value.Year);
        Assert.IsNotNull(result.Value.Artwork);
        Assert.AreEqual("image/jpeg", result.Value.ArtworkMimeType);
        Assert.AreEqual("jpg", result.Value.ArtworkExtension);
    }

    [TestMethod]
    public void Execute_ShouldReturnFailure_WhenFilePathIsNull()
    {
        // Arrange
        string? filePath = null;
        var query = new MetadataFromFileQuery(filePath);

        // Act
        var result = query.Execute();

        // Assert
        Assert.IsFalse(result.IsSuccess);
        Assert.IsNotNull(result.Error);
        // Add more assertions as needed
    }

    [TestMethod]
    public void Execute_ShouldReturnFailure_WhenFilePathIsEmpty()
    {
        // Arrange
        string filePath = string.Empty;
        var query = new MetadataFromFileQuery(filePath);

        // Act
        var result = query.Execute();

        // Assert
        Assert.IsFalse(result.IsSuccess);
        Assert.IsNotNull(result.Error);
        // Add more assertions as needed
    }
}