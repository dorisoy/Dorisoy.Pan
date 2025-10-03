using System;
namespace LocalizationManager.Avalonia.Reactive;
public interface IUnsubscribe
{
    bool Unsubscribe(object observer);
}



public interface IUnsubscribe<T> : IUnsubscribe
{
}
