using System.Collections.Concurrent;

namespace Kurmann.InfuseMediaIntegrator.Services;

public interface IEventMessage { }

public abstract class EventMessageBase : IEventMessage
{
    protected Ulid Ulid { get; }

    public string Id => Ulid.ToString();

    public DateTimeOffset Timestamp { get; }

    protected EventMessageBase()
    {
        Ulid = Ulid.NewUlid();
        Timestamp = Ulid.Time.ToLocalTime();
    }
}

public interface IMessageService
{
    // Sendet eine Nachricht eines beliebigen Typs.
    void Publish<TMessage>(TMessage message) where TMessage : IEventMessage;

    // Abonniert eine Nachricht eines beliebigen Typs mit einem Handler.
    void Subscribe<TMessage>(Action<TMessage> handler) where TMessage : IEventMessage;

    // Deabonniert eine Nachricht eines beliebigen Typs mit einem Handler.
    void Unsubscribe<TMessage>(Action<TMessage> handler) where TMessage : IEventMessage;
}

/// <summary>
/// Verantwortlich für das Senden und Empfangen von Nachrichten.
/// </summary>
/// <remarks>
/// Dieser Service ist ein einfacher Event-Bus, der Nachrichten an registrierte Handler sendet.
/// </remarks>
/// <seealso cref="IMessageService" />
/// <seealso cref="IEventMessage" />
/// <seealso cref="EventMessageBase" />
/// <seealso cref="IEventMessage" />
//
// Einige wichtige Punkte zur Implementierung:
//
// Thread-Sicherheit: Durch die Verwendung von lock beim Aktualisieren der Handler-Listen wird sichergestellt, dass die Operationen 
// threadsicher sind. Dies verhindert, dass zwei Threads gleichzeitig die Liste der Handler modifizieren.
// Verwendung von ConcurrentDictionary: Die Verwendung von ConcurrentDictionary anstelle von Dictionary stellt sicher, dass das
//
// Hinzufügen und Entfernen von Einträgen threadsicher ist.
// Verwendung von ToList() bei Iteration: Durch das Kopieren der Liste vor der Iteration wird sichergestellt, dass die Iteration
// threadsicher ist. Dies verhindert, dass ein anderer Thread die Liste während der Iteration modifiziert.
//
// Performance: Die Verwendung von lock kann zu einem Engpass führen, wenn viele Threads versuchen, gleichzeitig zu abonnieren oder zu 
// deabonnieren. Dies ist in der Regel in Hochlast-Szenarien relevant. In den meisten Anwendungsfällen sollte dies jedoch kein Problem darstellen.
//
// Liste kopieren vor der Modifikation: Durch das Kopieren der Liste vor der Modifikation und das Ersetzen der alten Liste im Dictionary 
// wird die Unveränderlichkeit der Listen für außenstehende Betrachter sichergestellt. Das bedeutet, dass keine Änderungen an der 
// Liste vorgenommen werden, während ein anderer Thread sie möglicherweise durchläuft.
public class MessageService : IMessageService
{
    private readonly ConcurrentDictionary<Type, List<Delegate>> _handlers = new();

    public void Publish<TMessage>(TMessage message) where TMessage : IEventMessage
    {
        if (_handlers.TryGetValue(typeof(TMessage), out var subscribers))
        {
            foreach (var handler in subscribers.ToList()) // ToList() für Thread-Sicherheit bei Iteration
            {
                handler.DynamicInvoke(message);
            }
        }
    }

    public void Subscribe<TMessage>(Action<TMessage> handler) where TMessage : IEventMessage
    {
        var key = typeof(TMessage);
        _handlers.AddOrUpdate(key,
            _ => [handler],
            (_, existingHandlers) =>
            {
                lock (existingHandlers)
                {
                    var newHandlersList = new List<Delegate>(existingHandlers) { handler };
                    return newHandlersList;
                }
            }
        );
    }

    public void Unsubscribe<TMessage>(Action<TMessage> handler) where TMessage : IEventMessage
    {
        var key = typeof(TMessage);
        if (_handlers.TryGetValue(key, out var subscribers))
        {
            lock (subscribers)
            {
                if (subscribers.Contains(handler))
                {
                    var newSubscribersList = new List<Delegate>(subscribers);
                    newSubscribersList.Remove(handler);
                    
                    // Ersetze die alte Liste mit der neuen, aktualisierten Liste
                    if (newSubscribersList.Count > 0)
                    {
                        _handlers[key] = newSubscribersList;
                    }
                    else
                    {
                        // Wenn keine Subscriber mehr vorhanden sind, entferne den Eintrag aus dem Dictionary
                        ((ICollection<KeyValuePair<Type, List<Delegate>>>)_handlers).Remove(new KeyValuePair<Type, List<Delegate>>(key, subscribers));
                    }
                }
            }
        }
    }
}
