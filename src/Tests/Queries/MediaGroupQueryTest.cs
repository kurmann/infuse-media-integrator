using Kurmann.InfuseMediaIntegrator.Queries;

namespace Kurmann.InfuseMediaIntegrator.Tests.Queries;

[TestClass]
public class MediaGroupQueryTests
{
    public const string MediaLibraryPath = "Data/Output/Mediathek";

    [TestMethod] // Testet, ob die Suche nach einer ID korrekt funktioniert
    public void WithId_ShouldReturnCorrectResult_WhenIdIsSet()
    {
        // Arrange
        var query = new MediaGroupQuery(MediaLibraryPath).ById("2024-02-16 Krokus Testaufnahme");

        // Act
        var result = query.Execute();

        // Assert
        Assert.IsTrue(result.IsSuccess);
        Assert.IsNotNull(result.Value);
        Assert.AreEqual("Data/Output/Mediathek/Familie Kurmann-Glück/2024/2024-02-16 Krokus Testaufnahme", result.Value.DirectoryPath);
    }
}