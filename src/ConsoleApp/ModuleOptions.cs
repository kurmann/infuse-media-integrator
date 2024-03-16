namespace Kurmann.InfuseMediaIntegratior;

public class ModuleOptions
{
    /// <summary>
    /// Der Pfad zur Infuse Media Library
    /// </summary>
    public string? MediaLibraryPath { get; init; }

    /// <summary>
    /// Der Pfad, der überwacht werden soll auf neue Dateien, die in die Mediathek verschoben werden sollen
    /// </summary>
    public string? WatchPath { get; init; }
}