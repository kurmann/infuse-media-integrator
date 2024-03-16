using System.Collections.Concurrent;

namespace Kurmann.InfuseMediaIntegrator.Services;

public interface IEventMessage { }

public abstract class EventMessageBase : IEventMessage
{
    public string Id { get; }

    protected EventMessageBase() => Id = Ulid.NewUlid().ToString();
}

public interface IMessageService
{
    // Sendet eine Nachricht eines beliebigen Typs.
    void Send<TMessage>(TMessage message);

    // Abonniert eine Nachricht eines beliebigen Typs mit einem Handler.
    void Subscribe<TMessage>(Action<TMessage> handler);
}

public class MessageService : IMessageService
{
    // Eine threadsichere Sammlung zum Speichern von Nachrichten-Handlern.
    // 'ConcurrentDictionary' wird aufgrund seiner Thread-Sicherheit und seiner effizienten, lock-freien Lesevorgänge verwendet.
    private readonly ConcurrentDictionary<Type, List<Delegate>> _handlers = new();

    /// <summary>
    /// Sendet eine Nachricht vom Typ 'TMessage' an alle Abonnenten.
    /// </summary>
    /// <typeparam name="TMessage"></typeparam>
    /// <param name="message"></param>
    public void Send<TMessage>(TMessage message)
    {
        // Wenn Abonnenten für den Nachrichtentyp vorhanden sind, rufe deren Handler auf.
        if (_handlers.TryGetValue(typeof(TMessage), out var subscribers))
        {
            foreach (var handler in subscribers)
            {
                // Rufe den Handler dynamisch mit der Nachricht auf.
                handler.DynamicInvoke(message);
            }
        }
    }

    /// <summary>
    /// Abonniert eine Nachricht vom Typ 'TMessage' mit einem Handler.
    /// </summary>
    /// <typeparam name="TMessage"></typeparam>
    /// <param name="handler"></param>
    public void Subscribe<TMessage>(Action<TMessage> handler)
    {
        var key = typeof(TMessage);
        var newHandlersList = new List<Delegate>();

        // Diese Umsetzung stellt sicher, dass das Hinzufügen von Handlern threadsicher ist, indem es eine neue Liste von 
        // Handlern erstellt und diese dann ersetzt, anstatt die bestehende Liste direkt zu ändern.
        _handlers.AddOrUpdate(key,

            // Hinzufügen, wenn der Key noch nicht existiert
            addValueFactory: _ => [handler],
            
            // Update der vorhandenen Liste, wenn der Key existiert
            updateValueFactory: (type, existingHandlers) =>
            {
                newHandlersList = new List<Delegate>(existingHandlers)
                {
                    handler
                };
                return newHandlersList;
            }
        );
    }

}
