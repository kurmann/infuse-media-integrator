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
    /// Ermittelt den Pfad des Unterverzeichnisses anhand des Dateipfades und des Wurzelverzeichnisses.
    /// Das Wurzelverzeichnis wird als relativer Pfad betrachtet um den Pfad des Unterverzeichnisses zu ermitteln.
    /// Beispiel: 
    /// Dateipfad: "/Ausgabeverzeichnis/Family/2024/2024-01-01 Neujahrstour.m4v"
    /// Wurzelverzeichnis: "/Ausgabeverzeichnis"
    /// Ergebnis: "/Family/2024"
    /// </summary>
    /// <param name="filePath"></param>
    /// <param name="rootDirectory"></param>
    /// <returns></returns>
    public static Result<LibraryFileSubDirectoryPath> Create(string? filePath, string? rootDirectory)
    {
        // Erstelle ein FilePathInfo-Objekt
        var filePathInfo = FilePathInfo.Create(filePath);
        if (filePathInfo.IsFailure)
            return Result.Failure<LibraryFileSubDirectoryPath>($"Error on reading file info: {filePathInfo.Error}");

        // Erstelle ein DirectoryPathInfo-Objekt
        var directoryPathInfo = DirectoryPathInfo.Create(rootDirectory);
        if (directoryPathInfo.IsFailure)
            return Result.Failure<LibraryFileSubDirectoryPath>($"Error on reading directory info: {directoryPathInfo.Error}");

        // Ermittle den Pfad des Unterverzeichnisses indem der Dateipfad relativ zum Wurzelverzeichnis betrachtet wird
        var relativeDirectoryPath = filePathInfo.Value.FilePath.Replace(directoryPathInfo.Value.DirectoryPath, string.Empty);

        // Erstelle wieder ein DirectoryPathInfo-Objekt
        directoryPathInfo = DirectoryPathInfo.Create(relativeDirectoryPath);
        if (directoryPathInfo.IsFailure)
            return Result.Failure<LibraryFileSubDirectoryPath>($"Error on reading directory info: {directoryPathInfo.Error}");

        return Result.Success(new LibraryFileSubDirectoryPath(directoryPathInfo.Value));
    }

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

    public override string ToString() => DirectoryPath.DirectoryPath;

    public static implicit operator string(LibraryFileSubDirectoryPath libraryFileSubDirectoryPath) => libraryFileSubDirectoryPath.DirectoryPath.DirectoryPath;
}