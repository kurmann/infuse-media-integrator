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
        // Add more assertions as needed
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