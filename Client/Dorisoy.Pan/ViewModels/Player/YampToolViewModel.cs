namespace Dorisoy.PanClient.ViewModels;

//[View(typeof(YampToolView))]
public class YampToolViewModel : ViewModelBase
{
    /// <summary>
    /// 监视器时间统计
    /// </summary>
    private System.Timers.Timer timer;
    private System.DateTime TimeNow = new DateTime();
    private TimeSpan TimeCount = new TimeSpan();


    private bool _isPlaying;

    private bool _isLoading;
    private double _volume;
    private double _duration;
    private double _instantTimeValue;

    private string _title;
    private string _durationString;

    public YampToolViewModel()
    {
        if (timer == null)
        {
            timer = new() { Interval = 1 };
            timer.Elapsed += timer_Elapsed;
        }

        _title = "No media selected";
        _durationString = "--:--:--:--";

        timer.Start();
    }


    /// <summary>
    /// 计时器
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
    {
        Dispatcher.UIThread.InvokeAsync(new Action(() =>
        {
            try
            {
                TimeCount = DateTime.Now - TimeNow;
                VideoDurationString = string.Format("{0:00}:{1:00}:{2:00}:{3:000}", TimeCount.Hours, TimeCount.Minutes, TimeCount.Seconds, TimeCount.Milliseconds);

            }
            catch (Exception) { }
        }));
    }


    #region PROPERTIES


    public string VideoTitle
    {
        get => _title;
        set => this.RaiseAndSetIfChanged(ref _title, value);
    }

    /// <summary>
    /// 视频持续时间
    /// </summary>
    public string VideoDurationString
    {
        get => _durationString;
        set => this.RaiseAndSetIfChanged(ref _durationString, value);
    }

    /// <summary>
    /// 视频持续时间
    /// </summary>
    public double VideoDuration
    {
        get => _duration;
        set => this.RaiseAndSetIfChanged(ref _duration, value);
    }

    public double XTime
    {
        get => _instantTimeValue;
        set => this.RaiseAndSetIfChanged(ref _instantTimeValue, value);
    }

    public bool IsPlaying
    {
        get => _isPlaying;
        set => this.RaiseAndSetIfChanged(ref _isPlaying, value);
    }

    public double XVolume
    {
        get => _volume;
        set => this.RaiseAndSetIfChanged(ref _volume, value);
    }

    public bool IsLoading
    {
        get => _isLoading;
        set => this.RaiseAndSetIfChanged(ref _isLoading, value);
    }

    #endregion

    public void Rewind()
    {
        try
        {
            var tmp = YampToolView.GetInstance();

            if (tmp.playerViewModel != null)
            {
                var mediaPlayer = tmp.playerViewModel.MediaPlayer;
                if (mediaPlayer == null)
                    return;

                mediaPlayer.Time -= Settings.SEEK_OFFSET;
            }
        }
        catch { }
    }

    public void Seek()
    {
        try
        {
            var tmp = YampToolView.GetInstance();
            if (tmp.playerViewModel != null)
            {
                var mediaPlayer = tmp.playerViewModel.MediaPlayer;
                if (mediaPlayer == null)
                    return;
                mediaPlayer.Time += Settings.SEEK_OFFSET;
            }
        }
        catch { }

    }
    public void Play()
    {
        try
        {
            IsPlaying = true;

            var tmp = YampToolView.GetInstance();
            if (tmp.playerViewModel != null)
            {
                tmp.playerViewModel.IsStopped = false;
                tmp.playerViewModel.MediaPlayer.Play();
            }

        }
        catch { }
    }

    public void ChangeTime(long time)
    {
        try
        {
            var tmp = YampToolView.GetInstance();
            if (tmp.playerViewModel != null)
            {
                var mediaPlayer = tmp.playerViewModel.MediaPlayer;
                if (mediaPlayer == null)
                    return;

                //XTime = time / 1000;
                mediaPlayer.Time = time;
            }

        }
        catch { }

    }

    public void Pause()
    {
        try
        {
            IsPlaying = false;
            var tmp = YampToolView.GetInstance();
            if (tmp.playerViewModel != null)
                tmp.playerViewModel?.MediaPlayer.Pause();
        }
        catch { }
    }

    public void Stop()
    {
        try
        {
            IsPlaying = false;

            XTime = 0.0;
            var tmp = YampToolView.GetInstance();
            if (tmp.playerViewModel != null)
            {
                tmp.playerViewModel.IsStopped = true;
                tmp.playerViewModel.MediaPlayer.Stop();
            }

        }
        catch { }
    }
}
