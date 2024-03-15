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
        FileMovedToMediaLibraryEventArgs? commandEventArgs = null; // Hilfsvariable für das Erfassen der Event-Informationen
        command.FileMovedToMediaLibrary += (sender, e) => commandEventArgs = e;
        var expectedNewMediaGroupPath = new FileInfo(Path.Combine(command.MediaLibraryPath, "2023-03-14 Primeli Kurzaufnahme/2023-03-14 Primeli Kurzaufnahme.m4v"));
        File.Delete(expectedNewMediaGroupPath.FullName); // Sicherstellen, dass die Datei gelöscht ist

        // Act
        var result = command.Execute();

        // Assert
        Assert.IsTrue(result.IsSuccess);
        Assert.IsNotNull(commandEventArgs);
        Assert.AreEqual(expectedNewMediaGroupPath.FullName, commandEventArgs.FileInfo.FullName); // Die Datei ist im erwarteten Verzeichnis vorhanden
        Assert.IsTrue(commandEventArgs.FileInfo.Exists);
        Assert.IsFalse(commandEventArgs.HasMovedToExistingMediaGroup); // Die Datei wurde nicht in eine bestehende Mediengruppe verschoben
        Assert.IsFalse(commandEventArgs.HasTargetFileBeenOverwritten); // Die Datei wurde nicht überschrieben
        Assert.IsTrue(File.Exists(expectedNewMediaGroupPath.FullName));
        
        // Clean up
        File.Delete(expectedNewMediaGroupPath.FullName);
    }
}