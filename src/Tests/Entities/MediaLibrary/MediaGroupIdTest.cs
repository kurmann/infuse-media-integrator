using Kurmann.InfuseMediaIntegrator.Entities.Elementary;
using Kurmann.InfuseMediaIntegrator.Entities.MediaLibrary;

namespace Kurmann.InfuseMediaIntegrator.Tests.Entities.MediaLibrary;

[TestClass]
public class MediaGroupIdTests
{
    [TestMethod]
    public void Create_ShouldReturnSuccess_WhenIdIsValid()
    {
        // Arrange
        string id = "2022-01-01 Title";

        // Act
        var result = MediaGroupId.Create(id);

        // Assert
        Assert.IsTrue(result.IsSuccess);
        Assert.IsNotNull(result.Value);
        Assert.AreEqual(id, result.Value.Id);
    }

    [TestMethod] // Fehlermeldung wenn Datum nicht zu Beginn steht
    public void Create_ShouldReturnFailure_WhenIsoDateIsAtTheEnd()
    {
        // Arrange
        string id = "Title 2022-01-01";

        // Act
        var result = MediaGroupId.Create(id);

        // Assert
        Assert.IsFalse(result.IsSuccess);
        Assert.IsNotNull(result.Error);
        // Add more assertions as needed
    }

    [TestMethod]
    public void Create_ShouldReturnFailure_WhenIdIsEmpty()
    {
        // Arrange
        string id = "";

        // Act
        var result = MediaGroupId.Create(id);

        // Assert
        Assert.IsFalse(result.IsSuccess);
        Assert.IsNotNull(result.Error);
        Assert.AreEqual("ID is empty.", result.Error);
    }

    [TestMethod]
    public void Create_ShouldReturnFailure_WhenIdIsWhitespace()
    {
        // Arrange
        string id = "   ";

        // Act
        var result = MediaGroupId.Create(id);

        // Assert
        Assert.IsFalse(result.IsSuccess);
        Assert.IsNotNull(result.Error);
        Assert.AreEqual("ID is empty.", result.Error);
    }

    [TestMethod]
    public void Create_ShouldReturnFailure_WhenIdIsInvalidFormat()
    {
        // Arrange
        string id = "2022/01/01 Title";

        // Act
        var result = MediaGroupId.Create(id);

        // Assert
        Assert.IsFalse(result.IsSuccess);
        Assert.IsNotNull(result.Error);
        // Add more assertions as needed
    }

    [TestMethod] // Teste ob die ID von einem Dateinamen abgeleitet werden kann
    public void CreateFromFileName_ShouldReturnSuccess_WhenFileNameIsValid()
    {
        // Arrange
        var fileNameInfo = FileNameInfo.Create("2022-01-01 Title.jpg").Value;

        // Act
        var result = MediaGroupId.CreateFromFileName(fileNameInfo);

        // Assert
        Assert.IsTrue(result.IsSuccess);
        Assert.IsNotNull(result.Value);
        Assert.AreEqual("2022-01-01 Title", result.Value.Id);
    }

    [TestMethod] // Teste ob typische Trennzeichen ignoriert werden
    public void CreateFromFileName_ShouldReturnSuccess_WhenFileNameContainsIgnoredSeparators()
    {
        // Arrange
        var fileNameInfo = FileNameInfo.Create("2022-01-01 Title-fanart.jpg").Value;

        // Act
        var result = MediaGroupId.CreateFromFileName(fileNameInfo);

        // Assert
        Assert.IsTrue(result.IsSuccess);
        Assert.IsNotNull(result.Value);
        Assert.AreEqual("2022-01-01 Title", result.Value.Id);
    }
}