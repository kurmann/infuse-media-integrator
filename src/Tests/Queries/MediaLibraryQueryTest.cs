using Kurmann.InfuseMediaIntegrator.Queries;

namespace Kurmann.InfuseMediaIntegrator.Tests.Queries
{
    [TestClass]
    public class MediaLibraryQueryTests
    {
        public const string MediaLibraryPath = "Data/Outout/Mediathek";

        [TestMethod]
        public void Execute_ShouldReturnFailure_WhenMediaLibraryPathIsEmpty()
        {
            // Arrange
            var query = new MediaLibraryQuery(string.Empty);

            // Act
            var result = query.Execute();

            // Assert
            Assert.IsFalse(result.IsSuccess);
            Assert.IsNotNull(result.Error);
            Assert.AreEqual("Media library path is empty.", result.Error);
        }
    }
}