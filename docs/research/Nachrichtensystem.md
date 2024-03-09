# Interner Nachrichtenaustausch von einem modularen Monolithen - Gespräch mit GPT-4

Patrick Kurmann:
> Wie könnte ich ein Messaging-System implementieren, das speicher-intern zwischen verschiedenen .NET Modulen Nachrichten austauscht?

ChatGPT: 
Um ein Messaging-System zu implementieren, das speicher-intern zwischen verschiedenen .NET Modulen Nachrichten austauscht, können Sie verschiedene Ansätze verfolgen. Einer der gängigsten und effizientesten Wege ist die Verwendung eines Event- und Delegate-Mechanismus, kombiniert mit einem Publisher-Subscriber-Modell. Dies ermöglicht eine lose Kopplung zwischen den Komponenten und erleichtert die Wartung und Erweiterung des Systems. Hier sind die grundlegenden Schritte, um ein solches System zu entwickeln:

1. **Definieren Sie ein Nachrichtenmodell:** Erstellen Sie Klassen oder Strukturen, die die Nachrichten repräsentieren, die zwischen den Modulen ausgetauscht werden sollen. Dies könnte einfache Typen für einfache Ereignisse bis hin zu komplexen Klassen für umfangreichere Daten umfassen.

2. **Implementieren Sie das Publisher-Subscriber-Modell:**
    - **Publisher:** Das Modul, das Nachrichten versendet, wird als Publisher bezeichnet. Jeder Publisher muss eine Möglichkeit bieten, Subscriber zu registrieren und zu entfernen, sowie eine Methode, um Nachrichten an alle registrierten Subscriber zu senden.
    - **Subscriber:** Module, die Nachrichten empfangen möchten, werden als Subscriber bezeichnet. Ein Subscriber muss eine Methode implementieren, die als Callback dient, wenn eine Nachricht empfangen wird.

3. **Verwenden Sie Delegates und Events:** In C# bieten Delegates und Events einen natürlichen Mechanismus für die Implementierung des Publish-Subscribe-Musters. Ein Delegate repräsentiert eine Methode, die später aufgerufen werden kann, und ein Event ist ein Mechanismus, der Delegates verwaltet und Aufrufe an alle registrierten Delegates sendet.
   
    - **Event-Definition:** Definieren Sie in Ihrem Publisher ein Event basierend auf einem Delegate, das dem Signaturvertrag Ihrer Nachrichten entspricht.
    - **Event-Abonnement:** Subscriber registrieren sich beim Publisher, indem sie ihre Callback-Methode an das Event des Publishers binden.
    - **Event-Benachrichtigung:** Wenn der Publisher eine Nachricht senden möchte, löst er das Event aus, und alle registrierten Subscriber werden über ihre Callback-Methoden benachrichtigt.

4. **Implementierungsbeispiel:**

```csharp
public class Nachricht
{
    public string Inhalt { get; set; }
}

public class Publisher
{
    public delegate void NachrichtEmpfangenHandler(Nachricht nachricht);
    public event NachrichtEmpfangenHandler NachrichtEmpfangen;

    public void Senden(Nachricht nachricht)
    {
        NachrichtEmpfangen?.Invoke(nachricht);
    }
}

public class Subscriber
{
    public void Abonnieren(Publisher publisher)
    {
        publisher.NachrichtEmpfangen += OnNachrichtEmpfangen;
    }

    public void Abbestellen(Publisher publisher)
    {
        publisher.NachrichtEmpfangen -= OnNachrichtEmpfangen;
    }

    private void OnNachrichtEmpfangen(Nachricht nachricht)
    {
        Console.WriteLine($"Nachricht erhalten: {nachricht.Inhalt}");
    }
}
```

5. **Testen Sie Ihr System:** Erstellen Sie Instanzen Ihres Publishers und eines oder mehrerer Subscriber. Registrieren Sie die Subscriber beim Publisher und testen Sie die Nachrichtenübermittlung, indem Sie Nachrichten vom Publisher senden lassen.

