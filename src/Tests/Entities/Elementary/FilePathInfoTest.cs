using Kurmann.InfuseMediaIntegrator.Entities.Elementary;

namespace Kurmann.InfuseMediaIntegrator.Tests.Entities.Elementary;

[TestClass]
public class FilePathInfoTests
{
    [TestMethod]
    public void Create_ShouldReturnFailure_WhenFilePathIsNull()
    {
        // Arrange
        string? filePath = null;

        // Act
        var result = FilePathInfo.Create(filePath);

        // Assert
        Assert.IsFalse(result.IsSuccess);
        Assert.IsNotNull(result.Error);
        // Add more assertions as needed
    }

    [TestMethod]
    public void Create_ShouldReturnFailure_WhenFilePathIsEmpty()
    {
        // Arrange
        string filePath = "";

        // Act
        var result = FilePathInfo.Create(filePath);

        // Assert
        Assert.IsFalse(result.IsSuccess);
        Assert.IsNotNull(result.Error);
        // Add more assertions as needed
    }

    [TestMethod]
    public void Create_ShouldReturnFailure_WhenFilePathIsWhitespace()
    {
        // Arrange
        string filePath = "   ";

        // Act
        var result = FilePathInfo.Create(filePath);

        // Assert
        Assert.IsFalse(result.IsSuccess);
        Assert.IsNotNull(result.Error);
        // Add more assertions as needed
    }

    [TestMethod] // Nicht erfolgreich wenn Pfad unzulässige Zeichen enthält (auf dem Mac)
    public void Create_ShouldReturnFailure_WhenFilePathContainsInvalidCharacters()
    {
        // Arrange
        string filePath = "C:/path/with?invalid>characters";

        // Act
        var result = FilePathInfo.Create(filePath);

        // Assert
        Assert.IsFalse(result.IsSuccess);
        Assert.IsNotNull(result.Error);
        // Add more assertions as needed
    }

    [TestMethod]
    public void Create_ShouldReturnFailure_WhenFilePathIsDirectory()
    {
        // Arrange
        string filePath = "C:/path/to/directory/";

        // Act
        var result = FilePathInfo.Create(filePath);

        // Assert
        Assert.IsFalse(result.IsSuccess);
        Assert.IsNotNull(result.Error);
        // Add more assertions as needed
    }
}