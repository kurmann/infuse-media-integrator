using CSharpFunctionalExtensions;

namespace Kurmann.InfuseMediaIntegrator.Entities;

/// <summary>
/// Repräsentiert einen Kategorienpfad.
/// Entspricht einer Verzeichnisstruktur, die durch Schrägstriche getrennt ist
/// und gleichzeitig Kategorien und Unterkategorien darstellt.
/// Der Kategorienpfad selbst hat eine gültige Verzeichnisstruktur.
/// </summary>
public class CategoryPath
{
    private readonly List<string> categories = [];

    private CategoryPath(List<string> categories) => this.categories = categories;

    /// <summary>
    /// Liste der Kateogrien. Die oberste Kategorie ist an erster Stelle, gefolgt von den Unterkategorien.
    /// </summary>
    public IReadOnlyList<string> Categories => categories;

    /// <summary>
    /// Pfad der Kategorien. Repräsentiert die Verzeichnisstruktur und setzt die Kategorien durch Schrägstriche getrennt.
    /// </summary>
    public string Value => ToString();

    /// <summary>
    /// Erstellt eine neue Instanz der Klasse <see cref="CategoryPath"/> aus einem Pfad.
    /// Prüft auf ungültige Zeichen im Sinne von <see cref="Path.GetInvalidPathChars"/>.
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    public static Result<CategoryPath> Create(string? path)
    {
        // Wenn der Pfad leer ist, dann ist das ein Fehler
        if (string.IsNullOrWhiteSpace(path))
            return Result.Failure<CategoryPath>("Path is null or empty");

        // Prüfe auf ungültige Zeichen und gib diese in der Fehlermeldung zurück
        if (path.Any(c => Path.GetInvalidPathChars().Contains(c)))
            return Result.Failure<CategoryPath>($"Path contains invalid characters: {string.Join(", ", Path.GetInvalidPathChars())}");

        // Entferne Leerzeichen und trenne die Kategorien an den Schrägstrichen und entferne leere Kategorien
        var categories = path.Split('/').Select(c => c.Trim()).Where(c => !string.IsNullOrEmpty(c)).ToList();

        return new CategoryPath(categories);
    }

    /// <summary>
    /// Erstellt eine neue Instanz der Klasse <see cref="CategoryInfo"/> aus einem Wurzelverzeichnis und einem Verzeichnis.
    /// Da eine Datei nur einer Verzeichnishierarchie zugehören kann, kann nur mit einer Verzeichnisstruktur nur
    /// eine Kategorie ausgelesen werden. Etwaige Unterverzeichnisse werden als Unterkategorien interpretiert.
    /// Beispiel: "Data/Input/Testcase 3/Lyssach/Garten" wird zu "Lyssach/Garten" wenn "Data/Input/Testcase 3" als Wurzelverzeichnis verwendet wird.
    /// </summary>
    /// <param name="rootPath"></param>
    /// <param name="directoryPath"></param>
    /// <returns></returns>
    public static Result<CategoryPath?> Create(string? rootPath, string? directoryPath)
    {
        if (string.IsNullOrWhiteSpace(rootPath))
            return Result.Failure<CategoryPath?>("RootPath is null or empty");
        if (string.IsNullOrWhiteSpace(directoryPath))
            return Result.Failure<CategoryPath?>("DirectoryPath is null or empty");

        // Entferne Trennzeichen am Anfang und am Ende.
        rootPath = rootPath.Trim(Path.DirectorySeparatorChar);
        directoryPath = directoryPath.Trim(Path.DirectorySeparatorChar);

        var root = new DirectoryInfo(rootPath);
        if (!root.Exists)
            return Result.Failure<CategoryPath?>($"RootPath '{rootPath}' does not exist");

        var directory = new DirectoryInfo(directoryPath);
        if (!directory.Exists)
            return Result.Failure<CategoryPath?>($"DirectoryPath '{directoryPath}' does not exist");

        // Ermittle das relative Verzeichnis zum Wurzelverzeichnis
        var relativeDirectory = directory.FullName[root.FullName.Length..].Trim(Path.DirectorySeparatorChar);

        // Wenn das Wurzelverzeichnis und das Verzeichnis das gleiche sind, dann ist die Kategorie leer
        if (root.FullName == directory.FullName)
            return null;
        
        // Das relative Verzeichnis wird als Kategorie interpretiert (mit Unterkategorien getrennt durch Schrägstriche)
        var categoryPath = CategoryPath.Create(relativeDirectory);
        if (categoryPath.IsFailure)
            return Result.Failure<CategoryPath?>(categoryPath.Error);

        return categoryPath.Value;
    }

    /// <summary>
    /// Fügt eine Kategorie hinzu. Ist die Kategorie leer, dann wird sie ignoriert.
    /// Entfernt Leerzeichen zu Beginn und am Ende.
    /// </summary>
    /// <param name="category"></param>
    public Result AddCategory(string? category)
    {
        if (string.IsNullOrEmpty(category))
        {
            return Result.Failure("Category is null or empty");
        }

        // Entferne Leerzeichen zu Beginn und am Ende
        category = category.Trim();

        // Prüfe auf ungültige Zeichen und gib diese in der Fehlermeldung zurück
        if (category.Contains(Path.GetInvalidPathChars()[0]))
            return Result.Failure<CategoryPath>($"Category contains invalid characters: {string.Join(", ", Path.GetInvalidPathChars()[0])}");

        // Füge die Kategorie hinzu
        categories.Add(category);
        return Result.Success();
    }

    public override string ToString() => string.Join("/", categories);
}
