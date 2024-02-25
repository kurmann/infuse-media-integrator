using Kurmann.InfuseMediaIntegrator.Entities;

namespace Kurmann.InfuseMediaIntegrator.Tests.Entities;

[TestClass]
public class CategoryPathTests
{
    [TestMethod]
    public void Create_ShouldReturnSuccess_WhenPathIsValid()
    {
        // Arrange
        string path = "Root/Category1/Category2";

        // Act
        var result = CategoryPath.Create(path);

        // Assert
        Assert.IsTrue(result.IsSuccess);
        Assert.IsNotNull(result.Value);
        CollectionAssert.AreEqual(new[] { "Root", "Category1", "Category2" }, result.Value.Categories.ToArray());
    }

    [TestMethod]
    public void Create_ShouldReturnFailure_WhenPathIsNull()
    {
        // Arrange
        string? path = null;

        // Act
        var result = CategoryPath.Create(path);

        // Assert
        Assert.IsFalse(result.IsSuccess);
        Assert.AreEqual("Path is null or empty", result.Error);
    }

    [TestMethod]
    public void Create_ShouldReturnFailure_WhenPathIsEmpty()
    {
        // Arrange
        string path = "";

        // Act
        var result = CategoryPath.Create(path);

        // Assert
        Assert.IsFalse(result.IsSuccess);
        Assert.AreEqual("Path is null or empty", result.Error);
    }

    [TestMethod]
    public void Create_ShouldReturnSuccess_WhenPathContainsLeadingAndTrailingSpaces()
    {
        // Arrange
        string path = "  Root/Category1/Category2  ";

        // Act
        var result = CategoryPath.Create(path);

        // Assert
        Assert.IsTrue(result.IsSuccess);
        Assert.IsNotNull(result.Value);
        CollectionAssert.AreEqual(new[] { "Root", "Category1", "Category2" }, result.Value.Categories.ToArray());
    }

    [TestMethod]
    public void Create_ShouldReturnSuccess_WhenPathContainsEmptyCategories()
    {
        // Arrange
        string path = "Root//Category1//Category2";

        // Act
        var result = CategoryPath.Create(path);

        // Assert
        Assert.IsTrue(result.IsSuccess);
        Assert.IsNotNull(result.Value);
        CollectionAssert.AreEqual(new[] { "Root", "Category1", "Category2" }, result.Value.Categories.ToArray());
    }

    [TestMethod] // Prüfe ob ungültige Zeichen erkannt werden
    public void Create_ShouldReturnFailure_WhenPathContainsInvalidCharacters()
    {
        // Arrange
        string path = "Root/Category1/Category2" + Path.GetInvalidPathChars().First();

        // Act
        var result = CategoryPath.Create(path);

        // Assert
        Assert.IsFalse(result.IsSuccess);
        Assert.AreEqual($"Path contains invalid characters: {Path.GetInvalidPathChars().First()}", result.Error);
    }

    [TestMethod] // Prüfe ob nach dem Erstellen des CategoryPath noch eine weitere Kategorie hinzugefügt werden kann
    public void AddCategory_ShouldReturnSuccess_WhenAdditionalCategoryIsAddedAfterCreating()
    {
        // Arrange
        var categoryPath = CategoryPath.Create("Root/Category1/Category2").Value;

        // Act
        var result = categoryPath.AddCategory("Category3");

        // Assert
        Assert.IsTrue(result.IsSuccess);
        CollectionAssert.AreEqual(new[] { "Root", "Category1", "Category2", "Category3" }, categoryPath.Categories.ToArray());
    }
}