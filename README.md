# infuse-media-integrator

## Überblick
Der `kurmann/infuse-media-integrator` ist ein Microservice innerhalb der `kurmann/media-library`, der speziell dafür entwickelt wurde, Videodateien nahtlos in eine Infuse-kompatible Mediathek zu integrieren. Dieser Service optimiert die Strukturierung der Medieninhalte, sodass sie direkt mit dem Infuse Player auf Apple TV und anderen Geräten gestreamt werden können.

## Funktionen
- **Automatische Integration**: Verschiebt Videodateien mit bearbeiteten Metadaten in die Infuse-Mediathek, indem er eine spezifische Verzeichnis- und Dateistruktur verwendet, die von Infuse erkannt wird.
- **Volume-Bindung**: Unterstützt das Einbinden von zwei Volumes – eines für die Quelldatei und eines für die Infuse-Mediathek –, um eine direkte und effiziente Dateiverarbeitung innerhalb des Docker-Containers zu ermöglichen.
- **Metadatenbasierte Organisation**: Nutzt Metadaten, um Videodateien korrekt zu kategorisieren und in der Infuse-Mediathek einzuordnen, was eine intuitive Navigation und Wiedergabe ermöglicht.

## Einsatz
Der Service ist so konzipiert, dass er als Teil der `kurmann/media-library`-Architektur fungiert, wobei er die Kompatibilität mit lokalen Dateisystemen und die nahtlose Integration in bestehende Docker-basierte Workflows gewährleistet.

## Ziel
Ziel des `kurmann/infuse-media-integrator` ist es, die Verwaltung und das Teilen von Videomedien zu vereinfachen, indem er eine Brücke zwischen der `kurmann/media-library` und der Infuse-App schafft.
