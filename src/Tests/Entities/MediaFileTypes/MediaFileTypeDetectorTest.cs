using Kurmann.InfuseMediaIntegrator.Entities.MediaFileTypes;

namespace Kurmann.InfuseMediaIntegrator.Tests.Entities;

[TestClass]
public class MediaFileTypeDetectorTests
{
    [TestMethod] // Bei Dateiendung .mp4 wird ein Mpeg4Video-Objekt zurückgegeben
    public void GetMediaFile_ShouldReturnMpeg4Video_WhenFileExtensionIsMp4()
    {
        // Arrange
        string path = "/volume/path/to/file.mp4";

        // Act
        var result = MediaFileTypeDetector.GetMediaFile(path);

        // Assert
        Assert.IsTrue(result.IsSuccess);
        Assert.IsNotNull(result.Value);
        Assert.IsInstanceOfType(result.Value, typeof(Mpeg4Video));
    }

    [TestMethod] // Bei Dateiendung .mov wird ein QuickTimeVideo-Objekt zurückgegeben
    public void GetMediaFile_ShouldReturnQuickTimeVideo_WhenFileExtensionIsMov()
    {
        // Arrange
        string path = "/volume/path/to/file.mov";

        // Act
        var result = MediaFileTypeDetector.GetMediaFile(path);

        // Assert
        Assert.IsTrue(result.IsSuccess);
        Assert.IsNotNull(result.Value);
        Assert.IsInstanceOfType(result.Value, typeof(QuickTimeVideo));
    }

    [TestMethod] // Bei Dateiendung .jpg wird ein JpegImage-Objekt zurückgegeben
    public void GetMediaFile_ShouldReturnJpegImage_WhenFileExtensionIsJpg()
    {
        // Arrange
        string path = "/volume/path/to/file.jpg";

        // Act
        var result = MediaFileTypeDetector.GetMediaFile(path);

        // Assert
        Assert.IsTrue(result.IsSuccess);
        Assert.IsNotNull(result.Value);
        Assert.IsInstanceOfType(result.Value, typeof(JpegImage));
    }

    [TestMethod] // Bei Dateiendung .jpeg wird ein JpegImage-Objekt zurückgegeben
    public void GetMediaFile_ShouldReturnJpegImage_WhenFileExtensionIsJpeg()
    {
        // Arrange
        string path = "/volume/path/to/file.jpeg";

        // Act
        var result = MediaFileTypeDetector.GetMediaFile(path);

        // Assert
        Assert.IsTrue(result.IsSuccess);
        Assert.IsNotNull(result.Value);
        Assert.IsInstanceOfType(result.Value, typeof(JpegImage));
    }

    [TestMethod] // Bei Dateiendung .txt wird ein NotSupportedFile-Objekt zurückgegeben
    public void GetMediaFile_ShouldReturnNotSupportedFile_WhenFileExtensionIsTxt()
    {
        // Arrange
        string path = "/volume/path/to/file.txt";

        // Act
        var result = MediaFileTypeDetector.GetMediaFile(path);

        // Assert
        Assert.IsTrue(result.IsSuccess);
        Assert.IsNotNull(result.Value);
        Assert.IsInstanceOfType(result.Value, typeof(NotSupportedFile));
    }

    [TestMethod] // Bei Dateiendung .pdf wird ein NotSupportedFile-Objekt zurückgegeben
    public void GetMediaFile_ShouldReturnNotSupportedFile_WhenFileExtensionIsPdf()
    {
        // Arrange
        string path = "/volume/path/to/file.pdf";

        // Act
        var result = MediaFileTypeDetector.GetMediaFile(path);

        // Assert
        Assert.IsTrue(result.IsSuccess);
        Assert.IsNotNull(result.Value);
        Assert.IsInstanceOfType(result.Value, typeof(NotSupportedFile));
    }

    [TestMethod] // Bei einem Verzeichnispfad wird ein Fehler zurückgegeben
    public void GetMediaFile_ShouldReturnFailure_WhenPathIsDirectory()
    {
        // Arrange
        string path = "/volume/path/to/directory";

        // Act
        var result = MediaFileTypeDetector.GetMediaFile(path);

        // Assert
        Assert.IsTrue(result.IsFailure);
    }
}