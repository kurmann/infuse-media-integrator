using Kurmann.InfuseMediaIntegrator.Entities;

namespace Kurmann.InfuseMediaIntegrator.Tests.Entities;

[TestClass]
public class CategoryPathTests
{
    [TestMethod]
    public void Create_ShouldReturnSuccess_WhenValidPathProvided()
    {
        // Arrange
        string validPath = "Category1/Category2/Category3";

        // Act
        var result = CategoryPath.Create(validPath);

        // Assert
        Assert.IsTrue(result.IsSuccess);
        Assert.AreEqual(3, result.Value.Categories.Count);
        Assert.AreEqual("Category1", result.Value.Categories[0]);
        Assert.AreEqual("Category2", result.Value.Categories[1]);
        Assert.AreEqual("Category3", result.Value.Categories[2]);
        Assert.AreEqual(result.Value.Value, validPath);
    }

    [TestMethod]
    public void Create_ShouldReturnFailure_WhenPathIsNull()
    {
        // Arrange
        string nullPath = null;

        // Act
        var result = CategoryPath.Create(nullPath);

        // Assert
        Assert.IsFalse(result.IsSuccess);
        Assert.AreEqual("Path is null or empty", result.Error);
    }

    [TestMethod]
    public void Create_ShouldReturnFailure_WhenPathIsEmpty()
    {
        // Arrange
        string emptyPath = "";

        // Act
        var result = CategoryPath.Create(emptyPath);

        // Assert
        Assert.IsFalse(result.IsSuccess);
        Assert.AreEqual("Path is null or empty", result.Error);
    }

    [TestMethod]
    public void Create_ShouldReturnFailure_WhenPathContainsInvalidCharacters()
    {
        // Arrange
        string pathWithInvalidChars = "Category1/Category2/Category*3";

        // Act
        var result = CategoryPath.Create(pathWithInvalidChars);

        // Assert
        Assert.IsFalse(result.IsSuccess);
        Assert.AreEqual("Path contains invalid characters: /, \\, ?, %, *, :, |, \", <, >", result.Error);
    }
}