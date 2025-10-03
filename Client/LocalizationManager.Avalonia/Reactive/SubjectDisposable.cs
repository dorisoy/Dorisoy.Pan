using System;
using System.Threading;

namespace LocalizationManager.Avalonia.Reactive;
public class SubjectDisposable<T> : IDisposable
{
    public SubjectDisposable(IUnsubscribe<T> subject, IObserver<T> observer)
    {
        _subject = subject;
        _observer = observer;
    }

    private IUnsubscribe<T>? _subject;
    private volatile IObserver<T>? _observer;

    public void OnCompleted()
    {
        var observer = Interlocked.Exchange(ref _observer, null);
        if (observer == null)
            return;

        observer.OnCompleted();
        Dispose();
    }

    public void OnError(Exception error) 
    {
        var observer = Interlocked.Exchange(ref _observer, null);
        if (observer == null)
            return;

        observer.OnError(error);
        Dispose();
    }

    public void OnNext(T value) 
    {
        _observer?.OnNext(value);
    }

    public void Dispose()
    {
        var observer = Interlocked.Exchange(ref _observer, null);
        if (observer == null)
            return;

        _subject?.Unsubscribe(this);
        _subject = default;
    }
}