Dieser Ansatz ermöglicht es Ihnen, ein effizientes, speicherinternes Messaging-System zu implementieren, das flexibel und erweiterbar ist. Sie können es an die spezifischen Bedürfnisse Ihres Projekts anpassen, indem Sie verschiedene Nachrichtentypen und -formate definieren sowie die Logik für die Registrierung und Benachrichtigung der Subscriber verfeinern.

---

> Nimm als Beispiel eine Nachricht "NewVideoFileAdded" mit dem Inhalt "string Path". Ich möchte nun diese Logik in ein Modul packen, dass ich dann abonnieren kann.

Um die Logik für eine spezifische Nachricht wie "NewVideoFileAdded" mit dem Inhalt "string Path" in ein Modul zu packen und dieses dann abonnieren zu können, sollten Sie zunächst eine spezialisierte Nachrichtenklasse erstellen. Danach implementieren Sie das Publisher-Modul, welches diese spezifische Nachricht versenden kann, sowie ein Interface oder eine Basisklasse für Subscriber, die sich für solche Nachrichten interessieren.

Hier ist ein detaillierter Ansatz, um dies zu erreichen:

### Schritt 1: Definieren Sie die Nachrichtenklasse

```csharp
public class NewVideoFileAddedEventArgs : EventArgs
{
    public string Path { get; private set; }

    public NewVideoFileAddedEventArgs(string path)
    {
        Path = path;
    }
}
```

### Schritt 2: Erstellen Sie das Publisher-Modul

Das Publisher-Modul sollte in der Lage sein, Ereignisse für das Hinzufügen eines neuen Videodateipfades zu feuern. Wir nutzen hier `EventArgs` und Events.

```csharp
public class VideoFilePublisher
{
    // Event, das gefeuert wird, wenn ein neues Video hinzugefügt wird
    public event EventHandler<NewVideoFileAddedEventArgs> NewVideoFileAdded;

    // Methode zum Feuern des Events
    public void AddVideoFile(string path)
    {
        OnNewVideoFileAdded(new NewVideoFileAddedEventArgs(path));
    }

    // Geschützte virtuelle Methode zum Auslösen des Events
    protected virtual void OnNewVideoFileAdded(NewVideoFileAddedEventArgs e)
    {
        NewVideoFileAdded?.Invoke(this, e);
    }
}
```

### Schritt 3: Implementieren Sie die Subscriber-Logik

Subscriber sollten eine Methode implementieren, die als Event-Handler dient. Dieser Handler wird aufgerufen, wenn das Event ausgelöst wird.

```csharp
public class VideoFileSubscriber
{
    public void Subscribe(VideoFilePublisher publisher)
    {
        publisher.NewVideoFileAdded += OnNewVideoFileAdded;
    }

    public void Unsubscribe(VideoFilePublisher publisher)
    {
        publisher.NewVideoFileAdded -= OnNewVideoFileAdded;
    }

    // Methode, die aufgerufen wird, wenn das Event ausgelöst wird
    private void OnNewVideoFileAdded(object sender, NewVideoFileAddedEventArgs e)
    {
        Console.WriteLine($"Ein neues Video wurde hinzugefügt: {e.Path}");
    }
}
```

### Schritt 4: Verwendung

Erstellen Sie Instanzen des Publishers und des Subscribers, abonnieren Sie den Subscriber beim Publisher und testen Sie die Logik, indem Sie die Methode `AddVideoFile` aufrufen.

```csharp
var publisher = new VideoFilePublisher();
var subscriber = new VideoFileSubscriber();

subscriber.Subscribe(publisher);

publisher.AddVideoFile("/pfad/zum/video.mp4");

subscriber.Unsubscribe(publisher);
```

Dieser Ansatz ermöglicht es Ihnen, das Messaging für spezifische Ereignisse wie das Hinzufügen neuer Videodateien modular und wiederverwendbar zu gestalten. Durch die Verwendung von `EventArgs` und Events können Sie leicht weitere Ereignistypen und zugehörige Handler hinzufügen, um Ihr Messaging-System nach Bedarf zu erweitern.

---

