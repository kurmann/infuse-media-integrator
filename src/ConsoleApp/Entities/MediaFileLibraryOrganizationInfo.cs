namespace Kurmann.InfuseMediaIntegrator.Entities;

/// <summary>
/// Enthält Informationen um eine Mediendatei in einer Mediathek zu organisieren.
/// </summary>
public class MediaFileLibraryOrganizationInfo
{
    public string? LibraryName { get; }
    public string? LibraryPath { get; }
    public string? LibraryDescription { get; }

    public MediaFileLibraryOrganizationInfo(string? libraryName, string? libraryPath, string? libraryDescription)
    {
        LibraryName = libraryName;
        LibraryPath = libraryPath;
        LibraryDescription = libraryDescription;
    }
}