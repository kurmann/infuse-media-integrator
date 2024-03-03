
using Kurmann.InfuseMediaIntegrator.Tests.Entities;
using Microsoft.Extensions.Logging.Abstractions;

namespace Kurmann.InfuseMediaIntegrator.Tests.Commands;

[TestClass]
public class MoveFilesToInfuseMediaLibraryCommandTest
{
    private const string MediaLibraryPath = "Data/Output/Mediathek";

    [TestMethod]
    public void Execute_ShouldReturnFailure_WhenInputDirectoryPathIsEmpty()
    {
        // Arrange
        var command = new MoveFilesToInfuseMediaLibraryCommand(string.Empty, MediaLibraryPath, new NullLogger<MoveFilesToInfuseMediaLibraryCommand>());

        // Act
        var result = command.Execute();

        // Assert
        Assert.IsFalse(result.IsSuccess);
        Assert.IsNotNull(result.Error);
        Assert.AreEqual("Directory not found: ", result.Error);
    }

    [TestMethod]
    public void Execute_ShouldReturnFailure_WhenInfuseMediaLibraryPathIsEmpty()
    {
        // Arrange
        var command = new MoveFilesToInfuseMediaLibraryCommand(MediaLibraryPath, string.Empty, new NullLogger<MoveFilesToInfuseMediaLibraryCommand>());

        // Act
        var result = command.Execute();

        // Assert
        Assert.IsFalse(result.IsSuccess);
        Assert.IsNotNull(result.Error);
        Assert.AreEqual("Directory not found: ", result.Error);
    }

    [TestMethod]
    public void Execute_ShouldReturnFailure_WhenInputDirectoryPathDoesNotExist()
    {
        // Arrange
        var command = new MoveFilesToInfuseMediaLibraryCommand("Data/Output/NotExisting", MediaLibraryPath, new NullLogger<MoveFilesToInfuseMediaLibraryCommand>());

        // Act
        var result = command.Execute();

        // Assert
        Assert.IsFalse(result.IsSuccess);
        Assert.IsNotNull(result.Error);
        Assert.AreEqual("Directory not found: Data/Output/NotExisting", result.Error);
    }

    [TestMethod]
    public void Execute_ShouldReturnFailure_WhenInfuseMediaLibraryPathDoesNotExist()
    {
        // Arrange
        var command = new MoveFilesToInfuseMediaLibraryCommand(MediaLibraryPath, "Data/Output/NotExisting", new NullLogger<MoveFilesToInfuseMediaLibraryCommand>());

        // Act
        var result = command.Execute();

        // Assert
        Assert.IsFalse(result.IsSuccess);
        Assert.IsNotNull(result.Error);
        Assert.AreEqual("Directory not found: Data/Output/NotExisting", result.Error);
    }
}