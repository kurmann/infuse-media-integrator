using Kurmann.InfuseMediaIntegrator.Commands;

namespace Kurmann.InfuseMediaIntegrator.Tests.Commands;

[TestClass]
public class CreateFanartInfuseImageCommandTests
{
    private const string PathTestcase1 = "Data/Testcase 1";
    private const string PathTestcase2 = "Data/Testcase 2";

    /// <summary>
    /// Testet die Methode <see cref="CreateFanartInfuseImageCommand.Execute()"/>.
    /// Ausganssituation ist eine MPEG4-Datei mit eingebetteten Metadaten.
    /// </summary>
    [TestMethod]
    public void Execute_ShouldReturnSuccess_WhenMpeg4VideoWithMetadataExists()
    {
        // Arrange
        var videoPath = new FileInfo(Path.Combine(PathTestcase1, "Zwillinge Testvideo.m4v"));

        // Act
        var createFanartInfuseImageCommand = new CreateFanartInfuseImageCommand(videoPath.FullName);
        var result = createFanartInfuseImageCommand.Execute();

        // Assert
        Assert.IsTrue(result.IsSuccess);
        Assert.IsTrue(result.Value != null);
        Assert.IsTrue(File.Exists(Path.Combine(PathTestcase1, "Zwillinge Testvideo-fanart.jpg"))); // Das Fanart-Infuse-Image sollte erstellt worden sein
        Assert.IsTrue(new FileInfo(Path.Combine(PathTestcase1, "Zwillinge Testvideo-fanart.jpg")).Length > 0); // Das Fanart-Infuse-Image sollte nicht leer sein

        // Clean up
        File.Delete(Path.Combine(PathTestcase1, "Zwillinge Testvideo-fanart.jpg")); // Lösche das Fanart-Infuse-Image damit der Test wiederholt werden kann
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
        Assert.IsTrue(result.IsFailure);
        StringAssert.Contains(result.Error, "The MP4 video path is empty.");
    }

    [TestMethod] // Test ob bei der Datei "Zwillinge Testvideo (ohne Artwork).m4v" ein Fehler zurückgegeben wird, weil kein Artwork vorhanden ist
    public void Execute_ShouldReturnFailure_WhenMpeg4VideoDoesNotContainArtwork()
    {
        // Arrange
        var videoPath = new FileInfo(Path.Combine(PathTestcase2, "Zwillinge Testvideo (ohne Artwork).m4v"));

        // Act
        var createFanartInfuseImageCommand = new CreateFanartInfuseImageCommand(videoPath.FullName);
        var result = createFanartInfuseImageCommand.Execute();

        // Assert
        Assert.IsTrue(result.IsFailure);
        StringAssert.Contains(result.Error, "The MP4 video does not contain a title image (artwork).");
    }
}