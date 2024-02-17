using Kurmann.InfuseMediaIntegrator.Entities;

namespace Kurmann.InfuseMediaIntegrator.Tests.Entities;

[TestClass]
public class Mpeg4VideoInputFilesTests
{
    private const string InputDirectoryPath = "../../../tests/Data/Input";

    [TestMethod]
    public void Create_ShouldReturnSuccess_WhenDirectoryExists()
    {
        // Arrange
        var directoryPath = Path.Combine(InputDirectoryPath, "Zwillinge Testvideo.m4v");
        
        // Act
        var result = Mpeg4VideoInputFiles.Create(directoryPath);
        
        // Assert
        Assert.IsTrue(result.IsSuccess);
    }
    
    [TestMethod]
    public void Create_ShouldReturnFailure_WhenDirectoryNotFound()
    {
        // Arrange
        var directoryPath = Path.Combine("../not/existing", "Zwillinge Testvideo.m4v");
        
        // Act
        var result = Mpeg4VideoInputFiles.Create(directoryPath);
        
        // Assert
        Assert.AreEqual("Directory not found.", result.Error);
        Assert.IsTrue(result.IsFailure);
    }
}
