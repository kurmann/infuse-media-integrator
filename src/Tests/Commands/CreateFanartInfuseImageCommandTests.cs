using Microsoft.VisualStudio.TestPlatform.CrossPlatEngine;

namespace Kurmann.InfuseMediaIntegrator.Commands;

[TestClass]
public class CreateFanartInfuseImageCommandTests
{
    private const string InputDirectoryPath = "Data/Input";

    /// <summary>
    /// Testet die Methode <see cref="CreateFanartInfuseImageCommand.Execute()"/>.
    /// Ausganssituation ist eine MPEG4-Datei mit eingebetteten Metadaten.
    /// </summary>
    [TestMethod]
    public void Execute_ShouldReturnSuccess_WhenMpeg4VideoWithMetadataExists()
    {
        // Arrange
        var videoPath = new FileInfo(Path.Combine(InputDirectoryPath, "Zwillinge Testvideo.m4v"));

        // Act
        var createFanartInfuseImageCommand = new CreateFanartInfuseImageCommand(videoPath.FullName);
        var result = createFanartInfuseImageCommand.Execute();

        // Assert
        Assert.IsTrue(result.IsSuccess);
        Assert.IsTrue(File.Exists(Path.Combine(InputDirectoryPath, "Zwillinge Testvideo-fanart.jpg"))); // Das Fanart-Infuse-Image sollte erstellt worden sein
        Assert.IsTrue(new FileInfo(Path.Combine(InputDirectoryPath, "Zwillinge Testvideo-fanart.jpg")).Length > 0); // Das Fanart-Infuse-Image sollte nicht leer sein

        // Clean up
        File.Delete(Path.Combine(InputDirectoryPath, "Zwillinge Testvideo-fanart.jpg")); // Lösche das Fanart-Infuse-Image damit der Test wiederholt werden kann
    }

    [TestMethod] // Teste ob leerer Pfad zu einem Fehler führt
    public void Execute_ShouldReturnFailure_WhenMpeg4VideoPathIsEmpty()
    {
        // Arrange
        var videoPath = string.Empty;

        // Act
        var createFanartInfuseImageCommand = new CreateFanartInfuseImageCommand(videoPath);
        var result = createFanartInfuseImageCommand.Execute();

        // Assert
        StringAssert.Contains(result.Error, "The MP4 video does not exist.");
        Assert.IsTrue(result.IsFailure);
    }
}