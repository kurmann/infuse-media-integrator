using Kurmann.InfuseMediaIntegrator.Entities.MediaLibrary;

namespace Kurmann.InfuseMediaIntegrator.Tests.Entities
{
    [TestClass]
    public class LibraryFanartImageFileNameTests
    {
        [TestMethod]
        public void CreateWithAddedFanartPrefix_ShouldReturnUnchangedFileName_WhenFileNameEndsWithFanart()
        {
            // Arrange
            var fileName = "example-fanart.jpg";

            // Act
            var result = LibraryFanartImageFileName.CreateWithAddedFanartPrefix(fileName);

            // Assert
            Assert.IsTrue(result.IsSuccess);
            Assert.AreEqual(fileName, result.Value.FileName);
        }

        [TestMethod]
        public void CreateWithAddedFanartPrefix_ShouldReturnNewFileNameWithFanartPrefix_WhenFileNameDoesNotEndWithFanart()
        {
            // Arrange
            var fileName = "example.jpg";
            var expectedNewFileName = "example-fanart.jpg";

            // Act
            var result = LibraryFanartImageFileName.CreateWithAddedFanartPrefix(fileName);

            // Assert
            Assert.IsTrue(result.IsSuccess);
            Assert.AreEqual(expectedNewFileName, result.Value.FileName);
        }

        [TestMethod]
        public void CreateWithAddedFanartPrefix_ShouldReturnFailure_WhenFileNameInfoCreationFails()
        {
            // Arrange
            var fileName = "invalid-file-name";

            // Act
            var result = LibraryFanartImageFileName.CreateWithAddedFanartPrefix(fileName);

            // Assert
            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual("Error on reading file info: File name is not a file", result.Error); // Replace <error-message> with the actual error message
        }
    }
}