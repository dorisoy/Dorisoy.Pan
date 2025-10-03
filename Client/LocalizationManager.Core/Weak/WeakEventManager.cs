using System;
using System.Collections.Generic;

namespace LocalizationManager.Core.Weak;
public class WeakEventManager
{
    private struct Subscription
    {
        public readonly WeakReference? Subscriber;

        public readonly MethodInfo Handler;

        public Subscription(WeakReference? subscriber, MethodInfo handler)
        {
            Subscriber = subscriber;
            Handler = handler ?? throw new ArgumentNullException("handler");
        }
    }

    private readonly Dictionary<string, List<Subscription>> _eventHandlers = new Dictionary<string, List<Subscription>>();

    public void AddEventHandler<TEventArgs>(EventHandler<TEventArgs> handler, [CallerMemberName] string eventName = "") where TEventArgs : EventArgs
    {
        if (string.IsNullOrEmpty(eventName))
            throw new ArgumentNullException("eventName");

        if (handler == null)
            throw new ArgumentNullException("handler");

        AddEventHandler(eventName, handler.Target, handler.GetMethodInfo());
    }

    public void AddEventHandler(Delegate? handler, [CallerMemberName] string eventName = "")
    {
        if (string.IsNullOrEmpty(eventName))
            throw new ArgumentNullException("eventName");

        if (handler == null)
            throw new ArgumentNullException("handler");

        AddEventHandler(eventName, handler.Target, handler.GetMethodInfo());
    }

    public void HandleEvent(object sender, object args, string eventName)
    {
        var list = new List<(object?, MethodInfo)>();
        List<Subscription> list2 = new List<Subscription>();
        if (_eventHandlers.TryGetValue(eventName, out List<Subscription> value))
        {
            for (int i = 0; i < value.Count; i++)
            {
                Subscription item = value[i];
                if (item.Subscriber == null)
                {
                    list.Add((null, item.Handler));
                    continue;
                }
                var obj = item.Subscriber?.Target;
                if (obj == null)
                    list2.Add(item);
                else
                    list.Add((obj, item.Handler));
            }
            for (int j = 0; j < list2.Count; j++)
            {
                Subscription item2 = list2[j];
                value.Remove(item2);
            }
        }
        for (int k = 0; k < list.Count; k++)
        {
            var tuple = list[k];
            var (obj2, _) = tuple;
            tuple.Item2.Invoke(obj2, new object[2] { sender, args });
        }
    }

    public void RemoveEventHandler<TEventArgs>(EventHandler<TEventArgs> handler, [CallerMemberName] string eventName = "") where TEventArgs : EventArgs
    {
        if (string.IsNullOrEmpty(eventName))
            throw new ArgumentNullException("eventName");

        if (handler == null)
            throw new ArgumentNullException("handler");

        RemoveEventHandler(eventName, handler.Target, handler.GetMethodInfo());
    }

    public void RemoveEventHandler(Delegate? handler, [CallerMemberName] string eventName = "")
    {
        if (string.IsNullOrEmpty(eventName))
            throw new ArgumentNullException("eventName");

        if (handler == null)
            throw new ArgumentNullException("handler");

        RemoveEventHandler(eventName, handler.Target, handler.GetMethodInfo());
    }

    private void AddEventHandler(string eventName, object? handlerTarget, MethodInfo methodInfo)
    {
        if (!_eventHandlers.TryGetValue(eventName, out List<Subscription> value))
        {
            value = new List<Subscription>();
            _eventHandlers.Add(eventName, value);
        }
        if (handlerTarget == null)
            value.Add(new Subscription(null, methodInfo));
        else
            value.Add(new Subscription(new WeakReference(handlerTarget), methodInfo));
    }

    private void RemoveEventHandler(string eventName, object? handlerTarget, MemberInfo methodInfo)
    {
        if (!_eventHandlers.TryGetValue(eventName, out List<Subscription> value))
            return;

        for (int num = value.Count; num > 0; num--)
        {
            Subscription item = value[num - 1];
            if (item.Subscriber?.Target == handlerTarget && !(item.Handler.Name != methodInfo.Name))
            {
                value.Remove(item);
                break;
            }
        }
    }
}
