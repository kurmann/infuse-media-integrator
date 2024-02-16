namespace Kurmann.InfuseMediaIntegrator.Tests;

public class TestBase
{
    // Der Basispfad für alle Input-Verzeichnisse
    protected string BaseInputDirectoryPath => Path.Combine(Directory.GetCurrentDirectory(), "tests", "input");

    // Methode, um den vollständigen Pfad zu einem bestimmten Testverzeichnis zu erhalten
    protected string GetInputDirectoryPath(string directoryName)
    {
        return Path.Combine(BaseInputDirectoryPath, directoryName);
    }
}
