# Hosting

Gespräch mit ChatGPT am 16. März 2024.

> Kannst du mir den HostBuilder genauer erläutern?

```csharp
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Kurmann.InfuseMediaIntegrator;

internal class Program
{
    static void Main(string[] args)
    {
        CreateHostBuilder(args).Build().Run();
    }

    public static IHostBuilder CreateHostBuilder(string[] args) =>
    Host.CreateDefaultBuilder(args)
        .ConfigureServices((hostContext, services) =>
        {
            // services.AddHostedService<MyModuleService>();
            // Weitere Dienste konfigurieren
        });
}
```

> Wenn ich die Konsolenanwendung starte, was geschieht dann? Kann ich bereits auf einem Standardport auf die (leere) Anwendung zugreifen? Ich möchte die Mechanismen des .NET Hostingmodels verstehen. So viel ich weiss, ist jede Webanwendung im Grund genommen eine SelfHosting Konsole Applikation.

Der `HostBuilder` in .NET dient als zentrales Element für das Konfigurieren und Starten von Anwendungen, insbesondere bei Anwendungen, die auf dem Generic Host (`Microsoft.Extensions.Hosting`) basieren. Dieses Hostingmodell wird sowohl in Konsolenanwendungen als auch in Webanwendungen (z.B. ASP.NET Core) verwendet, um eine Vielzahl von Hintergrunddiensten (`IHostedService`), Konfigurationsoptionen, Logging und Dependency Injection zu unterstützen. Hier erkläre ich den Prozess und die Mechanismen im Detail:

### Was passiert beim Starten der Konsolenanwendung mit `HostBuilder`?

1. **Initialisierung des Hosts:** Durch den Aufruf von `CreateHostBuilder(args).Build().Run();` in der `Main`-Methode wird der Host initialisiert. `CreateHostBuilder` konfiguriert den Host mit den Standardeinstellungen, die für die meisten Anwendungen geeignet sind. Diese beinhalten die Konfiguration der App-Einstellungen, Logging, Dependency Injection und mehr.

2. **Konfiguration der Services:** Im Lambda von `.ConfigureServices((hostContext, services) => {})` haben Sie die Möglichkeit, Ihre eigenen Services, einschließlich Hintergrunddienste (`IHostedService`), zu registrieren. Diese Services werden dann vom DI-Container verwaltet und können in Ihrer Anwendung verwendet werden.

3. **Start des Hosts:** `.Run()` startet den Host und damit alle registrierten `IHostedService`-Implementierungen. Für jeden registrierten `IHostedService` ruft der Host die `StartAsync`-Methode beim Starten und `StopAsync` beim Herunterfahren auf.

### Zugriff auf die Anwendung:

- **Konsolenanwendungen:** Standardmäßig bietet eine reine Konsolenanwendung, die den `HostBuilder` verwendet, keinen HTTP-Server oder ähnliches. Das bedeutet, es gibt keinen "Standardport", auf den zugegriffen werden könnte. Für Hintergrundverarbeitung oder Worker-Services ist das ausreichend.

- **Webanwendungen:** In einer Webanwendung (z.B. mit ASP.NET Core) fügt der `HostBuilder` zusätzliche Konfigurationen hinzu, um Kestrel (den integrierten Webserver) oder einen anderen Webserver zu starten. Hier wird typischerweise ein Standardport (wie 5000 für HTTP und 5001 für HTTPS) genutzt, auf den dann zugegriffen werden kann. Dies erfolgt durch den Aufruf von `.ConfigureWebHostDefaults(webBuilder => { webBuilder.UseStartup<Startup>(); })`, welcher die Web-spezifischen Konfigurationen einrichtet, einschließlich der Port-Konfigurationen.

### Verständnis des Hostingmodells:

