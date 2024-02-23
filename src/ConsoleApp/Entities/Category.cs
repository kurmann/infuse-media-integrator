using CSharpFunctionalExtensions;

namespace Kurmann.InfuseMediaIntegrator.Entities;

public class Category
{
    private Category(string name) => Name = name;

    public string Name { get; }
    public IReadOnlyList<Category> SubCategories { get; } = [];

    public static Result<Category> Create(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            return Result.Failure<Category>("Name is null or empty");

        return new Category(name);
    }
}

public class CategoryComparer : IEqualityComparer<Category>
{
    public bool Equals(Category x, Category y)
    {
        if (ReferenceEquals(x, y)) return true;
        if (ReferenceEquals(x, null) || ReferenceEquals(y, null))
            return false;

        // Überprüfe, ob die Namen gleich sind
        bool areNamesEqual = x.Name == y.Name;
        
        // Überprüfe, ob die Anzahl der Unterkategorien gleich ist
        bool areSubCategoryCountsEqual = x.SubCategories.Count == y.SubCategories.Count;

        // Wenn die Namen oder die Anzahlen der Unterkategorien nicht übereinstimmen, sind die Kategorien nicht gleich
        if (!areNamesEqual || !areSubCategoryCountsEqual) return false;

        // Überprüfe, ob alle Unterkategorien in der gleichen Reihenfolge gleich sind
        for (int i = 0; i < x.SubCategories.Count; i++)
        {
            // Verwende rekursiv den gleichen Vergleich für Unterkategorien, falls erforderlich
            if (!Equals(x.SubCategories[i], y.SubCategories[i]))
                return false;
        }

        // Alle Prüfungen bestanden, die Kategorien sind gleich
        return true;
    }

    public int GetHashCode(Category obj)
    {
        if (ReferenceEquals(obj, null)) return 0;

        unchecked // Ermöglicht eine Überlauf-ignorierende Addition für den Hashcode
        {
            int hash = 17;
            hash = hash * 23 + obj.Name.GetHashCode();
            
            // Berechne den Hashcode basierend auf den Unterkategorien
            foreach (var subCategory in obj.SubCategories)
            {
                hash = hash * 23 + (subCategory != null ? subCategory.GetHashCode() : 0);
            }

            return hash;
        }
    }
}
