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
    private readonly List<string> _categories = [];

    private CategoryPath(List<string> categories) => _categories = categories;

    /// <summary>
    /// Liste der Kateogrien. Die oberste Kategorie ist an erster Stelle, gefolgt von den Unterkategorien.
    /// </summary>
    public IReadOnlyList<string> Categories => _categories;

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
        _categories.Add(category);
        return Result.Success();
    }

    public override string ToString() => string.Join("/", _categories);
}
