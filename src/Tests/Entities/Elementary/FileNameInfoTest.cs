using Kurmann.InfuseMediaIntegrator.Entities.Elementary;

namespace Kurmann.InfuseMediaIntegrator.Tests.Entities.Elementary;

[TestClass]
public class FileNameInfoTests
{
    [TestMethod]
    public void Create_ShouldReturnSuccess_WhenFileNameIsValid()
    {
        // Arrange
        string fileName = "example_file.txt";

        // Act
        var result = FileNameInfo.Create(fileName);

        // Assert
        Assert.IsTrue(result.IsSuccess);
        Assert.IsNotNull(result.Value);
        Assert.AreEqual(fileName, result.Value.FileName);
    }

    [TestMethod]
    public void Create_ShouldReturnFailure_WhenFileNameIsNull()
    {
        // Arrange
        string? fileName = null;

        // Act
        var result = FileNameInfo.Create(fileName);

        // Assert
        Assert.IsFalse(result.IsSuccess);
        Assert.IsNotNull(result.Error);
        Assert.AreEqual("File name is null or empty", result.Error);
    }

    [TestMethod]
    public void Create_ShouldReturnFailure_WhenFileNameIsEmpty()
    {
        // Arrange
        string fileName = "";

        // Act
        var result = FileNameInfo.Create(fileName);

        // Assert
        Assert.IsFalse(result.IsSuccess);
        Assert.IsNotNull(result.Error);
        Assert.AreEqual("File name is null or empty", result.Error);
    }

    [TestMethod]
    public void Create_ShouldReturnFailure_WhenFileNameIsWhitespace()
    {
        // Arrange
        string fileName = "   ";

        // Act
        var result = FileNameInfo.Create(fileName);

        // Assert
        Assert.IsFalse(result.IsSuccess);
        Assert.IsNotNull(result.Error);
        Assert.AreEqual("File name is null or empty", result.Error);
    }

    [TestMethod]
    public void Create_ShouldReturnFailure_WhenFileNameIsDirectory()
    {
        // Arrange
        string fileName = "path/to/directory/";

        // Act
        var result = FileNameInfo.Create(fileName);

        // Assert
        Assert.IsFalse(result.IsSuccess);
        Assert.IsNotNull(result.Error);
        Assert.AreEqual("File name is not a file", result.Error);
    }

    [TestMethod]
    public void Create_ShouldReturnFailure_WhenFileNameContainsInvalidChars()
    {
        // Arrange
        string fileName = "example_file?.txt";

        // Act
        var result = FileNameInfo.Create(fileName);

        // Assert
        Assert.IsFalse(result.IsSuccess);
        Assert.IsNotNull(result.Error);
        StringAssert.StartsWith(result.Error, "File name contains invalid characters");
    }
}