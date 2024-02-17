using Kurmann.InfuseMediaIntegrator.Entities;

namespace Kurmann.InfuseMediaIntegrator.Tests.Entities;

[TestClass]
public class Mpeg4VideoInputFilesTests
{

    [TestMethod]
    public void Create_ShouldReturnSuccess_WhenDirectoryExists()
    {
        // Arrange
        var directoryPath = InputDirectoryPath;
        
        // Act
        var result = Mpeg4VideoInputFiles.Create(directoryPath);
        
        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
    }
    
    [TestMethod]
    public void Create_ShouldReturnFailure_WhenDirectoryNotFound()
    {
        // Arrange
        var directoryPath = InputDirectoryPath;
        
        // Act
        var result = Mpeg4VideoInputFiles.Create(directoryPath);
        
        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal("Directory not found.", result.Error);
    }
}
