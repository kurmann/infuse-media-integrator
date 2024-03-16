using System.Collections.Concurrent;

namespace Kurmann.InfuseMediaIntegrator.Services;

public interface IMessageService
{
    void Send<TNachricht>(TNachricht nachricht);
    void Subscribe<TNachricht>(Action<TNachricht> handler);
}

public class MessageService : IMessageService
{
    private readonly ConcurrentDictionary<Type, List<Delegate>> _handlers = new();

    public void Send<TMessage>(TMessage nachricht)
    {
        if (_handlers.TryGetValue(typeof(TMessage), out var subscribers))
        {
            foreach (var handler in subscribers)
            {
                handler.DynamicInvoke(nachricht);
            }
        }
    }

    public void Subscribe<TNachricht>(Action<TNachricht> handler)
    {
        var subscribers = _handlers.GetOrAdd(typeof(TNachricht), _ => new List<Delegate>());
        subscribers.Add(handler);
    }
}
