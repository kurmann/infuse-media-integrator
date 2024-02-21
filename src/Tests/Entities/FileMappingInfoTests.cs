using Kurmann.InfuseMediaIntegrator.Entities;

namespace Kurmann.InfuseMediaIntegrator.Tests.Entities;

[TestClass]
public class FileMappingInfoTests
{
    [TestMethod]
    public void Create_ShouldReturnFailure_WhenCategoryIsNullOrWhitespace()
    {
        // Arrange
        var category = string.Empty;
        var fileName = "2024-21-03 Ausflug nach Willisau.m4v";
        
        // Act
        var result = FileMappingInfo.Create(category, fileName);
        
        // Assert
        StringAssert.Contains(result.Error, "Category cannot be null or whitespace.");
        Assert.IsTrue(result.IsFailure);
    }

    [TestMethod]
    public void Create_ShouldReturnFailure_WhenFileNameDoesNotHaveExtension()
    {
        // Arrange
        var category = "Familie";
        var fileName = "2024-21-03 Ausflug nach Willisau";
        
        // Act
        var result = FileMappingInfo.Create(category, fileName);
        
        // Assert
        StringAssert.Contains(result.Error, "File name does not match the expected format");
    }

    [TestMethod]
    public void Create_ShouldReturnFailure_WhenFileNameDoesNotHaveYear()
    {
        // Arrange
        var category = "Familie";
        var fileName = "Ausflug nach Willisau.m4v";
        
        // Act
        var result = FileMappingInfo.Create(category, fileName);
        
        // Assert
        StringAssert.Contains(result.Error, "File name does not match the expected format");
    }

    [TestMethod]
    public void Create_ShouldReturnFailure_WhenFileNameHaveNoWhitespaceBetweenDateAndTitle()
    {
        // Arrange
        var category = "Familie";
        var fileName = "2024-21-03Ausflug nach Willisau.m4v";
        
        // Act
        var result = FileMappingInfo.Create(category, fileName);
        
        // Assert
        StringAssert.Contains(result.Error, "File name does not match the expected format");
    }

    [TestMethod]
    public void Create_ShouldReturnSuccess_WhenFileNameHaveYearAndTitle()
    {
        // Arrange
        var category = "Familie";
        var fileName = "2024 Ausflug nach Willisau.m4v";
        
        // Act
        var result = FileMappingInfo.Create(category, fileName);
        
        // Assert
        Assert.IsTrue(result.IsSuccess);
        Assert.AreEqual(2024, result.Value.Year);
    }

    [TestMethod]
    public void Create_ShouldReturnFailure_WhenYearIsOlderThan1900()
    {
        // Arrange
        var category = "Familie";
        var fileName = "1899-21-03 Ausflug nach Willisau.m4v";
        
        // Act
        var result = FileMappingInfo.Create(category, fileName);
        
        // Assert
        StringAssert.Contains(result.Error, "File name does not match the expected format");
    }

    [TestMethod]
    public void Create_ShouldReturnSuccess_WhenFileNameHasYearWithMonthAndTitle()
    {
        // Arrange
        var category = "Familie";
        var fileName = "2024-12 Ausflug nach Willisau.m4v";
        
        // Act
        var result = FileMappingInfo.Create(category, fileName);
        
        // Assert
        Assert.IsTrue(result.IsSuccess);
        Assert.AreEqual(2024, result.Value.Year);
    }

    [TestMethod]
    public void Create_ShouldReturnSuccess_WhenFileNameHasIsoDateAndTitle()
    {
        // Arrange
        var category = "Familie";
        var fileName = "2024-21-03 Ausflug nach Willisau.m4v";
        
        // Act
        var result = FileMappingInfo.Create(category, fileName);
        
        // Assert
        Assert.IsTrue(result.IsSuccess);
        Assert.AreEqual(category, result.Value.Category);
        Assert.AreEqual(2024, result.Value.Year);
        Assert.AreEqual(fileName, result.Value.SourcePath);
        Assert.AreEqual("Familie/2024/2024-21-03 Ausflug nach Willisau.m4v", result.Value.TargetPath);
    }

