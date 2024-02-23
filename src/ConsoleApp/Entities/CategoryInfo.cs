using CSharpFunctionalExtensions;

namespace Kurmann.InfuseMediaIntegrator.Entities;

/// <summary>
/// Liest aus einem Wurzelverzeichnis (als Referenz) die Kategorien aus und speichert diese in einer Liste.
/// Das erste Listen-Element ist die Hauptkatetorie (entspricht dem nächsten Verzeichnis unter dem Wurzelverzeichnis).
/// Die weiteren Listen-Elemente sind die Unterkategorien (entspricht den weiteren Verzeichnissen unter dem Wurzelverzeichnis).
/// </summary>
public class CategoryInfo
{
    private CategoryInfo(List<string> categories) => Categories = categories;

    /// <summary>
    /// Liste der Kategorien. Das erste Element ist die Hauptkategorie, die weiteren Elemente sind die Unterkategorien.
    /// </summary>
    public List<string> Categories { get; }

    /// <summary>
    /// Erstellt eine neue Instanz der Klasse <see cref="CategoryInfo"/> aus einem Wurzelverzeichnis und einem Verzeichnis.
    /// Beispiel: RootPath = "Data/Input/Testcase 3", DirectoryPath = "Data/Input/Testcase 3/Lyssach/Garten"
    /// Result: ["Lyssach", "Garten"]
    /// </summary>
    /// <param name="rootPath"></param>
    /// <param name="directoryPath"></param>
    /// <returns></returns>
    public static Result<CategoryInfo> CreateFromDirectoryStructure(string? rootPath, string? directoryPath)
    {
        if (string.IsNullOrWhiteSpace(rootPath))
            return Result.Failure<CategoryInfo>("RootPath is null or empty");
        if (string.IsNullOrWhiteSpace(directoryPath))
            return Result.Failure<CategoryInfo>("DirectoryPath is null or empty");

        // Entferne Trennzeichen am Anfang und am Ende.
        rootPath = rootPath.Trim(Path.DirectorySeparatorChar);
        directoryPath = directoryPath.Trim(Path.DirectorySeparatorChar);

        var root = new DirectoryInfo(rootPath);
        if (!root.Exists)
            return Result.Failure<CategoryInfo>($"RootPath '{rootPath}' does not exist");

        var directory = new DirectoryInfo(directoryPath);
        if (!directory.Exists)
            return Result.Failure<CategoryInfo>($"DirectoryPath '{directoryPath}' does not exist");

        // Wenn das Wurzelverzeichnis und das Verzeichnis das gleiche sind, dann ist die Kategorie leer
        if (root.FullName == directory.FullName)
            return Result.Success(new CategoryInfo([]));

        // Erstelle das relative Verzeichnis, vom Wurzelverzeichnis aus gesehen.
        var relativeDirectory = directory.FullName[(root.FullName.Length + 1)..];

        // Erstelle die Kategorien aus dem relativen Verzeichnis.
        var categories = relativeDirectory.Split(Path.DirectorySeparatorChar).ToList();

        return Result.Success(new CategoryInfo(categories));
    }

    // Erstellt eine neue Instanz der Klasse <see cref="CategoryInfo"/> aus einer Liste von Komma-separierten Kategorien.
    public static Result<CategoryInfo> CreateFromCommaSeparatedList(string? categories)
    {
        if (string.IsNullOrWhiteSpace(categories))
            return Result.Failure<CategoryInfo>("Categories is null or empty");

        var categoriesList = categories.Split(',').Select(c => c.Trim()).ToList();
        return Result.Success(new CategoryInfo(categoriesList));
    }
}

/// <summary>
/// Repräsentiert eine Kategorie.
/// </summary>
public class Category
{
    /// <summary>
    /// Name der Kategorie.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Liste der Unterkategorien.
    /// </summary>
    public IReadOnlyList<Category> Subcategories { get; }

    private Category(string name, List<Category> subcategories)
    {
        Name = name;
        Subcategories = subcategories.AsReadOnly();
    }

    public static Result<Category> Create(string name, List<Category>? subcategories = null)
    {
        subcategories ??= [];

        if (string.IsNullOrWhiteSpace(name))
        {
            return Result.Failure<Category>("Category name cannot be empty.");
        }

        return Result.Success(new Category(name, subcategories));
    }
}