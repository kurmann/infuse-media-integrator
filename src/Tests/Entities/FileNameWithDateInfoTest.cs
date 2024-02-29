using System.Reflection;
using Kurmann.InfuseMediaIntegrator.Entities;

namespace Kurmann.InfuseMediaIntegrator.Tests.Entities;

[TestClass]
public class FileNameWithDateInfoTests
{
    [TestMethod] // Erfolgreich wenn ISO-8601-Datum am Anfang des Dateinamens steht
    public void Create_ShouldReturnSuccess_WhenFileNameStartsWithISO8601Date()
    {
        // Arrange
        string fileName = "2021-12-31 example_file.txt";

        // Act
        var result = FileNameWithDateInfo.Create(fileName);

        // Assert
        Assert.IsTrue(result.IsSuccess);
        Assert.IsNotNull(result.Value);
        Assert.AreEqual(fileName, result.Value.FileName);
        Assert.AreEqual("2021-12-31", result.Value.DateString);
        Assert.AreEqual("2021-12-31 example_file.txt", result.Value.FileName);
        Assert.IsTrue(result.Value.IsDateAtStart);
        Assert.IsFalse(result.Value.IsDateAtEnd);
    }

    [TestMethod] // Nicht erfolgreich wenn kein Datum im Dateinamen steht
    public void Create_ShouldReturnFailure_WhenFileNameDoesNotContainDate()
    {
        // Arrange
        string fileName = "example_file.txt";

        // Act
        var result = FileNameWithDateInfo.Create(fileName);

        // Assert
        Assert.IsFalse(result.IsSuccess);
        Assert.IsNotNull(result.Error);
        Assert.AreEqual("No date found in file name", result.Error);
        // Add more assertions as needed
    }

    [TestMethod]
    public void Create_ShouldReturnFailure_WhenFileNameIsNull()
    {
        // Arrange
        string? fileName = null;

        // Act
        var result = FileNameWithDateInfo.Create(fileName);

        // Assert
        Assert.IsFalse(result.IsSuccess);
        Assert.IsNotNull(result.Error);
        // Add more assertions as needed
    }

    [TestMethod] // Erfolgreich wenn ISO-8601-Datum am Ende des Dateinamens steht
    public void Create_ShouldReturnSuccess_WhenFileNameEndsWithISO8601Date()
    {
        // Arrange
        string fileName = "example_file 2021-12-31.txt";

        // Act
        var result = FileNameWithDateInfo.Create(fileName);

        // Assert
        Assert.IsTrue(result.IsSuccess);
        Assert.IsNotNull(result.Value);
        Assert.AreEqual(fileName, result.Value.FileName);
        Assert.AreEqual("2021-12-31", result.Value.DateString);
        Assert.AreEqual("example_file 2021-12-31.txt", result.Value.FileName);
        Assert.IsFalse(result.Value.IsDateAtStart);
        Assert.IsTrue(result.Value.IsDateAtEnd);
    }

    [TestMethod] // Erfolgreich wenn deutsches Datum am Ende des Dateinamens steht
    public void Create_ShouldReturnSuccess_WhenFileNameEndsWithGermanDate()
    {
        // Arrange
        string fileName = "example_file 31.12.2021.txt";

        // Act
        var result = FileNameWithDateInfo.Create(fileName);

        // Assert
        Assert.IsTrue(result.IsSuccess);
        Assert.IsNotNull(result.Value);
        Assert.AreEqual(fileName, result.Value.FileName);
        Assert.AreEqual("31.12.2021", result.Value.DateString);
        Assert.AreEqual("example_file 31.12.2021.txt", result.Value.FileName);
        Assert.IsFalse(result.Value.IsDateAtStart);
        Assert.IsTrue(result.Value.IsDateAtEnd);
    }

    [TestMethod] // Erfolgreich wenn nur ein Jahr im Dateinamen steht
    public void Create_ShouldReturnSuccess_WhenFileNameContainsOnlyYear()
    {
        // Arrange
        string fileName = "2021 example_file.txt";

        // Act
        var result = FileNameWithDateInfo.Create(fileName);

        // Assert
        Assert.IsTrue(result.IsSuccess);
        Assert.IsNotNull(result.Value);
        Assert.AreEqual(fileName, result.Value.FileName);
        Assert.AreEqual("2021", result.Value.DateString);
        Assert.AreEqual("2021 example_file.txt", result.Value.FileName);
        Assert.IsTrue(result.Value.IsDateAtStart);
        Assert.IsFalse(result.Value.IsDateAtEnd);
        Assert.AreEqual(new DateOnly(2021, 12, 31), result.Value.Date);
    }
}