    [TestMethod]
    public void Create_ShouldReturnSuccess_WhenFileNameHasIsoDateAndTitleWithUmlaut()
    {
        // Arrange
        var category = "Familie";
        var fileName = "2024-21-03 Ausflug nach Willisäu.m4v";
        
        // Act
        var result = FileMappingInfo.Create(category, fileName);
        
        // Assert
        Assert.IsTrue(result.IsSuccess);
        Assert.AreEqual(category, result.Value.Category);
        Assert.AreEqual(2024, result.Value.Year);
        Assert.AreEqual(fileName, result.Value.SourcePath);
        Assert.AreEqual("Familie/2024/2024-21-03 Ausflug nach Willisäu.m4v", result.Value.TargetPath);
    }

    [TestMethod]
    public void Create_ShouldReturnSuccess_WhenFileNameHasIsoDateAndTitleWithSpecialCharacters()
    {
        // Arrange
        var category = "Familie";
        var fileName = "2024-21-03 Ausflug nach Willisäu!?.m4v";
        
        // Act
        var result = FileMappingInfo.Create(category, fileName);
        
        // Assert
        Assert.IsTrue(result.IsSuccess);
        Assert.AreEqual(category, result.Value.Category);
        Assert.AreEqual(2024, result.Value.Year);
        Assert.AreEqual(fileName, result.Value.SourcePath);
        Assert.AreEqual("Familie/2024/2024-21-03 Ausflug nach Willisäu!?.m4v", result.Value.TargetPath);
    }

    [TestMethod]
    public void Create_ShouldRecognizeJpgFileAsFanart()
    {
        // Arrange
        var category = "Familie";
        var fileName = "2024-21-03 Ausflug nach Willisau.jpg";
        
        // Act
        var result = FileMappingInfo.Create(category, fileName);
        
        // Assert
        Assert.IsTrue(result.IsSuccess);
        Assert.AreEqual(InfuseMediaType.FanartImage, result.Value.MediaType);
        Assert.AreEqual(category, result.Value.Category);
        Assert.AreEqual(fileName, result.Value.SourcePath);
    }

    [TestMethod] /// prüft, ob InfuseMediaType.MovieFile zurückgegeben wird, wenn die Dateiendung nicht .jpg oder .jpeg ist
    public void Create_ShouldRecognizeNonJpgFileAsMovieFile()
    {
        // Arrange
        var category = "Familie";
        var fileName = "2024-21-03 Ausflug nach Willisau.m4v";
        
        // Act
        var result = FileMappingInfo.Create(category, fileName);
        
        // Assert
        Assert.IsTrue(result.IsSuccess);
        Assert.AreEqual(InfuseMediaType.MovieFile, result.Value.MediaType);
        Assert.AreEqual(category, result.Value.Category);
        Assert.AreEqual(fileName, result.Value.SourcePath);
    }

    [TestMethod] /// Prüft, ob Dateien vom InfuseMediaType.Fanart das Präfix "-fanart" erhalten vor der Dateiendung
    public void Create_ShouldReturnFanartImage_WhenMediaTypeIsFanartImage()
    {
        // Arrange
        var category = "Familie";
        var fileName = "2024-21-03 Ausflug nach Willisau.jpg";
        
        // Act
        var result = FileMappingInfo.Create(category, fileName);
        
        // Assert
        Assert.IsTrue(result.IsSuccess);
        Assert.AreEqual(InfuseMediaType.FanartImage, result.Value.MediaType);
        Assert.AreEqual(category, result.Value.Category);
        Assert.AreEqual(fileName, result.Value.SourcePath);
        Assert.AreEqual("Familie/2024/2024-21-03 Ausflug nach Willisau-fanart.jpg", result.Value.TargetPath);
    }

}