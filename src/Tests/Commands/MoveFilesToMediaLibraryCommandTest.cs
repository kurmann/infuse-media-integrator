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
        var command = new MoveFileToMediaLibraryCommand
        {
            FilePath = "Data/Input/Testcase 4/2023-03-14 Primeli Kurzaufnahme.m4v",
            MediaLibraryPath = "Data/Output/Mediathek",
            Logger = new NullLogger<MoveFileToMediaLibraryCommand>()
        };
        FileInfo? capturedEventFileInfo = null; // Hilfsvariable für das Erfassen der Event-Informationen
        command.FileMovedToNewMediaGroup += (sender, e) => capturedEventFileInfo = e;
        var expectedNewMediaGroupPath = new FileInfo(Path.Combine(command.MediaLibraryPath, "2023-03-14 Primeli Kurzaufnahme/2023-03-14 Primeli Kurzaufnahme.m4v"));
        File.Delete(expectedNewMediaGroupPath.FullName); // Sicherstellen, dass die Datei gelöscht ist

        // Act
        var result = command.Execute();

        // Assert
        Assert.IsTrue(result.IsSuccess);
        Assert.IsNotNull(capturedEventFileInfo);
        Assert.AreEqual(expectedNewMediaGroupPath.FullName, capturedEventFileInfo.FullName);
        Assert.IsTrue(File.Exists(expectedNewMediaGroupPath.FullName));
        
        // Clean up
        File.Delete(expectedNewMediaGroupPath.FullName);
    }
}