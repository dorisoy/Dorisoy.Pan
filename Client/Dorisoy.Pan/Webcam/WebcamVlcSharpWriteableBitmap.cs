//using Avalonia.Visuals.Media.Imaging;
using PixelFormat = Avalonia.Platform.PixelFormat;
using Disposable = System.Reactive.Disposables.Disposable;

namespace Dorisoy.PanClient.Webcam;

public sealed class WebcamVlcSharpWriteableBitmap : Bitmap
{
    private bool _disposed;
    private object _lockRead = new object();
    private object _lockWrite = new object();
    private PixelFormat? _pixelFormat;
    private WriteableBitmap _read;
    private ISubject<Unit> _rendered = new Subject<Unit>();
    private ISubject<Unit> _updated = new Subject<Unit>();

    /// <summary>
    /// 写入可写位图图像
    /// </summary>
    private WriteableBitmap _write;
    public WebcamVlcSharpWriteableBitmap(string fileName = "") : base(fileName)
    { }

    public IObservable<Unit> Rendered => _rendered;

    //public Size Size => GetValueSafe(x => x.Size);

    public IObservable<Unit> Updated => _updated;
    public event EventHandler Invalidated;

    public void Clear()
    {
        using (LockWrite())
        {
            _write?.Dispose();
            _write = null;
        }
        using (LockRead())
        {
            _read?.Dispose();
            _read = null;
        }

        NotifyUpdated();
    }

    public override void Dispose()
    {
        if (_disposed)
            return;

        _updated.OnCompleted();
        _rendered.OnCompleted();

        Clear();

        _disposed = true;
    }

    public void Draw(DrawingContext context, Rect sourceRect, Rect destRect, BitmapInterpolationMode bitmapInterpolationMode)
    {
        Render(context, 1, sourceRect, destRect, bitmapInterpolationMode);
    }

    public void Read(Action<Bitmap> action)
    {
        using (LockRead())
        {
            if (_read != null)
            {
                action(_read);
            }
        }
    }

    public void Render(DrawingContext context, double opacity, Rect sourceRect, Rect destRect, BitmapInterpolationMode bitmapInterpolationMode = BitmapInterpolationMode.Unspecified)
    {
        //Render(context, opacity, sourceRect, destRect, bitmapInterpolationMode);
        //Size.Width
        var bitmap = _read.CreateScaledBitmap(new PixelSize(100, 100), bitmapInterpolationMode);
        Read(b => context.DrawImage(bitmap, sourceRect, destRect));
        NotifyRendered();
    }


    public void Save(string fileName) => Read(b => b.Save(fileName));

    public void Save(Stream stream) => Read(b => b.Save(stream));

    public void Write(PixelSize size, Vector dpi, PixelFormat format, Action<ILockedFramebuffer> action)
    {
        using (LockWrite())
        {
            if (_write == null || _write.Dpi != dpi || _write.PixelSize != size || _pixelFormat != format)
            {
                _write?.Dispose();
                _write = new WriteableBitmap(size, dpi, format);
                _pixelFormat = format;
            }

            using (var fb = _write.Lock())
            {
                action(fb);
            }

            using (LockRead())
            {
                var tmp = _read;
                _read = _write;
                _write = tmp;
            }
        }

        NotifyUpdated();
    }

    private T GetValueSafe<T>(Func<Bitmap, T> getter, T defaultvalue = default(T))
    {
        if (_disposed)
            throw new ObjectDisposedException(nameof(WebcamVlcSharpWriteableBitmap));

        using (LockRead())
        {
            return _read == null ? defaultvalue : getter(_read);
        }
    }

    /// <summary>
    /// 锁
    /// </summary>
    /// <param name="lockObject"></param>
    /// <returns></returns>
    private IDisposable Lock(object lockObject)
    {
        Monitor.Enter(lockObject);
        return Disposable.Create(() => Monitor.Exit(lockObject));
    }

    /// <summary>
    /// 读锁
    /// </summary>
    /// <returns></returns>
    private IDisposable LockRead() => Lock(_lockRead);

    /// <summary>
    /// 写锁
    /// </summary>
    /// <returns></returns>
    private IDisposable LockWrite() => Lock(_lockWrite);

    /// <summary>
    /// 通知渲染
    /// </summary>
    private void NotifyRendered() => _rendered?.OnNext(Unit.Default);
    private void NotifyUpdated()
    {
        _updated?.OnNext(Unit.Default);
        if (Invalidated != null)
        {
            Dispatcher.UIThread.Post(() => Invalidated?.Invoke(this, EventArgs.Empty));
        }
    }

}
