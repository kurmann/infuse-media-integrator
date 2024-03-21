namespace Kurmann.InfuseMediaIntegratior;

public class AppSettings
{
    /// <summary>
    /// Der Pfad zur Mediathek
    /// </summary>
    public string? MediaLibraryPath { get; init; }

    /// <summary>
    /// Die Konfiguration für das lokale Dateisystem
    /// </summary>
    public InputDirectorySettings? InputDirectory { get; init; }
}

public class InputDirectorySettings
{
    /// <summary>
    /// Der Pfad, der überwacht werden soll auf neue Dateien, die in die Mediathek verschoben werden sollen
    /// </summary>
    public string? Path { get; init; }

    /// <summary>
    /// Die Dateiendungen, die überwacht werden sollenÍ
    /// </summary>
    public List<string> ExtensionsToWatch { get; set; } = [".m4v", ".mp4v", ".mov", ".jpg", ".jpeg", ".png"];
}
