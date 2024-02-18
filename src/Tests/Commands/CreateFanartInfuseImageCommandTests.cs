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
        var outputDirectory = "Data/Processing";

        // Act
        var createFanartInfuseImageCommand = new CreateFanartInfuseImageCommand(videoPath.FullName, outputDirectory);
        var result = createFanartInfuseImageCommand.Execute();

        // Assert
        Assert.IsTrue(result.IsSuccess);
        Assert.IsTrue(File.Exists(Path.Combine(outputDirectory, "Zwillinge Testvideo-fanart.jpg")));
        Assert.IsTrue(new FileInfo(Path.Combine(outputDirectory, "Zwillinge Testvideo-fanart.jpg")).Length > 0);

        // Clean up
        File.Delete(Path.Combine(outputDirectory, "Zwillinge Testvideo-fanart.jpg"));
    }
}