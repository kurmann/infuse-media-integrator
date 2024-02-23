using Kurmann.InfuseMediaIntegrator.Entities;

namespace Kurmann.InfuseMediaIntegrator.Tests.Entities
{
    [TestClass]
    public class CategoryInfoTests
    {
        private const string RootPath = "Data/Input/Testcase 3";
        private const string DirectoryPath = "Data/Input/Testcase 3/Lyssach/Garten/";

        [TestMethod]
        public void Create_ShouldReturnSuccess_WhenRootAndDirectoryExist()
        {
            // Arrange

            // Act
            var result = CategoryInfo.CreateFromDirectoryStructure(RootPath, DirectoryPath);

            // Assert
            Assert.IsTrue(result.IsSuccess);
            Assert.IsNotNull(result.Value);
            CollectionAssert.Contains(result.Value.Categories, "Lyssach");
            CollectionAssert.Contains(result.Value.Categories, "Garten");
            Assert.AreEqual(2, result.Value.Categories.Count);
            Assert.AreEqual("Lyssach", result.Value.Categories.ElementAt(0));
            Assert.AreEqual("Garten", result.Value.Categories.ElementAt(1));
        }

        [TestMethod]
        public void Create_ShouldReturnSuccess_WhenRootAndDirectoryAreTheSame()
        {
            // Arrange

            // Act
            var result = CategoryInfo.CreateFromDirectoryStructure(RootPath, RootPath);

            // Assert
            Assert.IsTrue(result.IsSuccess);
            Assert.IsNotNull(result.Value);
            Assert.AreEqual(0, result.Value.Categories.Count);
        }

        [TestMethod]
        public void Create_ShouldReturnSuccess_WhenRootAndDirectoryAreTheSameButWithDifferentSeparators()
        {
            // Arrange

            // Act
            var result = CategoryInfo.CreateFromDirectoryStructure(RootPath, RootPath + "/");

            // Assert
            Assert.IsTrue(result.IsSuccess);
            Assert.IsNotNull(result.Value);
            Assert.AreEqual(0, result.Value.Categories.Count);
        }

        [TestMethod]
        public void Create_ShouldReturnFailure_WhenRootPathIsNull()
        {
            // Arrange

            // Act
            var result = CategoryInfo.CreateFromDirectoryStructure(null, DirectoryPath);

            // Assert
            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual("RootPath is null or empty", result.Error);
        }

        [TestMethod]
        public void Create_ShouldReturnFailure_WhenDirectoryPathIsNull()
        {
            // Arrange

            // Act
            var result = CategoryInfo.CreateFromDirectoryStructure(RootPath, null);

            // Assert
            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual("DirectoryPath is null or empty", result.Error);
        }

        [TestMethod]
        public void Create_ShouldReturnFailure_WhenRootPathDoesNotExist()
        {
            // Arrange

            // Act
            var result = CategoryInfo.CreateFromDirectoryStructure("NonExistingRootPath", DirectoryPath);

            // Assert
            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual("RootPath 'NonExistingRootPath' does not exist", result.Error);
        }

        [TestMethod]
        public void Create_ShouldReturnFailure_WhenDirectoryPathDoesNotExist()
        {
            // Arrange

            // Act
            var result = CategoryInfo.CreateFromDirectoryStructure(RootPath, "NonExistingDirectoryPath");

            // Assert
            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual("DirectoryPath 'NonExistingDirectoryPath' does not exist", result.Error);
        }

        [TestMethod] // Teste die Komma-separierten Kategorien
        public void Create_ShouldReturnSuccess_WhenCategoriesAreValid()
        {
            // Arrange
            const string categories = "Root,Category1,Category2";

            // Act
            var result = CategoryInfo.CreateFromCommaSeparatedList(categories);

            // Assert
            Assert.IsTrue(result.IsSuccess);
            Assert.IsNotNull(result.Value);
            CollectionAssert.Contains(result.Value.Categories, "Root");
            CollectionAssert.Contains(result.Value.Categories, "Category1");
            CollectionAssert.Contains(result.Value.Categories, "Category2");
        }

        [TestMethod]
        public void Create_ShouldReturnFailure_WhenCategoriesAreNull()
        {
            // Arrange

            // Act
            var result = CategoryInfo.CreateFromCommaSeparatedList(null);

            // Assert
            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual("Categories is null or empty", result.Error);
        }
    }
}