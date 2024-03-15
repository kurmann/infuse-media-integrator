using Kurmann.InfuseMediaIntegrator.Entities;

namespace Kurmann.InfuseMediaIntegrator.Tests.Entities;

[TestClass]
public class VideoIntegrationDirectoryTests
{
    private const string InputPathTestCase1 = "Data/Testcase 1";
    private const string InputPathTestCase2 = "Data/Testcase 2";

    /// <summary>
    /// Testet die Methode <see cref="VideoIntegrationDirectory.Create(string)"/>.
    /// Ausganssituation ist ein Verzeichnis mit einer MPEG4-Datei und einer QuickTime-Datei.
    /// </summary>
    [TestMethod]
    public void Create_ShouldReturnSuccess_WhenDirectoryExists()
    {
        // Arrange
        var directoryPath = InputPathTestCase1;
        
        // Act
        var result = VideoIntegrationDirectory.Create(directoryPath);
        
        // Assert
        Assert.IsTrue(result.IsSuccess);
        Assert.AreEqual(1, result.Value.Mpeg4VideoFiles.Count); // Hier sollte nur die MPEG4-Datei enthalten sein
        Assert.AreEqual(1, result.Value.QuickTimeVideoFiles.Count); // Hier sollte nur die QuickTime-Datei enthalten sein
    }

    /// <summary>
    /// Testet, ob die Test-Datei mit TXT-Dateiendung als nicht unterstützte Datei erkannt wird.
    /// </summary>
    [TestMethod]
    public void Create_ShouldReturnSuccess_WhenDirectoryContainsNotSupportedFile()
    {
        // Arrange
        var directoryPath = InputPathTestCase1;
        
        // Act
        var result = VideoIntegrationDirectory.Create(directoryPath);
        
        // Assert
        Assert.IsTrue(result.IsSuccess);
        Assert.AreEqual(1, result.Value.NotSupportedFiles.Count); // Hier sollte nur die TXT-Datei enthalten sein
    }
    
    [TestMethod]
    public void Create_ShouldReturnFailure_WhenDirectoryNotFound()
    {
        // Arrange
        var directoryPath = "../not/existing";
        
        // Act
        var result = VideoIntegrationDirectory.Create(directoryPath);
        
        // Assert
        StringAssert.Contains(result.Error, "Directory not found: ");
        Assert.IsTrue(result.IsFailure);
    }

    [TestMethod] // Prüfe ob beim Verzeichnis "Testcase 2" je eine Mpeg4- und JPEG gefunden wird und keine QuickTime-Datei und keine nicht unterstützte Datei
    public void Create_ShouldReturnSuccess_WhenDirectoryContainsMpeg4VideoAndJpegFile()
    {
        // Arrange
        var directoryPath = InputPathTestCase2;
        
        // Act
        var result = VideoIntegrationDirectory.Create(directoryPath);
        
        // Assert
        Assert.IsTrue(result.IsSuccess);
        Assert.AreEqual(1, result.Value.Mpeg4VideoFiles.Count); // Hier sollte nur die MPEG4-Datei enthalten sein
        Assert.AreEqual(1, result.Value.JpegFiles.Count); // Hier sollte nur die JPEG-Datei enthalten sein
        Assert.AreEqual(0, result.Value.QuickTimeVideoFiles.Count); // Hier sollte keine QuickTime-Datei enthalten sein
        Assert.AreEqual(0, result.Value.NotSupportedFiles.Count); // Hier sollte keine nicht unterstützte Datei enthalten sein
    }
}
