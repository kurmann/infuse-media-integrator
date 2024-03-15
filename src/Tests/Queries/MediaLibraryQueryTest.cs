using Kurmann.InfuseMediaIntegrator.Queries;

namespace Kurmann.InfuseMediaIntegrator.Tests.Queries;

[TestClass]
public class MediaLibraryQueryTests
{
    public const string MediaLibraryPath = "Data/Input/Testcase 6";

    [TestMethod]
    public void Execute_ShouldReturnFailure_WhenMediaLibraryPathIsEmpty()
    {
        // Arrange
        var query = new MediaLibraryQuery(string.Empty).ById(string.Empty);

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
        var query = new MediaLibraryQuery(MediaLibraryPath).ById("2024-02-16 Krokus Testaufnahme");

        // Act
        var result = query.Execute();

        // Assert
        Assert.IsTrue(result.IsSuccess);
        Assert.IsNotNull(result.Value);
        Assert.AreEqual(2, result.Value.Count);
        Assert.AreEqual("2024-02-16 Krokus Testaufnahme.m4v", result.Value[0].FilePath.FileName);
        Assert.AreEqual("2024-02-16 Krokus Testaufnahme-fanart.jpg", result.Value[1].FilePath.FileName);
    }

    [TestMethod] // Suche nach Aufnahme liefert korrektes Ergebnis
    public void WithRecording_ShouldReturnCorrectResult_WhenDateIsSet()
    {
        // Arrange
        var query = new MediaLibraryQuery(MediaLibraryPath).ByProperties().WithDate(new DateOnly(2024, 2, 16)).Query();

        // Act
        var result = query.Execute();

        // Assert
        Assert.IsTrue(result.IsSuccess);
        Assert.IsNotNull(result.Value);
        Assert.AreEqual(2, result.Value.Count);
        Assert.AreEqual("2024-02-16 Krokus Testaufnahme.m4v", result.Value[0].FilePath.FileName);
        Assert.AreEqual("2024-02-16 Krokus Testaufnahme-fanart.jpg", result.Value[1].FilePath.FileName);
    }

    [TestMethod] // Suche nach Titel liefert korrektes Ergebnis
    public void WithTitle_ShouldReturnCorrectResult_WhenTitleIsSet()
    {
        // Arrange
        var query = new MediaLibraryQuery(MediaLibraryPath).ByProperties().WithTitle("Krokus Testaufnahme").Query();

        // Act
        var result = query.Execute();

        // Assert
        Assert.IsTrue(result.IsSuccess);
        Assert.IsNotNull(result.Value);
        Assert.AreEqual(2, result.Value.Count);
        Assert.AreEqual("2024-02-16 Krokus Testaufnahme.m4v", result.Value[0].FilePath.FileName);
        Assert.AreEqual("2024-02-16 Krokus Testaufnahme-fanart.jpg", result.Value[1].FilePath.FileName);
    }

    [TestMethod] // Suche nach Titel und Aufnahme liefert korrektes Ergebnis
    public void WithTitleAndRecording_ShouldReturnCorrectResult_WhenTitleAndDateAreSet()
    {
        // Arrange
        var query = new MediaLibraryQuery(MediaLibraryPath).ByProperties().WithTitle("Krokus Testaufnahme").WithDate(new DateOnly(2024, 2, 16)).Query();

        // Act
        var result = query.Execute();

        // Assert
        Assert.IsTrue(result.IsSuccess);
        Assert.IsNotNull(result.Value);
        Assert.AreEqual(2, result.Value.Count);
        Assert.AreEqual("2024-02-16 Krokus Testaufnahme.m4v", result.Value[0].FilePath.FileName);
        Assert.AreEqual("2024-02-16 Krokus Testaufnahme-fanart.jpg", result.Value[1].FilePath.FileName);
    }

    [TestMethod] // Suche nach leerem Titel und leerer Aufnahme liefert leere Ergebnisliste
    public void WithEmptyTitleAndRecording_ShouldReturnEmptyResult_WhenTitleAndDateAreEmpty()
    {
        // Arrange
        var query = new MediaLibraryQuery(MediaLibraryPath).ByProperties().WithTitle(string.Empty).WithDate(DateOnly.MinValue).Query();

        // Act
        var result = query.Execute();

        // Assert
        Assert.IsTrue(result.IsSuccess);
        Assert.IsNotNull(result.Value);
        Assert.AreEqual(0, result.Value.Count);
    }

    [TestMethod] // Suche nach gesetztem Datum und leerem Titel liefert korrektes Ergebnis
    public void WithRecordingAndEmptyTitle_ShouldReturnCorrectResult_WhenDateIsSetAndTitleIsEmpty()
    {
        // Arrange
        var query = new MediaLibraryQuery(MediaLibraryPath).ByProperties().WithTitle(string.Empty).WithDate(new DateOnly(2024, 2, 16)).Query();

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