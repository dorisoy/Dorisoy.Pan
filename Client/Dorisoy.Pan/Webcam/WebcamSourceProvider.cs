using PixelFormat = Avalonia.Platform.PixelFormat;

namespace Dorisoy.Pan.Webcam;

/// <summary>
/// 可以提供Avalonia图像源以显示视频的类
/// </summary>
public class WebcamSourceProvider : IDisposable
{
    private IntPtr _buffer;
    private object[] _callbacks;
    private PixelSize _formatSize;
    private PixelFormat _pixelFormat;
    private uint _pixelFormatPixelSize;

    /// <summary>
    /// 用于绑定的视频资源源
    /// </summary>
    private WebcamVlcSharpWriteableBitmap _videoSource;
    public WebcamStreamingPlayer PipPlayer { get; private set; }
    private ISubject<Bitmap> _source = new Subject<Bitmap>();
    public IObservable<Bitmap> Source => _source;

    /// <summary>
    /// 表示视频的图像源
    /// </summary>
    public Bitmap VideoSource => _videoSource;

    private static void ToFourCC(string fourCCString, IntPtr destination)
    {
        if (fourCCString.Length != 4)
        {
            throw new ArgumentException("4CC代码的长度必须为4个字符", nameof(fourCCString));
        }

        var bytes = Encoding.ASCII.GetBytes(fourCCString);
        for (var i = 0; i < 4; i++)
        {
            Marshal.WriteByte(destination, i, bytes[i]);
        }
    }

    public void Init(WebcamStreamingPlayer player)
    {
        PipPlayer = player;
    }

    /// <summary>
    /// 移除视频源
    /// </summary>
    private void CleanUp()
    {
        _videoSource?.Clear();
        _source.OnNext(null);
    }

    private uint GetAlignedDimension(uint dimension, uint mod)
    {
        var modResult = dimension % mod;
        if (modResult == 0)
        {
            return dimension;
        }

        return dimension + mod - (dimension % mod);
    }

    #region Vlc video callbacks

    private void CleanupCallback(ref IntPtr opaque)
    {
        Marshal.FreeHGlobal(_buffer);

        if (!_disposed)
        {
            CleanUp();
        }
    }

    private void DisplayVideo(IntPtr opaque, IntPtr picture)
    {
        _videoSource.Write(_formatSize, new Avalonia.Vector(96, 96), _pixelFormat,
            fb =>
            {
                unsafe
                {
                    long size = fb.Size.Width * fb.Size.Height * 4;
                    Buffer.MemoryCopy((void*)opaque, (void*)fb.Address, size, size);
                }
            });

        _source.OnNext(_videoSource);

        //we can wait bitmap to render ???
        //_videoSource.Rendered.FirstAsync().Timeout(TimeSpan.FromMilliseconds(10)).Wait();
    }


    private IntPtr LockVideo(IntPtr opaque, IntPtr planes)
    {
        Marshal.WriteIntPtr(planes, opaque);
        return opaque;
    }

    private void UnlockVideo(IntPtr opaque, IntPtr picture, IntPtr planes)
    {
    }

    private uint VideoFormatCallback(ref IntPtr opaque, IntPtr chroma, ref uint width, ref uint height, ref uint pitches, ref uint lines)
    {
        _pixelFormat = PixelFormat.Bgra8888;
        _pixelFormatPixelSize = 4;
        _formatSize = new PixelSize((int)width, (int)height);

        ToFourCC("BGRA", chroma);
        //or ToFourCC("RV32", chroma);

        pitches = GetAlignedDimension(width * _pixelFormatPixelSize, 32);
        lines = GetAlignedDimension(height, 32);

        var size = pitches * lines;

        opaque = _buffer = Marshal.AllocHGlobal((int)size);
        return 1;
    }

    #endregion Vlc video callbacks

    #region IDisposable Support

    private bool _disposed = false;

    ~WebcamSourceProvider()
    {
        Dispose(false);
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            _videoSource?.Dispose();
            _videoSource = null;
            _disposed = true;
            PipPlayer = null;
            CleanUp();
        }
    }

    #endregion IDisposable Support





}
