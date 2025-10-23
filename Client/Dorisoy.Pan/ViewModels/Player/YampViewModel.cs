using Path = System.IO.Path;

namespace Dorisoy.Pan.ViewModels;

//[View(typeof(YampView))]
public class YampViewModel : ViewModelBase
{
    private static HttpClient httpClient = new();
    private readonly LibVLC _libVLC;
    public MediaPlayer MediaPlayer { get; }
    private Bitmap _cover;
    private bool _isStopped;

    public YampViewModel()
    {
        if (!Avalonia.Controls.Design.IsDesignMode)
        {
            var libVlcDirectoryPath = Path.Combine(Environment.CurrentDirectory, "libvlc", Utilities.IsWin64() ? "win-x64" : "win-x86");

            LibVLCSharp.Shared.Core.Initialize(libVlcDirectoryPath);

            _libVLC = new LibVLC(enableDebugLogs: Settings.ISDEBUGGING, Settings.LibVlc_AdditionalOptions);

            //if (Settings.ISDEBUGGING)
            //    _libVLC.Log += VlcLogger_Event;

            MediaPlayer = new MediaPlayer(_libVLC)
            {
                Fullscreen = true,
                //使用硬件加速
                //EnableHardwareDecoding = true
                //EnableMouseInput = false,
                //Scale = 1
            };

            MediaPlayer.TimeChanged += MediaPlayer_TimeChanged;
            MediaPlayer.Playing += MediaPlayer_Playing;
            MediaPlayer.EndReached += MediaPlayer_EndReached;
            MediaPlayer.Vout += MediaPlayer_VideoOut;

            IsStopped = false;

        }
    }

    /// <summary>
    /// VLClib 日志事件
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="l"></param>
    private void VlcLogger_Event(object sender, LogEventArgs l)
    {
        Debug.WriteLine(l.Message);
    }

    /// <summary>
    /// 媒体更改和停止后开始播放时触发的事件
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void MediaPlayer_VideoOut(object sender, MediaPlayerVoutEventArgs e)
    {
        Dispatcher.UIThread.InvokeAsync(() =>
        {
            IsStopped = false;
            MediaPlayer.Volume = (int)Math.Ceiling(YampView.ControlsView.ViewModel.XVolume);
        });
    }

    /// <summary>
    /// MediaPlayer到达媒体末尾时激发的事件
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void MediaPlayer_EndReached(object sender, EventArgs e)
    {
        Dispatcher.UIThread.InvokeAsync(() =>
        {
            YampView.ControlsView.ViewModel.Stop();
        });
    }

    /// <summary>
    /// MediaPlayer开始播放媒体时触发一次事件
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void MediaPlayer_Playing(object sender, EventArgs e)
    {
        Dispatcher.UIThread.InvokeAsync(() =>
        {
            try
            {
                //视频持续时间
                MediaPlayer.Scale = 0;
                YampView.ControlsView.ViewModel.VideoDuration = MediaPlayer.Length / 1000;
            }
            catch { }

        });
    }

    /// <summary>
    /// 播放中每次更改时间时触发的事件
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void MediaPlayer_TimeChanged(object sender, MediaPlayerTimeChangedEventArgs e)
    {
        Dispatcher.UIThread.InvokeAsync(() =>
        {
            try
            {
                MediaPlayer.Volume = (int)Math.Ceiling(YampView.ControlsView.ViewModel.XVolume);
                YampView.ControlsView.ViewModel.XTime = MediaPlayer.Time / 1000.0;
            }
            catch
            {
                //System.InvalidOperationException:“Call from invalid thread
                YampView.ControlsView.ViewModel.XTime = 0.0;
            }
        });
    }

    #region PROPERTIES

    public Bitmap Cover
    {
        get => _cover;
        private set => this.RaiseAndSetIfChanged(ref _cover, value);
    }

    public bool IsStopped
    {
        get => _isStopped;
        set => this.RaiseAndSetIfChanged(ref _isStopped, value);
    }

    #endregion

    protected override void DisposeUnmanaged()
    {
        MediaPlayer?.Dispose();
        _libVLC?.Dispose();
    }

    public void StartPlay(string ephemeralUrl, string coverUrl)
    {
        try
        {
            //var currentDirectory = Settings.ApplicationFolder();
            //var destination = Path.Combine(currentDirectory, "record.ts");

            using var media = new Media(_libVLC, new Uri(ephemeralUrl)
                //,
                //":sout=#file{dst=" + destination + "}",
                //":sout-keep"                    
                , Settings.Media_AdditionalOptions
                );


            YampView.ControlsView.ViewModel.IsPlaying = true;

            //MediaPlayer.Volume = (int)Math.Ceiling(YampView.ControlsView.ViewModel.XVolume);

            var eq = new Equalizer();
            Debug.WriteLine($"EQUALIZER BANDS:{eq.BandCount}");

            MediaPlayer.SetEqualizer(eq);
            eq.Dispose();

            MediaPlayer.Play(media);
            media?.Dispose();

        }
        catch { }
    }

    private async void LoadCover(string coverUrl)
    {
        if (!string.IsNullOrWhiteSpace(coverUrl))
        {
            using (var imageStream = await LoadCoverBitmapAsync(coverUrl))
            {
                Cover = Bitmap.DecodeToWidth(imageStream, 400);
            }
        }
    }

    private async Task<Stream> LoadCoverBitmapAsync(string coverUrl)
    {
        byte[] data;
        try
        {
            data = await httpClient.GetByteArrayAsync(coverUrl);
        }
        catch (Exception ex)
        {
            var assemblyPath = Utilities.ApplicationFolder();
            data = ImageToByteArray(Path.Combine(assemblyPath, Settings.WeTubeImageNotAvailable));
        }

        return new MemoryStream(data);

    }
    private static byte[] ImageToByteArray(string imageName)
    {
        //初始化文件流以读取图像文件
        var fs = new FileStream(imageName, FileMode.Open, FileAccess.Read);

        //初始化流大小的字节数组
        var imgByteArr = new byte[fs.Length];

        //从文件流中读取数据并放入字节数组
        fs.Read(imgByteArr, 0, Convert.ToInt32(fs.Length));

        //关闭文件流
        fs.Close();

        return imgByteArr;
    }

    /// <summary>
    /// 播放退出
    /// </summary>
    public void PlayerIsExiting()
    {
        YampView.ControlsView.ViewModel.Stop();
        //Dispose();
    }
}
