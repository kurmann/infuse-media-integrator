using Kurmann.InfuseMediaIntegrator.Entities.Elementary;

namespace Kurmann.InfuseMediaIntegrator.Tests.Entities.Elementary;

[TestClass]
public class DirectoryPathInfoTests
{
    [TestMethod]
    public void Create_ShouldReturnSuccess_WhenPathIsValidDirectory()
    {
        // Arrange
        string path = "/workspaces/infuse-media-integrator/src/ConsoleApp/Entities";

        // Act
        var result = DirectoryPathInfo.Create(path);

        // Assert
        Assert.IsTrue(result.IsSuccess);
        Assert.IsNotNull(result.Value);
        Assert.AreEqual(path, result.Value);
    }

    [TestMethod] // Teste ob mit Angabe eines Verzeichnisnamens ein gültiges DirectoryPathInfo-Objekt erstellt wird
    public void Create_ShouldReturnSuccess_WhenPathIsValidDirectoryWithDirectoryName()
    {
        // Arrange
        string path = "Familiy Movies";

        // Act
        var result = DirectoryPathInfo.Create(path);

        // Assert
        Assert.IsTrue(result.IsSuccess);
        Assert.IsNotNull(result.Value);
        Assert.AreEqual(path, result.Value.DirectoryPath);
    }


    [TestMethod]
    public void Create_ShouldReturnFailure_WhenPathIsNull()
    {
        // Arrange
        string? path = null;

        // Act
        var result = DirectoryPathInfo.Create(path);

        // Assert
        Assert.IsFalse(result.IsSuccess);
        Assert.IsNotNull(result.Error);
        Assert.AreEqual("Path is null or empty", result.Error);
    }

    [TestMethod]
    public void Create_ShouldReturnFailure_WhenPathIsEmpty()
    {
        // Arrange
        string path = "";

        // Act
        var result = DirectoryPathInfo.Create(path);

        // Assert
        Assert.IsFalse(result.IsSuccess);
        Assert.IsNotNull(result.Error);
        Assert.AreEqual("Path is null or empty", result.Error);
    }

    [TestMethod] // Test ob der Pfad korrekt ausgelesen wird wenn eine Datei angegeben wird
    public void Create_ShouldReturnSuccess_WhenPathIsFile()
    {
        // Arrange
        string path = "/workspaces/infuse-media-integrator/src/ConsoleApp/Entities/Elementary/DirectoryPathInfo.cs";

        // Act
        var result = DirectoryPathInfo.Create(path);

        // Assert
        Assert.IsTrue(result.IsSuccess);
        Assert.IsNotNull(result.Value);
        Assert.AreEqual("/workspaces/infuse-media-integrator/src/ConsoleApp/Entities/Elementary", result.Value);
    }

    [TestMethod]
    public void Create_ShouldReturnFailure_WhenPathContainsInvalidCharacters()
    {
        // Arrange
        string path = "/workspaces/infuse-media-integrator/src/ConsoleApp/Entities/Invalid?Directory";

        // Act
        var result = DirectoryPathInfo.Create(path);

        // Assert
        Assert.IsFalse(result.IsSuccess);
        Assert.IsNotNull(result.Error);
    }
}