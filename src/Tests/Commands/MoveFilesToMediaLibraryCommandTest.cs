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
            FilePath = "Data/Testcase 4/2023-03-14 Primeli Kurzaufnahme.m4v",
            MediaLibraryPath = "Data/Output/Mediathek",
            Logger = new NullLogger<MoveFileToMediaLibraryCommand>()
        };
        FileMovedToMediaLibraryResultArgs? commandEventArgs = null; // Hilfsvariable für das Erfassen der Event-Informationen
        command.FileMovedToMediaLibrary += (sender, e) => commandEventArgs = e;
        var expectedNewMediaGroupPath = new FileInfo(Path.Combine(command.MediaLibraryPath, "2023-03-14 Primeli Kurzaufnahme/2023-03-14 Primeli Kurzaufnahme.m4v"));

        if (File.Exists(expectedNewMediaGroupPath.FullName))
        {
            File.Delete(expectedNewMediaGroupPath.FullName); // Sicherstellen, dass die Datei gelöscht ist
        }

        // Act
        var result = command.Execute();

        // Assert
        Assert.IsTrue(result.IsSuccess);
        Assert.IsNotNull(commandEventArgs);
        Assert.AreEqual(expectedNewMediaGroupPath.FullName, commandEventArgs.FileInfo.FullName); // Die Datei ist im erwarteten Verzeichnis vorhanden
        Assert.IsTrue(commandEventArgs.FileInfo.Exists);
        Assert.IsFalse(commandEventArgs.HasMovedToExistingMediaGroup); // Die Datei wurde nicht in eine bestehende Mediengruppe verschoben
        Assert.IsFalse(commandEventArgs.HasTargetFileBeenOverwritten); // Die Datei wurde nicht überschrieben
    }

    [TestMethod] // Wird korrekt in eine bestehende Mediengruppe verschoben 
    public void Execute_ShouldMoveFileToExistingMediaGroup_WhenFileIsAssignedToExistingMediaGroup()
    {
        // Arrange
        var sourceFile = new FileInfo("Data/Testcase 5/2023-06-15 Balkenmäher.m4v");
        var mediaLibraryDirectory = new DirectoryInfo("Data/Output/Mediathek");
        var existingMediaGroupDirectory = new DirectoryInfo(Path.Combine(mediaLibraryDirectory.FullName, "Familie/2023/"));
        var fileInExistingMediaGroupDirectory = new FileInfo(Path.Combine(existingMediaGroupDirectory.FullName, sourceFile.Name));

        if (!existingMediaGroupDirectory.Exists) // Erstelle das Mediengruppenverzeichnis, falls es noch nicht existiert
        {
            existingMediaGroupDirectory.Create();
        }

        var expectedTargetFile = new FileInfo(fileInExistingMediaGroupDirectory.FullName);

        File.Copy(sourceFile.FullName, fileInExistingMediaGroupDirectory.FullName, true); // Kopiere die Datei um eine bestehende Mediengruppe zu simulieren

        var command = new MoveFileToMediaLibraryCommand
        {
            FilePath = sourceFile.FullName,
            MediaLibraryPath = mediaLibraryDirectory.FullName,
            Logger = new NullLogger<MoveFileToMediaLibraryCommand>()
        };

        FileMovedToMediaLibraryResultArgs? commandEventArgs = null; // Hilfsvariable für das Erfassen der Event-Informationen
        command.FileMovedToMediaLibrary += (sender, e) => commandEventArgs = e;

        // Act
        var result = command.Execute(); // Verschiebe die Datei erneut (in die gerade erstellte Mediengruppe)

        // Assert
        Assert.IsTrue(result.IsSuccess);
        Assert.IsTrue(commandEventArgs != null);
        Assert.AreEqual(expectedTargetFile.FullName, commandEventArgs?.FileInfo.FullName);
        Assert.IsTrue(commandEventArgs?.FileInfo.Exists);
        Assert.IsTrue(commandEventArgs?.HasMovedToExistingMediaGroup); // Die Datei wurde in eine bestehende Mediengruppe verschoben
        Assert.IsTrue(commandEventArgs?.HasTargetFileBeenOverwritten); // Die Datei wurde überschrieben
    }
}