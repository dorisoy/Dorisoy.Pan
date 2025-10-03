using System;
using System.Threading;
using System.Collections;
using System.Reflection;
using System.Runtime.Serialization;


namespace LocalizationManager.Avalonia.Reactive;
public class Subjected<T> : SubjectBase<T>, IUnsubscribe<T>
{
    public Subjected(T value)
    {
        _value = value;
        _observers = ImmutableList<SubjectDisposable<T>>.Empty;
    }
    private readonly object _gate = new();
    private ImmutableList<SubjectDisposable<T>> _observers;
    private bool _isStopped;
    private T _value;
    private Exception? _exception;
    private bool _isDisposed;

    public override bool HasObservers => _observers.Data.Length > 0;

    public override bool IsDisposed
    {
        get
        {
            lock (_gate)
                return _isDisposed;
        }
    }

    public T Value
    {
        get
        {
            lock (_gate)
            {
                CheckDisposed();
                _exception?.Throw();
                return _value;
            }
        }
    }

    public bool TryGetValue(out T? value)
    {
        lock (_gate)
        {
            if (_isDisposed)
            {
                value = default;
                return false;
            }

            _exception?.Throw();
            value = _value;
            return true;
        }
    }

    public override void Dispose()
    {
        lock (_gate)
        {
            _isDisposed = true;
            _observers = null!;
            _value = default!;
            _exception = null;
        }
    }

    public override void OnCompleted()
    {
        SubjectDisposable<T>[]? os = null;

        lock (_gate)
        {
            CheckDisposed();

            if (!_isStopped)
            {
                os = _observers.Data;
                _observers = ImmutableList<SubjectDisposable<T>>.Empty;
                _isStopped = true;
            }
        }

        if (os != null)
        {
            foreach (var o in os)
                o.OnCompleted();
        }
    }

    public override void OnError(Exception error)
    {
        if (error == null)
            throw new ArgumentNullException(nameof(error));

        SubjectDisposable<T>[]? os = null;

        lock (_gate)
        {
            CheckDisposed();

            if (!_isStopped)
            {
                os = _observers.Data;
                _observers = ImmutableList<SubjectDisposable<T>>.Empty;
                _isStopped = true;
                _exception = error;
            }
        }

        if (os != null)
        {
            foreach (var o in os)
                o.OnError(error);
        }
    }

    public override void OnNext(T value)
    {
        SubjectDisposable<T>[]? os = null;

        lock (_gate)
        {
            CheckDisposed();

            if (!_isStopped)
            {
                _value = value;
                os = _observers.Data;
            }
        }

        if (os != null)
        {
            foreach (var o in os)
                o.OnNext(value);
        }
    }

    public override IDisposable Subscribe(IObserver<T> observer)
    {
        if (observer == null)
            throw new ArgumentNullException(nameof(observer));

        Exception? ex;
        lock (_gate)
        {
            CheckDisposed();

            if (!_isStopped)
            {
                var subject = new SubjectDisposable<T>(this, observer);
                _observers = _observers.Add(subject);
                observer.OnNext(_value);
                return subject;
            }

            ex = _exception;
        }

        if (ex != null)
            observer.OnError(ex);
        else
            observer.OnCompleted();

        return Disposable.Empty;
    }

    public bool Unsubscribe(object observer)
    {
        if (observer is not SubjectDisposable<T> subject)
            return false;

        lock (_gate)
        {
            if (!_isDisposed)
                _observers = _observers.Remove(subject);
        }

        return true;
    }

    private void CheckDisposed()
    {
        if (_isDisposed)
            throw new ObjectDisposedException(string.Empty);
    }
}
