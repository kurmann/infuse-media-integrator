using System.Collections.Concurrent;

namespace Kurmann.InfuseMediaIntegrator.Services;

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
        // Fügt auf threadsichere Weise einen Nachrichten-Handler für den angegebenen Nachrichtentyp hinzu.
        // Die Operationen 'GetOrAdd' und 'Add' sind atomar und verhindern race conditions.
        var subscribers = _handlers.GetOrAdd(typeof(TMessage), _ => []);
        subscribers.Add(handler);
    }
}
