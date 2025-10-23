using Dorisoy.Pan.ViewModels;

namespace Dorisoy.Pan.Player;

public partial class YampView : ReactiveUserControl<YampViewModel>
{
    private static YampView? _this;
    public static YampToolView? ControlsView;
    public VideoView2? _videoViewer;
    public string? videoUrl { get; set; }
    public string? coverUrl { get; set; }
    public string? videoDuration { get; set; }
    public string? videoTitle { get; set; }
    public int videoWidth { get; set; }
    public int videoHeight { get; set; }
    public string videoAspectRatio { get; set; }

    public YampView()
    {
        this.InitializeComponent();

        _this = this;

        ViewModel = new YampViewModel();

        DataContext = ViewModel;

        //��ƵԤ��
        _videoViewer = this.Get<VideoView2>("VideoViewer");

        this.WhenActivated(disposable => { });
    }

    public static YampView GetInstance()
    {
        return _this;
    }

    /// <summary>
    /// ���ò��ž��
    /// </summary>
    public void SetPlayerHandle()
    {
        if (_videoViewer != null && ViewModel.MediaPlayer != null)
        {
            _videoViewer.MediaPlayer = ViewModel.MediaPlayer;
            _videoViewer.MediaPlayer.Hwnd = _videoViewer.hndl.Handle;
            ViewModel.IsStopped = true;
        }
    }

    /// <summary>
    /// ��ʼ��������
    /// </summary>
    private void VideoPlayerViewControl_Init()
    {
        ControlsView = YampToolView.GetInstance();

        //ControlsView.timeSlider.Maximum = 100.0;
        //ControlsView.timeSlider.Minimum = 0.0;
        //ControlsView.timeSlider.Value = 0.0;

        ControlsView.volumeSlider.Value = 50.0;
        ControlsView.ViewModel.XVolume = 50.0;
        ControlsView.ViewModel.XTime = 0.0;
    }

    public void VideoPlayerViewControl_Play()
    {
        if (null != videoUrl)
        {
            //��ʼ��������
            VideoPlayerViewControl_Init();
            ControlsView.ViewModel.VideoDurationString = videoDuration;
            ControlsView.ViewModel.VideoTitle = videoTitle;
            ViewModel.StartPlay(videoUrl, coverUrl);
        }
    }
}