- **Self-Hosting Konsole Applikation:** Tatsächlich fungiert jede ASP.NET Core Webanwendung unter der Haube als eine Self-Hosting-Konsolenanwendung. Der Generic Host (`IHostBuilder`) orchestriert dabei nicht nur Hintergrunddienste, sondern auch den Webserver (Kestrel) und weitere Web-spezifische Features. Dies ermöglicht eine konsistente Entwicklungs- und Laufzeitumgebung, unabhängig davon, ob es sich um eine Web-, Konsolen- oder Worker-Service-Anwendung handelt.

- **Flexibilität und Erweiterbarkeit:** Durch die Verwendung des Generic Hosts in .NET können Entwickler eine konsistente und erweiterbare Architektur für eine Vielzahl von Anwendungstypen erstellen, wobei sie von einem reichhaltigen Ökosystem von integrierten und Drittanbieter-Services profitieren.

Der `HostBuilder` und das Generic Hosting-Modell bieten also eine flexible und leistungsstarke Basis für die Entwicklung moderner .NET-Anwendungen, sowohl für Konsolen-basierte Hintergrundservices als auch für komplexe Webanwendungen.

---

> Vielen Dank für deine Erklärung. Grundsätzlich ist der Hosted Service dazu da, um die Verwaltung des App-Servers zu übernehmen, unter anderem?

Ja, genau. Der Begriff „Hosted Service“ in .NET bezieht sich typischerweise auf eine Implementierung des `IHostedService`-Interfaces, das Teil des Generic Host (`Microsoft.Extensions.Hosting`) ist. Hosted Services sind dazu da, Hintergrundverarbeitungen oder langlaufende Prozesse innerhalb der Anwendung zu verwalten, was einschließt, aber nicht beschränkt ist auf die Verwaltung des Lebenszyklus von App-Services wie einem Webserver.

### Hauptverantwortlichkeiten von Hosted Services:

1. **Lebenszyklus-Management:** Sie ermöglichen es Ihnen, Start- und Stopp-Routinen für Ihre Anwendung zu definieren. Dies ist besonders nützlich für die Initialisierung beim Start und die ordnungsgemäße Ressourcenfreigabe beim Herunterfahren.

2. **Hintergrundverarbeitung:** Viele Anwendungen benötigen Prozesse, die im Hintergrund laufen, ohne die Hauptausführung der Anwendung zu blockieren. Beispiele hierfür könnten periodische Datenprüfungen, das Verarbeiten von Warteschlangennachrichten oder das Ausführen von Wartungsaufgaben sein.

3. **Webserver-Management:** In Webanwendungen wird oft Kestrel als interner Webserver verwendet. Durch die Integration als Hosted Service wird der Webserver zusammen mit der Anwendung gestartet und gestoppt. Dies ist ein konkretes Beispiel dafür, wie Hosted Services die Verwaltung von App-Services übernehmen.

4. **Integrierte und benutzerdefinierte Dienste:** .NET bietet einige integrierte Hosted Services, wie z.B. das Hosting von ASP.NET Core-Webanwendungen. Darüber hinaus können Entwickler eigene Hosted Services definieren, um spezifische Hintergrundverarbeitungen oder Dienste zu implementieren.

### Beispiel für einen benutzerdefinierten Hosted Service:

```csharp
public class MeinHintergrundService : IHostedService, IDisposable
{
    private Timer _timer;

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _timer = new Timer(DoWork, null, TimeSpan.Zero, 
            TimeSpan.FromMinutes(5));

        return Task.CompletedTask;
    }

    private void DoWork(object state)
    {
        // Logik, die periodisch ausgeführt wird
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _timer?.Change(Timeout.Infinite, 0);
        return Task.CompletedTask;
    }

    public void Dispose()
    {
        _timer?.Dispose();
    }
}
```

In diesem Beispiel ist `MeinHintergrundService` ein benutzerdefinierter Hosted Service, der eine periodische Aufgabe im Hintergrund ausführt. Solche Services können über die `ConfigureServices`-Methode im `Startup`-Prozess der Anwendung registriert werden:

```csharp
services.AddHostedService<MeinHintergrundService>();
```

