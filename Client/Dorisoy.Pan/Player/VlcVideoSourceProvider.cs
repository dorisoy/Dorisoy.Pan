using PixelFormat = Avalonia.Platform.PixelFormat;
using VLC = LibVLCSharp.Shared;

namespace Dorisoy.PanClient.Player;

/// <summary>
/// 用于表示VideoSource 提供器
/// </summary>
public class VlcVideoSourceProvider : ReactiveObject, IDisposable
{
    private IntPtr _buffer;
    private object[] _callbacks;
    private PixelSize _formatSize;
    private PixelFormat _pixelFormat;
    private uint _pixelFormatPixelSize;

    /// <summary>
    /// 表示视频写入源
    /// </summary>
    private VlcSharpWriteableBitmap _videoSource;
    /// <summary>
    /// 表示视频的图像源
    /// </summary>
    public Bitmap VideoSource => _videoSource;


    private ISubject<VlcSharpWriteableBitmap> _display = new Subject<VlcSharpWriteableBitmap>();
    public IObservable<VlcSharpWriteableBitmap> Display => _display;

    /// <summary>
    /// VLC播放器
    /// </summary>
    public VLC.MediaPlayer MediaPlayer { get; private set; }

    public Action<Bitmap> Rander { get; set; }

    private static void ToFourCC(string fourCCString, IntPtr destination)
    {
        if (fourCCString.Length != 4)
        {
            throw new ArgumentException("4CC codes must be 4 characters long", nameof(fourCCString));
        }

        var bytes = Encoding.ASCII.GetBytes(fourCCString);

        for (var i = 0; i < 4; i++)
        {
            Marshal.WriteByte(destination, i, bytes[i]);
        }
    }

    /// <summary>
    /// 初始化
    /// </summary>
    /// <param name="player"></param>
    public void Init(VLC.MediaPlayer player)
    {
        MediaPlayer = player;

        //初始位图实例
        _videoSource = new VlcSharpWriteableBitmap(new PixelSize(1920, 1080), new Vector(96, 96), PixelFormat.Rgba8888);

        //注册视频源无效时触发事件回调
        //_videoSource.Invalidated += _videoSource_Invalidated;

        var c = new VLC.MediaPlayer.LibVLCVideoCleanupCb(CleanupCallback);
        var f = new VLC.MediaPlayer.LibVLCVideoFormatCb(VideoFormatCallback);

        //设置解码视频的色度和尺寸。这只能与libvlc_video_set_callbacks（）结合使用。
        MediaPlayer.SetVideoFormatCallbacks(f, c);

        var lv = new VLC.MediaPlayer.LibVLCVideoLockCb(LockVideo);
        var uv = new VLC.MediaPlayer.LibVLCVideoUnlockCb(UnlockVideo);
        var d = new VLC.MediaPlayer.LibVLCVideoDisplayCb(DisplayVideo);

        //视频控制回调
        MediaPlayer.SetVideoCallbacks(lv, uv, d);

        _callbacks = new object[] { c, f, lv, uv, d };
    }

    private void _videoSource_Invalidated(object sender, EventArgs e)
    {
        var vsb = sender as VlcSharpWriteableBitmap;
        vsb.Read((bitmap) =>
        {
            System.Diagnostics.Debug.WriteLine($"Invalidated---------->{bitmap.Size}");
            Rander.Invoke(bitmap);
        });
    }


    /// <summary>
    /// 移除视频源
    /// </summary>
    private void CleanUp()
    {
        _videoSource?.Clear();
        _display.OnNext(null);
    }

    /// <summary>
    /// 将维度与mod的下一个倍数对齐
    /// </summary>
    /// <param name="dimension">The dimension to be aligned</param>
    /// <param name="mod">The modulus</param>
    /// <returns>The aligned dimension</returns>
    private uint GetAlignedDimension(uint dimension, uint mod)
    {
        var modResult = dimension % mod;
        if (modResult == 0)
        {
            return dimension;
        }

        return dimension + mod - (dimension % mod);
    }

    #region Vlc 回调接口

    /// <summary>
    /// Called by Vlc when it requires a cleanup
    /// </summary>
    /// <param name="opaque">The parameter is not used</param>
    private void CleanupCallback(ref IntPtr opaque)
    {
        Marshal.FreeHGlobal(_buffer);

        if (!_disposed)
        {
            CleanUp();
        }
    }

