# Infuse Media Integrator

## Archivierung

### Dieses Repository wurde in das Monorepo "kurmann/videoschnitt" migriert

**Hinweis:** Dieses Repository wird nicht mehr aktiv weiterentwickelt und ist archiviert. Die Entwicklung wird nun im Monorepo [kurmann/videoschnitt](https://github.com/kurmann/videoschnitt) fortgeführt.

---

## Anforderungen und Prozess für das Verschieben von Dateien in die Mediathek

### Eingangsverzeichnis und Kategorisierung

- **Eingangsverzeichnis**: Das Hauptverzeichnis für neue Mediendateien wird über die Konfiguration festgelegt. Dieses Verzeichnis wird vom Modul kontinuierlich auf neue Dateien hin überwacht.

- **Kategorien**: Unterverzeichnisse innerhalb des Eingangsverzeichnisses repräsentieren Kategorien, die als erste Ebene der Organisation und Klassifizierung von Mediendateien dienen, bevor sie in die Mediathek verschoben werden.

- **Unterverzeichnisse**: Das Modul unterstützt auch die Erstellung und das Durchsuchen von Unterverzeichnissen von Unterverzeichnissen, um eine detaillierte und flexible Kategorisierung zu ermöglichen.

### Verschiebungsprozess

- **Durchsuchen des Eingangsverzeichnisses**: Das Modul durchsucht das gesamte konfigurierte Eingangsverzeichnis, einschließlich aller Unterverzeichnisse, um neue Dateien zu identifizieren.

- **Kategorienzuweisung**: Das relative Unterverzeichnis, in dem eine Datei gefunden wird, bestimmt die Kategorie, der die Datei in der Mediathek zugeordnet wird.

- **Erkennung und Zuordnung zu Mediengruppen**:
    - Das Modul identifiziert vorhandene Mediengruppen innerhalb der entsprechenden Kategorie in der Mediathek oder erstellt eine neue Mediengruppe, falls erforderlich, basierend auf der eindeutigen ID (`{ISO-Datum} {Titel}`).
    - Die Datei wird dann entsprechend in die existierende oder neu erstellte Mediengruppe verschoben.

### Erweiterte Organisationsstruktur

- **Flexibilität durch Unterverzeichnisse**: Die Unterstützung für Unterverzeichnisse ermöglicht eine detaillierte und flexible Organisation der Dateien im Eingangsverzeichnis.

- **Strukturierte Mediathek**: Die Mediathek nutzt die Kategorisierung und Mediengruppenstruktur, um eine intuitive und zugängliche Ablage der Mediendateien zu gewährleisten.

Natürlich, hier ist eine Zusammenfassung der Grundprinzipien für die Architektur eines Moduls, die Sie für Ihr GitHub README-File verwenden können:

## Weitere Infos

- [Testdateien](docs/Tests.md)
- [Dateistruktur Infuse](/docs/Dateistruktur.md)
- [Recherche](https://github.com/kurmann/infuse-media-integrator/tree/main/docs/research)
