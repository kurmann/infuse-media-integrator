
using Kurmann.InfuseMediaIntegrator.Tests.Entities;
using Microsoft.Extensions.Logging.Abstractions;

namespace Kurmann.InfuseMediaIntegrator.Tests.Commands
{
    [TestClass]
    public class MoveFilesToInfuseMediaLibraryCommandTests
    {
        private const string InputDirectoryPath = "Data/Input/Testcase 1";
        private const string InfuseMediaLibraryPath = "Data/Output/Mediathek";
        
        [TestMethod]
        public void Execute_ShouldMoveFilesToInfuseMediaLibrary()
        {
            // Arrange
            var logger = new NullLogger<MoveFilesToInfuseMediaLibraryCommand>();
            var command = new MoveFilesToInfuseMediaLibraryCommand(InputDirectoryPath, InfuseMediaLibraryPath, logger);

            // Act
            var result = command.Execute();

            // Assert
            Assert.IsTrue(result.IsSuccess);
            Assert.IsTrue(Directory.Exists(InfuseMediaLibraryPath));

            // Verify that the files are moved
            var mpeg4VideoFiles = Directory.GetFiles(InputDirectoryPath, "*.m4v");
            foreach (var file in mpeg4VideoFiles)
            {
                var targetPath = Path.Combine(InfuseMediaLibraryPath, Path.GetFileName(file));
                Assert.IsTrue(File.Exists(targetPath));
            }

            // Clean up
        }

        [TestMethod]
        public void Execute_ShouldReturnFailure_WhenInputDirectoryNotFound()
        {
            // Arrange
            var logger = new NullLogger<MoveFilesToInfuseMediaLibraryCommand>();
            var nonExistingInputDirectoryPath = "NonExistingDirectory";
            var command = new MoveFilesToInfuseMediaLibraryCommand(nonExistingInputDirectoryPath, InfuseMediaLibraryPath, logger);

            // Act
            var result = command.Execute();

            // Assert
            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual($"Directory not found: {nonExistingInputDirectoryPath}", result.Error);
        }

        [TestMethod]
        public void Execute_ShouldReturnFailure_WhenInfuseMediaLibraryNotFound()
        {
            // Arrange
            var logger = new NullLogger<MoveFilesToInfuseMediaLibraryCommand>();
            var nonExistingInfuseMediaLibraryPath = "NonExistingDirectory";
            var command = new MoveFilesToInfuseMediaLibraryCommand(InputDirectoryPath, nonExistingInfuseMediaLibraryPath, logger);

            // Act
            var result = command.Execute();

            // Assert
            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual($"Directory not found: {nonExistingInfuseMediaLibraryPath}", result.Error);
        }
    }
}