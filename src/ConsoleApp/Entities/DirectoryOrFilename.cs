using CSharpFunctionalExtensions;

namespace Kurmann.InfuseMediaIntegrator.Entities;

/// <summary>
/// Definiert den Namen einer Datei oder eines Verzeichnisses ohne Pfad. Prüft ob der Name für ein Verzeichnis gültig ist.
/// </summary>
[Obsolete("Use PathInfo instead.")]
public class DirectoryOrFilename
{
    /// <summary>
    /// Der Name des Verzeichnisses.
    /// </summary>
    /// <value></value>
    public string Name { get; }

    private DirectoryOrFilename(string name) => Name = name;

    /// <summary>
    /// Erstellt eine neue Instanz der Klasse <see cref="DirectoryOrFilename"/>.
    /// </summary>
    /// <param name="name">Der Name des Verzeichnisses.</param>
    /// <returns></returns>
    public static Result<DirectoryOrFilename> Create(string? name)
    {
        // Prüfe ob der Name angegeben ist
        if (string.IsNullOrWhiteSpace(name))
        {
            return Result.Failure<DirectoryOrFilename>("Directory name cannot be null or empty.");
        }
        var trimmedName = name.Trim();

        // Prüfe ob der Name gültig ist
        var invalidChars = Path.GetInvalidFileNameChars();
        if (trimmedName.Any(c => invalidChars.Contains(c)))
        {
            return Result.Failure<DirectoryOrFilename>($"Directory name '{trimmedName}' contains invalid characters.");
        }

        return new DirectoryOrFilename(trimmedName);
    }

    public override string ToString() => Name;
}