> Mir gefällt der Ansatz sehr gut. Könnte ich die Logik um Nachrichten zu Empfangen auch anbieten in einem ServiceCollectionExtensions um damit das ganze Modul einer Host-.NET-Anwendung hinzuzufügen?

> Oder welche andere Modularisierungsmöglichkeiten habe ich. Grundsätzlich möchte ich .NET-Programmmodule in einem eigenen GitHub-Repo entwickeln und als NuGet-Package veröffentlichen. Interessierte .NET-Anwendungen können dieses Package dann installieren und benutzen (vielleicht über ServiceCollectionExtensions).

Ja, Sie können Ihre Logik um Nachrichten zu empfangen und zu senden in einem `ServiceCollectionExtensions`-Klasse anbieten, um die Integration in .NET-Anwendungen zu vereinfachen. Das ermöglicht es, Ihre Module als Dienste in der `IServiceCollection` der Host-Anwendung zu registrieren. Dadurch können andere Entwickler Ihr Modul einfach durch Dependency Injection (DI) in ihre Anwendungen einbinden. Dieser Ansatz ist besonders nützlich, wenn Sie planen, Ihr Modul als NuGet-Package zu veröffentlichen.

Hier ist ein grundlegendes Beispiel, wie Sie eine `ServiceCollectionExtensions`-Klasse für Ihr Modul erstellen können:

### Schritt 1: Erstellen der Extension-Methode für IServiceCollection

```csharp
using Microsoft.Extensions.DependencyInjection;

public static class VideoMessagingServiceCollectionExtensions
{
    public static IServiceCollection AddVideoMessaging(this IServiceCollection services)
    {
        // Registrieren des Publishers als Singleton, da wir eine einzige Instanz für die gesamte App wollen
        services.AddSingleton<VideoFilePublisher>();

        // Registrieren des Subscribers. Abhängig von der Nutzung kann dies als Scoped oder Transient registriert werden
        services.AddTransient<VideoFileSubscriber>();

        return services;
    }
}
```

### Schritt 2: Nutzung in einer Host-.NET-Anwendung

Nachdem Sie Ihr NuGet-Package veröffentlicht haben, können Entwickler es in ihren Projekten installieren und wie folgt nutzen:

```csharp
public class Program
{
    public static void Main(string[] args)
    {
        var host = CreateHostBuilder(args).Build();

        // Beispiel für das Auslösen eines Events nach der Konfiguration
        var publisher = host.Services.GetService<VideoFilePublisher>();
        publisher.AddVideoFile("/pfad/zum/video.mp4");

        host.Run();
    }

    public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureServices((hostContext, services) =>
            {
                services.AddVideoMessaging();
            });
}
```

### Weitere Modularisierungsmöglichkeiten

- **NuGet-Package veröffentlichen:** Entwickeln Sie Ihre Module getrennt und veröffentlichen Sie sie als NuGet-Packages. Das ermöglicht eine einfache Versionierung und Abhängigkeitsverwaltung.
- **Verwenden von Interfaces:** Definieren Sie klare Interfaces für Ihre Dienste, um die Abhängigkeiten zwischen den Modulen und der Host-Anwendung zu minimieren. Dies erleichtert auch das Testen und Mocking.
- **Konfigurationsunterstützung:** Bieten Sie Möglichkeiten an, Ihre Dienste über die `appsettings.json` oder andere Konfigurationsmechanismen der Host-Anwendung zu konfigurieren.
- **Ereignisgesteuerte Architektur:** Verwenden Sie Integration Events und Event Busse (z.B. mit MassTransit oder NServiceBus) für eine noch losere Kopplung und verteilte Systeme.
- **Logging und Monitoring:** Integrieren Sie Unterstützung für Logging und Monitoring, um die Verwendung Ihres Moduls in Produktionsumgebungen zu erleichtern.

Indem Sie diese Richtlinien befolgen, können Sie hochmodulare, wiederverwendbare und leicht zu integrierende .NET-Programmmodule entwickeln, die sich ideal für die Veröffentlichung als NuGet-Package eignen.
