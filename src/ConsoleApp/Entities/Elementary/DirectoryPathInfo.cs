using CSharpFunctionalExtensions;

namespace Kurmann.InfuseMediaIntegrator.Entities.Elementary
{
    /// <summary>
    /// Repräsentiert Informationen über einen Verzeichnispfad, ohne direkt vom Dateisystem abhängig zu sein. Diese Klasse ist unveränderlich.
    /// Unterscheidet sich von FilePathInfo, da es keine Informationen über den Dateinamen enthält.
    /// </summary>
    public class DirectoryPathInfo
    {
        /// <summary>
        /// Der vollständige Pfad.
        /// </summary>
        public string DirectoryPath { get; }

        private DirectoryPathInfo(string directoryPath) => DirectoryPath = directoryPath;

        /// <summary>
        /// Liste der Verzeichnisse. Die oberste Kategorie ist an erster Stelle, gefolgt von den Unterkategorien.
        /// Entfernt leere Verzeichnisse und Leerzeichen.
        /// </summary>
        public List<DirectoryNameInfo> Directories => DirectoryPath
            .Split(Path.DirectorySeparatorChar)
            .Select(d => DirectoryNameInfo.Create(d).Value)
            .Where(d => !string.IsNullOrWhiteSpace(d.DirectoryName))
            .ToList();

        /// <summary>
        /// Erstellt eine Instanz von DirectoryPathInfo, wenn der gegebene Pfad gültig ist.
        /// </summary>
        /// <param name="path">Der Pfad, der validiert und gespeichert werden soll.</param>
        /// <returns>Ein Result-Objekt, das entweder eine DirectoryPathInfo-Instanz oder einen Fehler enthält.</returns>
        public static Result<DirectoryPathInfo> Create(string? path)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                return Result.Failure<DirectoryPathInfo>("Path is null or empty");
            }

            // Entferne die Datei im Pfad, falls vorhanden
            path = System.IO.Path.GetDirectoryName(path);

            // Gib eine Fehlermeldung zurück, wenn der Pfad kein Verzeichnis ist
            if (string.IsNullOrWhiteSpace(path))
            {
                return Result.Failure<DirectoryPathInfo>("Path is not a directory");
            }

            // Prüfe auf unzulässige Zeichen im Pfad
            char[] invalidPathChars = System.IO.Path.GetInvalidPathChars();
            if (path.Any(c => invalidPathChars.Contains(c)))
            {
                return Result.Failure<DirectoryPathInfo>("Path contains invalid characters: " + string.Join(", ", invalidPathChars));
            }

            return Result.Success(new DirectoryPathInfo(path));
        }
    }
}