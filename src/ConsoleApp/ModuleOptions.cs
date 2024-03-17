namespace Kurmann.InfuseMediaIntegratior;

public class ModuleOptions
{
    /// <summary>
    /// Die Konfiguration für das lokale Dateisystem
    /// </summary>
    public LocalFileSystemOptions? LocalFileSystem { get; init; }
}

public class LocalFileSystemOptions
{
    /// <summary>
    /// Der Pfad zur Infuse Media Library
    /// </summary>
    public string? MediaLibraryPath { get; init; }

    /// <summary>
    /// Der Pfad, der überwacht werden soll auf neue Dateien, die in die Mediathek verschoben werden sollen
    /// </summary>
    public string? WatchPath { get; init; }

    /// <summary>
    /// Die Dateiendungen, die überwacht werden sollen
    /// </summary>
    public List<string> WatchedFileExtensions { get; set; } = [".m4v", ".mp4v", ".mov", ".jpg", ".jpeg", ".png"];
}