    //
    // Summary:
    //     回调原型以显示图片.
    //
    // Parameters:
    //   opaque:
    //    传递到libvlc_video_set_callbacks（）的私有指针[IN]
    //
    //   picture:
    //     private pointer returned from the
    //
    // Remarks:
    //     当需要显示视频帧时，由媒体播放时钟决定，调用显示回调.
    //     callback [IN]
    private void DisplayVideo(IntPtr opaque, IntPtr picture)
    {
        _videoSource.Write(_formatSize, new Vector(96, 96), _pixelFormat, fb =>
        {
            unsafe
            {
                long size = fb.Size.Width * fb.Size.Height * 4;
                Buffer.MemoryCopy((void*)opaque, (void*)fb.Address, size, size);
            }
        });

        _display.OnNext(_videoSource);

        ////读取
        //var filebyte = ReadFile("C:\\Users\\Administrator\\Desktop\\Dorisoy\\20230627195449.bmp");
        //////转换成位图
        //using var stream = new MemoryStream(filebyte);
        //var cbitmap = Bitmap.DecodeToWidth(stream, 300, BitmapInterpolationMode.HighQuality);

        //Rander.Invoke(cbitmap);



        //System.Diagnostics.Debug.WriteLine($"_videoSource---------->{_videoSource.Size}");

        //我们可以等待位图渲染 ???
        //_videoSource.Rendered.FirstAsync().Timeout(TimeSpan.FromMilliseconds(10)).Wait();
    }

    /// <summary>Callback prototype to allocate and lock a picture buffer.</summary>
    /// <param name="opaque">private pointer as passed to libvlc_video_set_callbacks() [IN]</param>
    /// <param name="planes">
    /// <para>start address of the pixel planes (LibVLC allocates the array</para>
    /// <para>of void pointers, this callback must initialize the array) [OUT]</para>
    /// </param>
    /// <returns>
    /// <para>a private pointer for the display and unlock callbacks to identify</para>
    /// <para>the picture buffers</para>
    /// </returns>
    /// <remarks>
    /// <para>Whenever a new video frame needs to be decoded, the lock callback is</para>
    /// <para>invoked. Depending on the video chroma, one or three pixel planes of</para>
    /// <para>adequate dimensions must be returned via the second parameter. Those</para>
    /// <para>planes must be aligned on 32-bytes boundaries.</para>
    /// </remarks>
    private IntPtr LockVideo(IntPtr opaque, IntPtr planes)
    {
        Marshal.WriteIntPtr(planes, opaque);
        return opaque;
    }

    /// <summary>Callback prototype to unlock a picture buffer.</summary>
    /// <param name="opaque">private pointer as passed to libvlc_video_set_callbacks() [IN]</param>
    /// <param name="picture">private pointer returned from the</param>
    /// <param name="planes">pixel planes as defined by the</param>
    /// <remarks>
    /// <para>When the video frame decoding is complete, the unlock callback is invoked.</para>
    /// <para>This callback might not be needed at all. It is only an indication that the</para>
    /// <para>application can now read the pixel values if it needs to.</para>
    /// <para>A picture buffer is unlocked after the picture is decoded,</para>
    /// <para>but before the picture is displayed.</para>
    /// <para>callback [IN]</para>
    /// <para>callback (this parameter is only for convenience) [IN]</para>
    /// </remarks>
    private void UnlockVideo(IntPtr opaque, IntPtr picture, IntPtr planes)
    {
    }

    /// <summary>
    /// <para>Callback prototype to configure picture buffers format.</para>
    /// <para>This callback gets the format of the video as output by the video decoder</para>
    /// <para>and the chain of video filters (if any). It can opt to change any parameter</para>
    /// <para>as it needs. In that case, LibVLC will attempt to convert the video format</para>
    /// <para>(rescaling and chroma conversion) but these operations can be CPU intensive.</para>
    /// </summary>
    /// <param name="opaque">
    /// <para>pointer to the private pointer passed to</para>
    /// <para>libvlc_video_set_callbacks() [IN/OUT]</para>
    /// </param>
    /// <param name="chroma">pointer to the 4 bytes video format identifier [IN/OUT]</param>
    /// <param name="width">pointer to the pixel width [IN/OUT]</param>
    /// <param name="height">pointer to the pixel height [IN/OUT]</param>
    /// <param name="pitches">
    /// <para>table of scanline pitches in bytes for each pixel plane</para>
    /// <para>(the table is allocated by LibVLC) [OUT]</para>
    /// </param>
    /// <param name="lines">table of scanlines count for each plane [OUT]</param>
    /// <returns>the number of picture buffers allocated, 0 indicates failure</returns>
    /// <remarks>
    /// <para>For each pixels plane, the scanline pitch must be bigger than or equal to</para>
    /// <para>the number of bytes per pixel multiplied by the pixel width.</para>
    /// <para>Similarly, the number of scanlines must be bigger than of equal to</para>
    /// <para>the pixel height.</para>
    /// <para>Furthermore, we recommend that pitches and lines be multiple of 32</para>
    /// <para>to not break assumptions that might be held by optimized code</para>
    /// <para>in the video decoders, video filters and/or video converters.</para>
    /// </remarks>
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

    #region IDisposable支持

    private bool _disposed = false;

    /// <summary>
    /// The destructor
    /// </summary>
    ~VlcVideoSourceProvider()
    {
        Dispose(false);
    }

    /// <inheritdoc />
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Disposes the control.
    /// </summary>
    /// <param name="disposing">The parameter is not used.</param>
    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            _videoSource?.Dispose();
            _videoSource = null;
            _disposed = true;
            MediaPlayer = null;
            CleanUp();
        }
    }

    #endregion IDisposable Support
}
