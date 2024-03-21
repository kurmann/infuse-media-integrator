namespace Kurmann.InfuseMediaIntegrator.Queries;

public class SearchDirectoriesQuery
{
    /// <summary>
    /// Diese Klasse bietet eine Methode zur rekursiven Suche nach Dateien in einem Verzeichnisbaum.
    /// Die Suche basiert auf einem Startverzeichnis und einem Text, mit dem die Dateinamen beginnen sollen.
    /// Wir verwenden die Methode `EnumerateDirectories` aus dem `System.IO`-Namespace, um durch die Verzeichnisse zu iterieren.
    /// </summary>
    /// <remarks>
    /// `EnumerateDirectories` wird gegenüber `GetDirectories` bevorzugt, da es eine effizientere Art der Iteration bietet.
    /// Statt alle Verzeichnispfade sofort in den Speicher zu laden, liefert `EnumerateDirectories` einen Enumerator,
    /// der die Verzeichnisse nach und nach durchläuft. Dies ist besonders nützlich für die Arbeit mit großen Dateisystemen,
    /// da es den Speicherverbrauch reduziert und die Performance verbessert, indem es die Verzeichnisse verzögert lädt.
    /// So kann die Anwendung mit Verzeichnisstrukturen arbeiten, ohne dass der Speicherbedarf stark ansteigt oder die Anwendung verlangsamt wird.
    /// </remarks>
    /// <exception cref="ArgumentNullException">Das Startverzeichnis ist `null` oder leer.</exception>
    /// <exception cref="ArgumentException">Das Startverzeichnis ist nicht vorhanden oder nicht zugänglich.</exception>
    /// <exception cref="UnauthorizedAccessException">Der Zugriff auf das Startverzeichnis wurde verweigert.</exception>
    /// <exception cref="DirectoryNotFoundException">Das Startverzeichnis wurde nicht gefunden.</exception>
    public static IEnumerable<string> SearchFiles(string startDirectory, string searchText, bool searchAtBeginningOnly = false)
    {
        // Überprüfe, ob das Startverzeichnis vorhanden und zugänglich ist.
        Queue<string> directories = new();
        directories.Enqueue(startDirectory);

        // Durchsuche das Verzeichnis rekursiv nach Dateien, die mit dem angegebenen Text beginnen.
        while (directories.Count > 0)
        {
            // Entferne das erste Verzeichnis aus der Warteschlange.
            string currentDirectory = directories.Dequeue();

            // Lege fest, ob die Datei mit dem angegebenen Text beginnen soll.
            string searchPattern = searchAtBeginningOnly ? $"{searchText}*" : $"*{searchText}*";

            // Suche nach Dateien, die mit dem angegebenen Text beginnen
            // Hinweis: `EnumerateFiles` gibt den vollständigen Pfad der Datei zurück.
            // Hinweis: TopDirectoryOnly gibt an, dass nur das aktuelle Verzeichnis durchsucht wird, weil wir die rekursive Suche selbst steuern.
            foreach (var file in Directory.EnumerateFiles(currentDirectory, searchPattern, SearchOption.TopDirectoryOnly))
            {
                yield return file;
            }

            // Füge alle Unterverzeichnisse in die Warteschlange ein, um sie später zu durchsuchen.
            foreach (var subdir in Directory.EnumerateDirectories(currentDirectory))
            {
                directories.Enqueue(subdir);
            }
        }
    }

    /// <summary>
    /// Diese Methode bietet eine Möglichkeit, nach mehreren Texten in Dateinamen zu suchen, die alle gefunden werden müssen.
    /// </summary>
    /// <param name="startDirectory"></param>
    /// <param name="andSearchText"></param>
    /// <param name="searchAtBeginningOnly"></param>
    /// <returns></returns>
    public static IEnumerable<string> SearchFiles(string startDirectory, IEnumerable<string> andSearchText)
    {
        // Überprüfe, ob das Startverzeichnis vorhanden und zugänglich ist.
        Queue<string> directories = new();
        directories.Enqueue(startDirectory);

        // Durchsuche das Verzeichnis rekursiv nach Dateien, die mit dem angegebenen Text beginnen.
        while (directories.Count > 0)
        {
            // Entferne das erste Verzeichnis aus der Warteschlange.
            string currentDirectory = directories.Dequeue();

            // Suche nach Dateien, die mit dem angegebenen Text beginnen
            // Hinweis: `EnumerateFiles` gibt den vollständigen Pfad der Datei zurück.
            // Hinweis: TopDirectoryOnly gibt an, dass nur das aktuelle Verzeichnis durchsucht wird, weil wir die rekursive Suche selbst steuern.
            foreach (var file in Directory.EnumerateFiles(currentDirectory, "*.*", SearchOption.TopDirectoryOnly))
            {
                // Prüfe, ob alle Texte in der Datei enthalten sind.
                if (andSearchText.All(file.Contains))
                {
                    yield return file;
                }
            }

            // Füge alle Unterverzeichnisse in die Warteschlange ein, um sie später zu durchsuchen.
            foreach (var subdir in Directory.EnumerateDirectories(currentDirectory))
            {
                directories.Enqueue(subdir);
            }
        }
    }
}