Zusammenfassend bieten Hosted Services in .NET eine strukturierte und skalierbare Methode, um Hintergrundprozesse und Services zu verwalten, und sind ein zentraler Bestandteil des Generic Hosts, der die Grundlage für das Hosting-Modell in modernen .NET-Anwendungen bildet.

---

> Sehr gut, jetzt verstehe ich besser. Kannst du mir etwas zu der Standardkonfiguration sagen, mit der der Hosted Service initialisiert wird? Was genau gehört zur Standardkonfiguration? Welche Services schliesst das mit ein? Du sprachst ja schon von Dependency Injection und weiteren.

Die Standardkonfiguration des Generic Host (`Microsoft.Extensions.Hosting.HostBuilder`) in .NET-Anwendungen stellt eine Reihe von Basisdiensten und Konfigurationsoptionen bereit, die für viele Anwendungsszenarien sinnvoll und hilfreich sind. Wenn Sie den `HostBuilder` nutzen, um Ihre Anwendung zu konfigurieren, werden automatisch verschiedene Aspekte der Anwendungsumgebung eingerichtet, die Sie dann nach Bedarf anpassen können. Hier sind einige Schlüsselelemente der Standardkonfiguration:

### 1. **Konfigurationssystem:**

- Lädt Konfigurationseinstellungen aus mehreren Quellen, einschließlich Umgebungsvariablen, Befehlszeilenargumenten und Konfigurationsdateien (wie `appsettings.json` und `appsettings.{Environment}.json`).
- Erlaubt es, Konfigurationseinstellungen leicht zu lesen und in die Anwendung zu injizieren.

### 2. **Logging:**

- Einrichtung eines umfassenden Logging-Systems, das mit verschiedenen Providern wie Console, Debug, EventSource und EventLog arbeitet.
- Unterstützt die Konfiguration von Logging-Levels und anderen Optionen durch das Konfigurationssystem.

### 3. **Dependency Injection (DI):**

- Einrichtung eines DI-Containers, der es ermöglicht, Abhängigkeiten durch Konstruktoren zu injizieren und so die lose Kopplung von Komponenten und die Testbarkeit zu fördern.
- DI wird für das Einbinden von Services wie Hintergrunddiensten (`IHostedService`), Optionsmustern und eigenen Services genutzt.

### 4. **Hosted Services:**

- Unterstützung für die Registrierung und Verwaltung von Hintergrunddiensten (`IHostedService`), die beim Start der Anwendung ausgeführt werden und beim Herunterfahren sauber beendet werden.
- Ermöglicht es, langlaufende Hintergrundaufgaben oder Initialisierungslogik als Teil des Anwendungslebenszyklus zu integrieren.

### 5. **Umweltinformationen:**

- Bereitstellung von Informationen über die Laufzeitumgebung, wie z.B. Entwicklung, Staging oder Produktion, die über Umgebungsvariablen oder Konfigurationseinstellungen gesteuert werden können.
- Ermöglicht es, die Anwendung unterschiedlich zu konfigurieren, je nachdem, in welcher Umgebung sie ausgeführt wird.

### 6. **Anwendungslebenszyklus-Management:**

- Ein Interface (`IHostApplicationLifetime`), über das man auf Ereignisse wie `ApplicationStarted`, `ApplicationStopping` und `ApplicationStopped` reagieren kann, um benutzerdefinierte Start- und Shutdown-Logik zu implementieren.

### 7. **Weitere integrierte Services:**

- Je nach Anwendungstyp (z.B. Webanwendung mit ASP.NET Core) können zusätzliche Grunddienste wie HTTP-Serverkonfigurationen, Routing, MVC und mehr automatisch eingerichtet werden.

Diese Standardkonfigurationen und Services bieten eine solide Grundlage für die Entwicklung von .NET-Anwendungen, da sie viele Gemeinsamkeiten und Grundanforderungen moderner Softwareanwendungen abdecken. Natürlich können Entwickler diese Standardkonfigurationen anpassen und erweitern, um spezifische Anforderungen und Szenarien ihrer Projekte zu unterstützen.
