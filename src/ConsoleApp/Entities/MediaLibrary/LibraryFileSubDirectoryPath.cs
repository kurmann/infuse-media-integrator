using CSharpFunctionalExtensions;
using Kurmann.InfuseMediaIntegrator.Entities.Elementary;

namespace Kurmann.InfuseMediaIntegrator.Entities.MediaLibrary;

/// <summary>
/// Repräsentiert den Pfad eines Unterverzeichnisses in einer Mediathek.
/// Wird verwendet um Dateien in der Mediathek zu organisieren, insbesondere um Dateien in Unterverzeichnisse zu verschieben.
/// </summary>
public class LibraryFileSubDirectoryPath
{
    /// <summary>
    /// Der Pfad des Unterverzeichnisses.
    /// </summary>
    public DirectoryPathInfo DirectoryPath { get; }

    private LibraryFileSubDirectoryPath(DirectoryPathInfo directoryPath) => DirectoryPath = directoryPath;

    /// <summary>
    /// Erstellt ein LibraryFileSubDirectoryPath-Objekt.
    /// </summary>
    /// <param name="directoryPath"></param>
    /// <returns></returns>
    public static Result<LibraryFileSubDirectoryPath> Create(string? directoryPath)
    {
        // Erstelle ein DirectoryPathInfo-Objekt
        var directoryPathInfo = DirectoryPathInfo.Create(directoryPath);
        if (directoryPathInfo.IsFailure)
            return Result.Failure<LibraryFileSubDirectoryPath>($"Error on reading directory info: {directoryPathInfo.Error}");

        return Result.Success(new LibraryFileSubDirectoryPath(directoryPathInfo.Value));
    }

    /// <summary>
    /// Erstellt ein LibraryFileSubDirectoryPath-Objekt.
    /// </summary>
    /// <param name="directoryPath"></param>
    /// <returns></returns>
    public static Result<LibraryFileSubDirectoryPath> Create(DirectoryPathInfo directoryPath)
    {
        return Result.Success(new LibraryFileSubDirectoryPath(directoryPath));
    }
}