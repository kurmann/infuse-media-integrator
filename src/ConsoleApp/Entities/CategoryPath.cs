using CSharpFunctionalExtensions;

namespace Kurmann.InfuseMediaIntegrator.Entities;

/// <summary>
/// Repräsentiert einen Kategorienpfad.
/// Entspricht einer Verzeichnisstruktur, die durch Schrägstriche getrennt ist
/// und gleichzeitig Kategorien und Unterkategorien darstellt.
/// </summary>
public class CategoryPath
{
    private readonly List<string> _categories = [];

    private CategoryPath(List<string> categories) => _categories = categories;

    public static Result<CategoryPath> Create(string path)
    {
        // Wenn der Pfad leer ist, dann ist das ein Fehler
        if (string.IsNullOrWhiteSpace(path))
            return Result.Failure<CategoryPath>("Path is null or empty");

        // Entferne Leerzeichen und trenne die Kategorien an den Schrägstrichen und entferne leere Kategorien
        var categories = path.Split('/').Select(c => c.Trim()).Where(c => !string.IsNullOrEmpty(c)).ToList();

        return new CategoryPath(categories);
    }

    /// <summary>
    /// Fügt eine Kategorie hinzu. Ist die Kategorie leer, dann wird sie ignoriert.
    /// Entfernt Leerzeichen zu Beginn und am Ende.
    /// </summary>
    /// <param name="category"></param>
    public void AddCategory(string category)
    {
        if (!string.IsNullOrEmpty(category))
        {
            // Entferne Leerzeichen zu Beginn und am Ende
            category = category.Trim();

            _categories.Add(category);
        }
    }

    public override string ToString() => string.Join("/", _categories);
}
