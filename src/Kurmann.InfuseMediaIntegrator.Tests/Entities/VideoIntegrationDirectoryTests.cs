using Kurmann.InfuseMediaIntegrator.Entities;

namespace Kurmann.InfuseMediaIntegrator.Tests.Entities;

[TestClass]
public class VideoIntegrationDirectoryTests
{
    private const string InputDirectoryPath = "../../Data/Input";

    /// <summary>
    /// Testet die Methode <see cref="VideoIntegrationDirectory.Create(string)"/>.
    /// Ausganssituation ist ein Verzeichnis mit einer MPEG4-Datei und einer QuickTime-Datei.
    /// </summary>
    [TestMethod]
    public void Create_ShouldReturnSuccess_WhenDirectoryExists()
    {
        // Arrange
        var directoryPath = InputDirectoryPath;
        
        // Act
        var result = VideoIntegrationDirectory.Create(directoryPath);
        
        // Assert
        Assert.IsTrue(result.IsSuccess);
        Assert.AreEqual(1, result.Value.Mpeg4VideoFiles.Count); // Hier sollte nur die MPEG4-Datei enthalten sein
    }
    
    [TestMethod]
    public void Create_ShouldReturnFailure_WhenDirectoryNotFound()
    {
        // Arrange
        var directoryPath = "../not/existing";
        
        // Act
        var result = VideoIntegrationDirectory.Create(directoryPath);
        
        // Assert
        Assert.AreEqual("Directory not found.", result.Error);
        Assert.IsTrue(result.IsFailure);
    }
}
