using CSharpFunctionalExtensions;

namespace Kurmann.InfuseMediaIntegrator.Entities;

/// <summary>
/// Repräsentiert eine Kategorie-Information.
/// Besteht aus unterscheidbaren Hauptkategorien, die jeweils Unterkategorien haben können.
/// </summary>
public class CategoryInfo
{
    private CategoryInfo(HashSet<string> categories) => Categories = categories;
    private CategoryInfo(string? category) => Categories = category != null ? [category] : [];
    private CategoryInfo() => Categories = [];

    /// <summary>
    /// Liste der Kategorien. Eine Kategorie kann auch Unterkategorien haben, die durch einen Schrägstrich getrennt sind.
    /// Beispiel: "Lyssach/Garten"
    /// In diesem Fall ist "Lyssach" die Hauptkategorie und "Garten" die Unterkategorie.
    /// Dieser Ansatz wird verwendet aufgrund der Einfachheit und der Tatsache, dass die Kategorien eine Verzeichnisstruktur nachahmen.
    /// </summary>
    public HashSet<string> Categories { get; }

    /// <summary>
    /// Erstellt eine neue Instanz der Klasse <see cref="CategoryInfo"/> aus einem Wurzelverzeichnis und einem Verzeichnis.
    /// Beispiel: RootPath = "Data/Input/Testcase 3", DirectoryPath = "Data/Input/Testcase 3/Lyssach/Garten"
    /// Hinweis: Da eine Datei nur einer Verzeichnishierarchie zugehören kann, kann nur mit einer Verzeichnisstruktur nur
    /// eine Kategorie ausgelesen werden. Etwaige Unterverzeichnisse werden als Unterkategorien interpretiert.
    /// Beispiel: "Data/Input/Testcase 3/Lyssach/Garten" wird zu "Lyssach/Garten" wenn "Data/Input/Testcase 3" als Wurzelverzeichnis verwendet wird.
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

        // Ermittle das relative Verzeichnis zum Wurzelverzeichnis
        var relativeDirectory = directory.FullName.Substring(root.FullName.Length).Trim(Path.DirectorySeparatorChar);
        
        // Das relative Verzeichnis wird als Kategorie interpretiert (mit Unterkategorien getrennt durch Schrägstriche)
        var category = relativeDirectory;

        // Wenn das Wurzelverzeichnis und das Verzeichnis das gleiche sind, dann ist die Kategorie leer
        if (root.FullName == directory.FullName)
            return new CategoryInfo();

        return new CategoryInfo(category);
    }

    // Erstellt eine neue Instanz der Klasse <see cref="CategoryInfo"/> aus einer Liste von Komma-separierten Kategorien.
    public static Result<CategoryInfo> CreateFromCommaSeparatedList(string? categories)
    {
        if (string.IsNullOrWhiteSpace(categories))
            return Result.Failure<CategoryInfo>("Categories is null or empty");

        // Entferne Leerzeichen und trenne die Kategorien an den Kommas
        var categoriesList = categories.Split(',').Select(c => c.Trim()).ToHashSet();

        return Result.Success(new CategoryInfo(categoriesList));
    }

    public override string ToString() => string.Join(", ", Categories);

    public static implicit operator string(CategoryInfo categoryInfo) => categoryInfo.ToString();

    public Result AddCategory(string category)
    {
        if (string.IsNullOrWhiteSpace(category))
            return Result.Failure("Category is null or empty");

        // Entferne die Leerzeichen und trenne sie anhand Kommas
        var newCategories = category.Split(',').Select(c => c.Trim()).ToList();

        Categories.UnionWith(newCategories);
        return Result.Success();
    }
}