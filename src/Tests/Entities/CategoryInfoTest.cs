using Kurmann.InfuseMediaIntegrator.Entities;

namespace Kurmann.InfuseMediaIntegrator.Tests.Entities
{
    [TestClass]
    public class CategoryInfoTests
    {
        private const string RootPath = "Data/Root";
        private const string DirectoryPath = "Data/Root/Category1";

        [TestMethod]
        public void Create_ShouldReturnSuccess_WhenRootAndDirectoryExist()
        {
            // Arrange

            // Act
            var result = CategoryInfo.Create(RootPath, DirectoryPath);

            // Assert
            Assert.IsTrue(result.IsSuccess);
            Assert.IsNotNull(result.Value);
            CollectionAssert.Contains(result.Value.Categories, "Root");
            CollectionAssert.Contains(result.Value.Categories, "Category1");
        }

        [TestMethod]
        public void Create_ShouldReturnFailure_WhenRootPathIsNull()
        {
            // Arrange

            // Act
            var result = CategoryInfo.Create(null, DirectoryPath);

            // Assert
            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual("RootPath is null or empty", result.Error);
        }

        [TestMethod]
        public void Create_ShouldReturnFailure_WhenDirectoryPathIsNull()
        {
            // Arrange

            // Act
            var result = CategoryInfo.Create(RootPath, null);

            // Assert
            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual("DirectoryPath is null or empty", result.Error);
        }

        [TestMethod]
        public void Create_ShouldReturnFailure_WhenRootPathDoesNotExist()
        {
            // Arrange

            // Act
            var result = CategoryInfo.Create("NonExistingRootPath", DirectoryPath);

            // Assert
            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual("RootPath 'NonExistingRootPath' does not exist", result.Error);
        }

        [TestMethod]
        public void Create_ShouldReturnFailure_WhenDirectoryPathDoesNotExist()
        {
            // Arrange

            // Act
            var result = CategoryInfo.Create(RootPath, "NonExistingDirectoryPath");

            // Assert
            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual("DirectoryPath 'NonExistingDirectoryPath' does not exist", result.Error);
        }
    }
}