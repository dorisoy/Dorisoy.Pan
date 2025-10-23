using Avalonia.Controls.Primitives;
using Image = Avalonia.Controls.Image;

namespace Dorisoy.Pan.Player;


/// <summary>
/// 视频预览器
/// </summary>
public class VideoView : ContentControl, IVideoView
{
    static VideoView()
    {
        MediaPlayerProperty.Changed.AddClassHandler<VideoView>((v, e) => v.InitMediaPlayer());
    }

    public VideoView()
    {
        VlcRenderingOptions = LibVLCAvaloniaRenderingOptions.AvaloniaCustomDrawingOperation;
    }

    private VlcVideoSourceProvider _provider = new VlcVideoSourceProvider();
    private Image PART_Image;
    private NativeVideoPresenter PART_NativeHost;
    private bool _templateApplied;


    public static readonly DirectProperty<VideoView, MediaPlayer> MediaPlayerProperty =
        AvaloniaProperty.RegisterDirect<VideoView, MediaPlayer>(nameof(MediaPlayer), v => v.MediaPlayer, (s, v) => s.MediaPlayer = v);

    private MediaPlayer _mediaPlayer;
    public MediaPlayer MediaPlayer
    {
        get => _mediaPlayer;
        set => SetAndRaise(MediaPlayerProperty, ref _mediaPlayer, value);
    }

    public static readonly DirectProperty<VideoView, bool> DisplayRenderStatsProperty =
         AvaloniaProperty.RegisterDirect<VideoView, bool>(nameof(DisplayRenderStats), v => v.DisplayRenderStats, (s, v) => s.DisplayRenderStats = v);

    private bool _displayRenderStats;
    public bool DisplayRenderStats
    {
        get => _displayRenderStats;
        set => SetAndRaise(DisplayRenderStatsProperty, ref _displayRenderStats, value);
    }

    public static readonly StyledProperty<LibVLCAvaloniaRenderingOptions> VlcRenderingOptionsProperty =
            AvaloniaProperty.Register<VideoView, LibVLCAvaloniaRenderingOptions>(nameof(VlcRenderingOptions));

    public LibVLCAvaloniaRenderingOptions VlcRenderingOptions
    {
        get => GetValue(VlcRenderingOptionsProperty);
        set => SetValue(VlcRenderingOptionsProperty, value);
    }


    /// <summary>
    /// 在应用模板时
    /// </summary>
    /// <param name="e"></param>
    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);

        PART_Image = e.NameScope.Find<Image>("PART_RenderImage");
        PART_NativeHost = e.NameScope.Find<NativeVideoPresenter>("PART_NativeHost");

        _templateApplied = true;

        if (VlcRenderingOptions != LibVLCAvaloniaRenderingOptions.VlcNative)
        {
            if (PART_Image is VLCImageRenderer vb)
            {
                vb.SourceProvider = _provider;
                vb.UseCustomDrawingOperation = VlcRenderingOptions == LibVLCAvaloniaRenderingOptions.AvaloniaCustomDrawingOperation;

                _provider.Display
                    .ObserveOn(RxApp.MainThreadScheduler)
                    .Subscribe(b =>
                    {
                        b?.Read((bitmap) =>
                        {
                            vb.Source = bitmap;
                        });
                    });
            }
            else
            {
                PART_Image.Bind(Image.SourceProperty, _provider.Display);
            }
        }

        InitMediaPlayer();
    }

    private IDisposable _playerEvents;


    /// <summary>
    /// 初始MediaPlayer播放器
    /// </summary>
    /// <exception cref="NotSupportedException"></exception>
    private void InitMediaPlayer()
    {
        if (!Design.IsDesignMode && _templateApplied)
        {
            _playerEvents?.Dispose();
            _playerEvents = null;

            if (MediaPlayer.IsPlaying)
                throw new NotSupportedException("播放器应在初始化过程中停止!");

            if (VlcRenderingOptions != LibVLCAvaloniaRenderingOptions.VlcNative)
            {
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                    MediaPlayer.Hwnd = IntPtr.Zero;
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                    MediaPlayer.NsObject = IntPtr.Zero;
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                    MediaPlayer.XWindow = 0;

                _provider.Init(MediaPlayer);

                _playerEvents = Observable.FromEventPattern(MediaPlayer, nameof(MediaPlayer.Playing))
                    .ObserveOn(RxApp.MainThreadScheduler)
                    .Subscribe(_ =>
                    {
                        if (PART_Image is VLCImageRenderer vb)
                        {
                            vb.ResetStats();
                        }
                    });
            }
            else
            {
                PART_NativeHost?.UpdatePlayerHandle(MediaPlayer);
            }
        }
    }
}


/// <summary>
/// MediaPlayer 扩展方法
/// </summary>
public static class MediaPlayerExtensions
{
    /// <summary>
    /// 处理释放
    /// </summary>
    /// <param name="player"></param>
    public static void DisposeHandle(this MediaPlayer player)
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            player.Hwnd = IntPtr.Zero;
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            player.XWindow = 0;
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            player.NsObject = IntPtr.Zero;
    }

    /// <summary>
    /// 设置句柄
    /// </summary>
    /// <param name="player"></param>
    /// <param name="handle"></param>
    public static void SetHandle(this MediaPlayer player, IPlatformHandle handle)
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            player.Hwnd = handle.Handle;
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            player.XWindow = (uint)handle.Handle;
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            player.NsObject = handle.Handle;
    }
}
