# Infuse Media Integrator

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

## Grundprinzipien der Modularchitektur

Unsere Modularchitektur orientiert sich an den folgenden Kernprinzipien, um Wartbarkeit, Skalierbarkeit und Flexibilität unserer Software zu gewährleisten:

1. **Hohe Kohäsion:** Jedes Modul ist um eine klare und eng definierte Funktionalität oder Domäne herum organisiert. Dies stellt sicher, dass alle Teile eines Moduls eng miteinander verbunden sind und gemeinsam zu einem spezifischen Zweck beitragen.

2. **Lose Kopplung:** Module kommunizieren über definierte Schnittstellen oder Ereignisse miteinander, wodurch Abhängigkeiten zwischen ihnen minimiert werden. Diese Trennung ermöglicht es, Module unabhängig voneinander zu entwickeln, zu testen und zu aktualisieren.

3. **Unveränderlichkeit und Validität von Entitäten:** Entitäten innerhalb eines Moduls sind unveränderlich und werden stets in einem validen Zustand gehalten. Dies wird durch die Verwendung von Factory-Methoden und Ergebnistypen sichergestellt, die es ermöglichen, ungültige Zustände und Exceptions zu vermeiden.

4. **Domain-Driven Design (DDD):** Unsere Module folgen den Prinzipien des Domain-Driven Designs, indem sie ein reichhaltiges Domänenmodell aufbauen, das die Geschäftslogik und -regeln direkt in den Entitäten und Werten kapselt.

5. **Event-Driven Approach:** Die Module verwenden ein Event- und Delegate-Muster für die Kommunikation. Dies ermöglicht eine effektive Reaktion auf Zustandsänderungen und Ereignisse innerhalb und zwischen Modulen, ohne eine direkte Kopplung zu erfordern.

6. **SOLID-Prinzipien:** Unsere Architektur hält sich an die SOLID-Prinzipien (Single Responsibility, Open/Closed, Liskov Substitution, Interface Segregation, Dependency Inversion), um eine hohe Qualität und Erweiterbarkeit des Codes zu gewährleisten.

7. **Einfachheit und Klarheit:** Die Architektur strebt danach, so einfach wie möglich zu sein, ohne dabei Flexibilität und Funktionalität zu opfern. Überflüssige Komplexität wird vermieden, indem Features und Architekturelemente nur dann eingeführt werden, wenn sie einen klaren Nutzen bieten.

8. **Modulare Monolithen:** Wir streben eine Balance zwischen den Vorteilen modularer Monolithen und der Flexibilität von Mikroservice-Architekturen an. Dies erlaubt es uns, die Vorteile beider Architekturansätze zu nutzen, während wir ihre Nachteile minimieren.

### Zielsetzung der Grundprinzipien

Das Ziel unserer Architektur ist es, eine solide Basis für unsere Anwendung zu schaffen, die es uns ermöglicht, schnell auf Geschäftsanforderungen zu reagieren, dabei aber eine hohe Codequalität und Systemzuverlässigkeit zu gewährleisten. Durch die Einhaltung dieser Prinzipien schaffen wir eine robuste und flexible Plattform, die Wachstum und Veränderung unterstützt, ohne dabei Kompromisse bei der Wartbarkeit oder Erweiterbarkeit einzugehen.

Natürlich! Hier ist eine zusammengefasste Darstellung der Umsetzung unserer Modularchitektur, die Sie für Ihr GitHub README-File nutzen können:

## Umsetzung der Modularchitektur

Unsere Modularchitektur baut auf den definierten Grundprinzipien auf und implementiert diese durch eine gezielte Auswahl von Architekturelementen und -mustern:

1. **Domain Services:** Anstelle eines umfangreichen Common-Layers, der für alle Module zugänglich ist, setzen wir auf dedizierte Domain Services innerhalb jedes Moduls. Diese Services kapseln die Geschäftslogik und stellen sicher, dass unsere Entitäten und Domänenmodelle reich an Verhalten sind und direkt die Geschäftsregeln durchsetzen. Sie ermöglichen es uns, Geschäftsoperationen klar zu definieren und auszuführen, ohne dass Entitäten in einen ungültigen Zustand geraten.

2. **Factory-Muster für Entitätserstellung:** Die Verwendung des Factory-Musters gewährleistet, dass alle Entitäten von Beginn an in einem validen Zustand sind. Durch die Konstruktion von Entitäten über Factory-Methoden vermeiden wir ungültige Zustände und fördern die Unveränderlichkeit der Objekte. Dies unterstützt die Konsistenz und Zuverlässigkeit unserer Geschäftslogik.

3. **Event- und Delegate-Muster für die Nachrichtenverarbeitung:**
    - **Nachrichtenannahme:** Module können Nachrichten über ein einfaches, aber effektives In-Memory-Nachrichtensystem empfangen. Dieses System nutzt das Event- und Delegate-Muster, um lose gekoppelte Kommunikation zwischen verschiedenen Teilen der Anwendung zu ermöglichen.
    - **Nachrichtenausgabe:** Wenn ein Modul eine Operation abgeschlossen hat, wird ein Event ausgelöst, um andere Teile der Anwendung über das Ergebnis zu informieren. Dieses Event kann von anderen Modulen oder der Host-Anwendung abonniert werden, um auf die Nachricht zu reagieren und entsprechende Folgeaktionen einzuleiten.
    - **Verarbeitungslogik:** Die Logik zur Verarbeitung von Nachrichten ist direkt in den Domain Services oder den Entitäten integriert. Dadurch wird die Geschäftslogik zentralisiert und die Notwendigkeit separater Handler für verschiedene Aktionen reduziert.

