using Kurmann.InfuseMediaIntegrator.Commands;
using Microsoft.Extensions.Logging.Abstractions;

namespace Kurmann.InfuseMediaIntegrator.Tests.Commands;

[TestClass]
public class MoveFilesToMediaLibraryCommandTest
{
    private const string MediaLibraryPath = "Data/Output/Mediathek";
    private const string InputDirectoryPath = "Data/Input/Testcase 3";

    [TestMethod] // Wird korrekt in eine neue Mediengruppe verschoben wenn die Ausgangsdatei keiner bestehenden Mediengruppe zugeordnet ist
    public void Execute_ShouldMoveFileToNewMediaGroup_WhenFileIsNotAssignedToExistingMediaGroup()
    {
        // Arrange
        var file = Path.Combine(InputDirectoryPath, "Zwillinge Testvideo.m4v");
        var mediaLibraryPath = "Data/Output/Mediathek";
        var logger = new NullLogger<MoveFilesToMediaLibraryCommand>();
        var command = new MoveFilesToMediaLibraryCommand(logger).File(file).To(mediaLibraryPath);

        // Act
        var result = command.Execute();

        // Assert
        Assert.IsTrue(result.IsSuccess);
        Assert.IsTrue(File.Exists(Path.Combine(mediaLibraryPath, "TestFile.mp4")));
    }
}