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
        var expectedNewMediaGroupPath = new FileInfo(Path.Combine(command.MediaLibraryPath, "2023-03-14 Primeli Kurzaufnahme/2023-03-14 Primeli Kurzaufnahme.m4v"));

        if (File.Exists(expectedNewMediaGroupPath.FullName))
        {
            File.Delete(expectedNewMediaGroupPath.FullName); // Sicherstellen, dass die Datei gelöscht ist
        }

        // Act
        var result = command.Execute();

        // Assert
        Assert.IsTrue(result.IsSuccess);
        Assert.IsNotNull(result.Value);
        Assert.AreEqual(expectedNewMediaGroupPath.FullName, result.Value.MediaFile.FilePath); // Die Datei ist im erwarteten Verzeichnis vorhanden
        Assert.IsTrue(Path.Exists(result.Value.MediaFile.FilePath));
        Assert.IsFalse(result.Value.HasMovedToExistingMediaGroup); // Die Datei wurde nicht in eine bestehende Mediengruppe verschoben
        Assert.IsFalse(result.Value.HasTargetFileBeenOverwritten); // Die Datei wurde nicht überschrieben
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

        // Act
        var result = command.Execute(); // Verschiebe die Datei erneut (in die gerade erstellte Mediengruppe)

        // Assert
        Assert.IsTrue(result.IsSuccess);
        Assert.IsTrue(result.Value != null);
        Assert.AreEqual(expectedTargetFile.FullName, result.Value.MediaFile.FilePath);
        Assert.IsTrue(Path.Exists(result.Value.MediaFile.FilePath));
        Assert.IsTrue(result.Value.HasMovedToExistingMediaGroup); // Die Datei wurde in eine bestehende Mediengruppe verschoben
        Assert.IsTrue(result.Value.HasTargetFileBeenOverwritten); // Die Datei wurde überschrieben
    }
}