using CSharpFunctionalExtensions;

namespace Kurmann.InfuseMediaIntegrator.Entities.Elementary;

/// <summary>
/// Repräsentiert Informationen über einen Verzeichnisnamen, ohne direkt vom Dateisystem abhängig zu sein. Diese Klasse ist unveränderlich.
/// Unterscheidet sich von DirectoryPathInfo, da es keine Informationen über den Verzeichnispfad enthält.
/// </summary>
public class DirectoryNameInfo
{
    /// <summary>
    /// Der Verzeichnisname.
    /// </summary>
    public string DirectoryName { get; }

    private DirectoryNameInfo(string directoryName) => DirectoryName = directoryName;

    /// <summary>
    /// Erstellt ein DirectoryNameInfo-Objekt.
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public static Result<DirectoryNameInfo> Create(string? name)
    {
        // Gib eine Fehlermeldung zurück, wenn der Name null oder leer ist
        if (string.IsNullOrWhiteSpace(name))
            return Result.Failure<DirectoryNameInfo>("Name is null or empty");

        // Prüfe, ob es nicht ein Dateipfad ist
        if (Path.HasExtension(name))
            return Result.Failure<DirectoryNameInfo>("Name is a file path");

        // Prüfe, ob es nicht ein Verzeichnispfad ist
        if (Path.IsPathRooted(name))
            return Result.Failure<DirectoryNameInfo>("Name is a directory path");

        // Prüfe auf unzulässige Zeichen im Pfad
        char[] invalidPathChars = Path.GetInvalidPathChars();
        if (name.Any(c => invalidPathChars.Contains(c)))
            return Result.Failure<DirectoryNameInfo>("Name contains invalid characters: " + string.Join(", ", invalidPathChars));

        return Result.Success(new DirectoryNameInfo(name));
    }
}