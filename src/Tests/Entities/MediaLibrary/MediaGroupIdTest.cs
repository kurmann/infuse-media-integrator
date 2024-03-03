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
}