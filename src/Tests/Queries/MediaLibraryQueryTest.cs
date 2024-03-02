using Kurmann.InfuseMediaIntegrator.Queries;

namespace Kurmann.InfuseMediaIntegrator.Tests.Queries;

[TestClass]
public class MediaLibraryQueryTests
{
    public const string MediaLibraryPath = "Data/Output/Mediathek";

    [TestMethod]
    public void Execute_ShouldReturnFailure_WhenMediaLibraryPathIsEmpty()
    {
        // Arrange
        var query = new MediaLibraryQuery(string.Empty);

        // Act
        var result = query.Execute();

        // Assert
        Assert.IsFalse(result.IsSuccess);
        Assert.IsNotNull(result.Error);
        Assert.AreEqual("Media library path is empty.", result.Error);
    }

    [TestMethod] // Suche nach ID liefert korrektes Ergebnis
    public void WithId_ShouldReturnCorrectResult_WhenIdIsSet()
    {
        // Arrange
        var query = new MediaLibraryQuery(MediaLibraryPath).WithId("2024-02-16 Krokus Testaufnahme");

        // Act
        var result = query.Execute();

        // Assert
        Assert.IsTrue(result.IsSuccess);
        Assert.IsNotNull(result.Value);
        Assert.AreEqual(2, result.Value.Count);
        Assert.AreEqual("2024-02-16 Krokus Testaufnahme.m4v", result.Value[0].FilePath.FileName);
        Assert.AreEqual("2024-02-16 Krokus Testaufnahme-fanart.jpg", result.Value[1].FilePath.FileName);
    }
}