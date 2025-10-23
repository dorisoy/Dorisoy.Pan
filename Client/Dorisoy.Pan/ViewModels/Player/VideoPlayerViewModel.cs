using Path = System.IO.Path;

namespace Dorisoy.Pan.ViewModels;


//[View(typeof(VideoPlayerView))]
public class VideoPlayerViewModel : ViewModelBase
{
    private static HttpClient httpClient = new();
    private readonly LibVLC _libVLC;
    public MediaPlayer MediaPlayer { get; }

    // Properties

    private Bitmap _cover;
    private bool _isStopped;

    public VideoPlayerViewModel() : base()
    {
        if (!Avalonia.Controls.Design.IsDesignMode)
        {
            var libVlcDirectoryPath = Path.Combine(Environment.CurrentDirectory, "libvlc", Utilities.IsWin64() ? "win-x64" : "win-x86");

            LibVLCSharp.Shared.Core.Initialize(libVlcDirectoryPath);

            _libVLC = new LibVLC(enableDebugLogs: Settings.ISDEBUGGING, Settings.LibVlc_AdditionalOptions);

            if (Settings.ISDEBUGGING)
            {
                // _libVLC.Log += VlcLogger_Event;
            }


            //MediaPlayer = new MediaPlayer(_libVLC) { EnableHardwareDecoding = true };

            MediaPlayer = new MediaPlayer(_libVLC)
            {
                Fullscreen = true,
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
    /// VLClib logger event
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="l"></param>
    private void VlcLogger_Event(object sender, LogEventArgs l)
    {
        Debug.WriteLine(l.Message);
    }

    /// <summary>
    /// Event fired when media changes and when play starts after stop
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void MediaPlayer_VideoOut(object sender, MediaPlayerVoutEventArgs e)
    {
        IsStopped = false; // Needed to show player over still image

        MediaPlayer.Volume = (int)Math.Ceiling(VideoPlayerView.ControlsView.ViewModel.XVolume);
        //Debug.WriteLine($"*** VOUT *** => XVolume={VideoPlayerView.ControlsView.viewModel.XVolume} MPvolume={MediaPlayer.Volume}");
    }

    /// <summary>
    /// Event fired when MediaPlayer reaches the end of media
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void MediaPlayer_EndReached(object sender, EventArgs e)
    {
        var t = new Thread(VideoPlayerView.ControlsView.ViewModel.Stop);
        t.Start();
    }

    /// <summary>
    /// Event fired once when MediaPlayer starts playing media
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void MediaPlayer_Playing(object sender, EventArgs e)
    {
        try
        {
            MediaPlayer.Scale = 0;

            VideoPlayerView.ControlsView.ViewModel.VideoDuration = MediaPlayer.Length / 1000;
        }
        catch { }
    }

    /// <summary>
    /// Event fired at every timechange while playing
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void MediaPlayer_TimeChanged(object sender, MediaPlayerTimeChangedEventArgs e)
    {
        try
        {
            MediaPlayer.Volume = (int)Math.Ceiling(VideoPlayerView.ControlsView.ViewModel.XVolume);
            VideoPlayerView.ControlsView.ViewModel.XTime = MediaPlayer.Time / 1000.0;
        }
        catch { VideoPlayerView.ControlsView.ViewModel.XTime = 0.0; }
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


    public void StartPlay(string ephemeralUrl, string coverUrl)
    {
        var thread = new Thread(() => LoadCover(coverUrl));
        thread.Start();

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


            VideoPlayerView.ControlsView.ViewModel.IsPlaying = true;

            MediaPlayer.Volume = (int)Math.Ceiling(VideoPlayerView.ControlsView.ViewModel.XVolume);

            var eq = new Equalizer();
            Debug.WriteLine($"EQUALIZER BANDS:{eq.BandCount}");

            MediaPlayer.SetEqualizer(eq);
            eq.Dispose();

            MediaPlayer.Play(media);
            media.Dispose();


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
        //Initialize a file stream to read the image file
        var fs = new FileStream(imageName, FileMode.Open, FileAccess.Read);

        //Initialize a byte array with size of stream
        var imgByteArr = new byte[fs.Length];

        //Read data from the file stream and put into the byte array
        fs.Read(imgByteArr, 0, Convert.ToInt32(fs.Length));

        //Close a file stream
        fs.Close();

        return imgByteArr;
    }

    public void PlayerIsExiting()
    {
        VideoPlayerView.ControlsView.ViewModel.Stop();
        //Dispose();
    }

    protected override void DisposeUnmanaged()
    {
        MediaPlayer?.Dispose();
        _libVLC?.Dispose();
    }

}
