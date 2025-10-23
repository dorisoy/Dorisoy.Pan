using Dorisoy.Pan.ViewModels;

namespace Dorisoy.Pan.Player;

public partial class VideoPlayerView : ReactiveUserControl<VideoPlayerViewModel>
{
    //public VideoPlayerViewControlModel viewModel = new VideoPlayerViewControlModel();

    static VideoPlayerView _this;

    //public static ControlsPanelView? ControlsView = new ControlsPanelView();
    public static VideoPlayerToolView? ControlsView;

    //public Panel mpContainer;

    public VideoView? _videoViewer;


    public string videoUrl { get; set; }
    public string coverUrl { get; set; }
    public string videoDuration { get; set; }
    public string videoTitle { get; set; }
    public int videoWidth { get; set; }
    public int videoHeight { get; set; }

    public string videoAspectRatio { get; set; }
    public VideoPlayerView()
    {
        this.InitializeComponent();

        _this = this;

        DataContext = ViewModel;

        _videoViewer = this.Get<VideoView>("VideoViewer");

    }

    private void InitializeComponent()
    {
        this.InitializeComponent();
    }

    public static VideoPlayerView GetInstance()
    {
        return _this;
    }

    public void SetPlayerHandle()
    {
        if (_videoViewer != null && ViewModel.MediaPlayer != null)
        {
            //_videoViewer.MediaPlayer = viewModel.MediaPlayer;
            //_videoViewer.MediaPlayer.Hwnd = _videoViewer.hndl.Handle;

            ViewModel.IsStopped = true;
        }

    }


    private void VideoPlayerViewControl_Init()
    {

        ControlsView = VideoPlayerToolView.GetInstance();

        ControlsView.timeSlider.Maximum = 100.0;
        ControlsView.timeSlider.Minimum = 0.0;
        ControlsView.timeSlider.Value = 0.0;
        ControlsView.volumeSlider.Value = 50.0;

        //ControlsView.ViewModel.XVolume = 50.0;
        //ControlsView.ViewModel.XTime = 0.0;

    }

    public void VideoPlayerViewControl_Play()
    {
        if (null != videoUrl)
        {
            VideoPlayerViewControl_Init();

            ControlsView.ViewModel.VideoDurationString = videoDuration;
            ControlsView.ViewModel.VideoTitle = videoTitle;

            ViewModel.StartPlay(videoUrl, coverUrl);
        }
    }
}
