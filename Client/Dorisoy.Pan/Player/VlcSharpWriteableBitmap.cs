using Disposable = System.Reactive.Disposables.Disposable;
using PixelFormat = Avalonia.Platform.PixelFormat;
using Size = Avalonia.Size;

namespace Dorisoy.Pan.Player;


public class VlcSharpWriteableBitmap : WriteableBitmap //Bitmap
{
    private bool _disposed;
    private object _lockRead = new object();
    private object _lockWrite = new object();
    private PixelFormat? _pixelFormat;

    private ISubject<Unit> _rendered = new Subject<Unit>();
    private ISubject<Unit> _updated = new Subject<Unit>();

    private WriteableBitmap _read;
    private WriteableBitmap _write;

    public VlcSharpWriteableBitmap(PixelSize size, Vector dpi, PixelFormat? format = null, AlphaFormat? alphaFormat = null) : base(size, dpi, format, alphaFormat)
    {

    }

    /// <summary>
    /// 分辨率
    /// </summary>
    public new Vector Dpi => GetValueSafe(x => x.Dpi);

    /// <summary>
    /// 设备像素为单位的大小
    /// </summary>
    public new PixelSize PixelSize => GetValueSafe(x => x.PixelSize, new PixelSize(1920, 1080));

    /// <summary>
    /// 尺寸
    /// </summary>
    public new Size Size => GetValueSafe(x => x.Size);

    public IObservable<Unit> Rendered => _rendered;
    public IObservable<Unit> Updated => _updated;

    /// <summary>
    /// 无效时触发事件
    /// </summary>
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

    public void Render(ImmediateDrawingContext context, double opacity, Rect sourceRect, Rect destRect, BitmapInterpolationMode bitmapInterpolationMode = BitmapInterpolationMode.Unspecified)
    {
        var b = _read.CreateScaledBitmap(new PixelSize(1920, 1080));
        Read(b => context.DrawBitmap(b, destRect));
        NotifyRendered();
    }


    public new void Save(string fileName, int? quality = null) => Read(b => b.Save(fileName));

    public new void Save(Stream stream, int? quality = null) => Read(b => b.Save(stream));


    /// <summary>
    /// 写入位图
    /// </summary>
    /// <param name="size"></param>
    /// <param name="dpi"></param>
    /// <param name="format"></param>
    /// <param name="action"></param>
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
            throw new ObjectDisposedException(nameof(VlcSharpWriteableBitmap));

        using (LockRead())
        {
            return _read == null ? defaultvalue : getter(_read);
        }
    }

    private IDisposable Lock(object lockObject)
    {
        Monitor.Enter(lockObject);
        return Disposable.Create(() => Monitor.Exit(lockObject));
    }

    private IDisposable LockRead() => Lock(_lockRead);

    private IDisposable LockWrite() => Lock(_lockWrite);

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
