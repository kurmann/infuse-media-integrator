using Kurmann.InfuseMediaIntegrator.Commands;
using Microsoft.Extensions.Logging.Abstractions;

namespace Kurmann.InfuseMediaIntegrator.Tests.Commands;

[TestClass]
public class MoveFilesToMediaLibraryCommandTest
{
    [TestMethod] // Wird korrekt in eine neue Mediengruppe verschoben wenn die Ausgangsdatei keiner bestehenden Mediengruppe zugeordnet ist
    public void Execute_ShouldMoveFileToNewMediaGroup_WhenFileIsNotAssignedToExistingMediaGroup()
    {
        // Arrange
        var file = "Data/Input/Testcase 1/Zwillinge Testvideo.m4v";
        var mediaLibraryPath = "Data/Output/Mediathek";
        var logger = new NullLogger<MoveFileToMediaLibraryCommand>();
        var command = new MoveFileToMediaLibraryCommand(file, logger).ToMediaLibrary(mediaLibraryPath);

        // Act
        var result = command.Execute();

        // Assert
        Assert.IsTrue(result.IsSuccess);
        Assert.IsTrue(File.Exists(Path.Combine(mediaLibraryPath, "TestFile.mp4")));
    }
}