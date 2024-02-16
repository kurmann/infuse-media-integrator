using Xunit;

namespace Kurmann.InfuseMediaIntegrator.Tests.Entities;

public class Mpeg4VideoInputFilesTests
{
    private const string InputDirectoryPath = "../tests/input/family-kurmann-glueck-category-with-embedded-artwork";

    [Fact]
    public void Create_ShouldReturnSuccess_WhenDirectoryExists()
    {
        // Arrange
        var directoryPath = InputDirectoryPath;
        
        // Act
        var result = Mpeg4VideoInputFiles.Create(directoryPath);
        
        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        // Add more assertions as needed
    }
    
    [Fact]
    public void Create_ShouldReturnFailure_WhenDirectoryNotFound()
    {
        // Arrange
        var directoryPath = InputDirectoryPath;
        
        // Act
        var result = Mpeg4VideoInputFiles.Create(directoryPath);
        
        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal("Directory not found.", result.Error);
        // Add more assertions as needed
    }
    
}