4. **Ergebnistypen für Operationen:** Um eine klare Kommunikation von Erfolg oder Misserfolg von Operationen zu ermöglichen, verwenden wir Ergebnistypen (z.B. `Result<T>`). Dies ermöglicht eine explizite Handhabung von Fehlern und Ausnahmesituationen, ohne auf Exceptions angewiesen zu sein.

5. **Direkte Kommunikation ohne zusätzliche Commons oder Handler:** Um die Architektur zu vereinfachen und redundante Schichten zu vermeiden, verzichten wir auf separate Commons-Schichten oder zusätzliche Handler für die Nachrichtenverarbeitung. Stattdessen erfolgt die Kommunikation direkt über die definierten Events und Delegates, wobei die Geschäftslogik in den Domain Services und Entitäten gekapselt bleibt.

### Ziel der Umsetzung

Das Ziel dieser Umsetzung ist es, eine klare, verständliche und effiziente Architektur zu schaffen, die es ermöglicht, schnell und flexibel auf Änderungen zu reagieren, ohne dabei die Qualität oder Konsistenz der Anwendung zu beeinträchtigen. Durch die Fokussierung auf starke Domänenmodelle und die direkte Kommunikation zwischen Modulen fördern wir eine Architektur, die sowohl robust als auch anpassungsfähig ist, und unterstützen ein nachhaltiges Wachstum unserer Software.

Natürlich, hier ist eine Erweiterung der Umsetzungszusammenfassung, die die Integration von Modulen in die Host-Anwendung über die `IServiceCollection` einschließt:

## Integration von Modulen in die Host-Anwendung

Die Integration der einzelnen Module in die zentrale Host-Anwendung ist ein entscheidender Schritt, um die modulare Architektur effektiv zu nutzen. Hierfür verwenden wir die `IServiceCollection` von .NET Core/ASP.NET Core, die uns eine flexible und mächtige Möglichkeit bietet, Abhängigkeiten zu verwalten und zu injizieren. Die Integration erfolgt in zwei Hauptphasen:

1. **Registrierung der Modul-Services:**
   Jedes Modul bietet eine Erweiterungsmethode für die `IServiceCollection`, die alle für das Modul spezifischen Services, einschließlich Domain Services, Factories und eventuelle Infrastrukturservices, registriert. Dieser Ansatz ermöglicht es, dass Module ihre eigenen Abhängigkeiten definieren und sich selbstständig in die Host-Anwendung integrieren können, ohne dass die Host-Anwendung detailliertes Wissen über die inneren Abhängigkeiten jedes Moduls benötigt.

   ```csharp
   public static class ServiceCollectionExtensions
   {
       public static IServiceCollection AddVideoProcessingModule(this IServiceCollection services)
       {
           services.AddSingleton<IVideoService, VideoService>();
           // Registriere weitere spezifische Services oder Handlers, die das Modul benötigt
           return services;
       }
   }
   ```

2. **Konfiguration in der Host-Anwendung:**
   In der Startup-Konfiguration der Host-Anwendung werden diese Erweiterungsmethoden aufgerufen, um jedes Modul zu integrieren. Dies erfolgt in der `ConfigureServices`-Methode der `Startup`-Klasse oder dem Setup-Teil einer minimalen API. Diese Methode ruft die Erweiterungsmethoden der Module auf, welche wiederum ihre Services zur `IServiceCollection` hinzufügen.

   ```csharp
   public void ConfigureServices(IServiceCollection services)
   {
       services.AddVideoProcessingModule();
       // Integration weiterer Module
   }
   ```

### Dynamische Modulintegration

Diese Struktur bietet eine klare und kohärente Methode, um Module dynamisch in eine größere Anwendungslandschaft zu integrieren. Sie unterstützt eine starke Isolation zwischen den Modulen, fördert die Wiederverwendbarkeit von Code und ermöglicht eine flexible Erweiterung der Host-Anwendung durch das Hinzufügen neuer Module.

### Ziel der Integration

Das Hauptziel dieser Integrationsmethode ist es, eine konsistente, wartbare und erweiterbare Basis für die Entwicklung von Software zu bieten, die aus mehreren Modulen besteht. Durch die Verwendung der `IServiceCollection` zur Registrierung von Modulabhängigkeiten halten wir unsere Anwendung flexibel und offen für Erweiterungen, während wir gleichzeitig die Vorteile der Dependency Injection und der losen Kopplung voll ausschöpfen.

## Testdaten

Für Unittesting mit Videodateien stehen Testdaten bereit: [Testdateien](docs/Tests.md)

## Dokumentation
- [Dateistruktur Infuse](/docs/Dateistruktur.md)

### Recherche
- [Nachrichtensystem](/docs/research/Nachrichtensystem.md)
- [Konfiguration](/docs/research/Konfiguration.md)
- [Hosting](/docs/research/Hosting.md)
