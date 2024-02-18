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
        var outputDirectory = new DirectoryInfo(Path.Combine(InputDirectoryPath, "Data/Output"));

        // Act
        var createFanartInfuseImageCommand = new CreateFanartInfuseImageCommand(videoPath.FullName, outputDirectory.FullName);
        var result = createFanartInfuseImageCommand.Execute();

        // Assert
        Assert.IsTrue(result.IsSuccess);
    }
}