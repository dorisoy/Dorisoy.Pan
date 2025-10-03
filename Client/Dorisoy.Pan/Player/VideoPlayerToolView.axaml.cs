using Dorisoy.PanClient.ViewModels;

namespace Dorisoy.PanClient.Player;

public partial class VideoPlayerToolView : ReactiveUserControl<VideoPlayerToolViewModel>
{
    public Slider volumeSlider;
    public Slider timeSlider;
    public FlyoutPanel flyPanelContainer;

    public VideoPlayerViewModel? playerViewModel;
    static VideoPlayerToolView? _this;
    public VideoPlayerToolView()
    {
        this.InitializeComponent();

        volumeSlider = this.Get<Slider>("SliderVolume");
        timeSlider = this.Get<Slider>("SliderTime");
        //_full = this.Get<Button>("Full");

        flyPanelContainer = this.Get<FlyoutPanel>("ControlsPanel");


        DataContext = ViewModel;

        _this = this;

        //Opened += ControlsPanelViewControl_Opened;

        timeSlider.Value = 0.0;

        // Time & Volume sliders events
        timeSlider.AddHandler(PointerReleasedEvent, TimeSlider_PointerReleased, RoutingStrategies.Tunnel);
        timeSlider.AddHandler(PointerPressedEvent, TimeSlider_PointerPressed, RoutingStrategies.Tunnel);

        volumeSlider.AddHandler(PointerPressedEvent, VolumeSlider_PointerPressed, RoutingStrategies.Tunnel);
        volumeSlider.AddHandler(PointerReleasedEvent, VolumeSlider_PointerReleased, RoutingStrategies.Tunnel);


        PointerEntered += Controls_PointerEnter;
        PointerExited += Controls_PointerLeave;

        // Full button is disabled when the player's width < height (e.g. vertical videos)
        //_full.IsVisible = true;            
    }



    private void InitializeComponent()
    {
        this.InitializeComponent();
    }

    public void Controls_PointerEnter(object? sender, PointerEventArgs e)
    {
        //Debug.WriteLine("POINTER ENTER");
        //this.Opacity = 1;
        flyPanelContainer.IsOpen = true;
        ControlsPanelViewControl_Opened();
    }

    public void Controls_PointerLeave(object? sender, PointerEventArgs e)
    {
        //Debug.WriteLine("POINTER LEAVE");
        //this.Opacity = 0;
        flyPanelContainer.IsOpen = false;
    }

    public static VideoPlayerToolView GetInstance()
    {
        return _this;
    }


    //private void ControlsPanelViewControl_Opened(object? sender, EventArgs e)
    private void ControlsPanelViewControl_Opened()
    {
        try
        {
            var v = VideoPlayerView.GetInstance();
            if (v != null)
                playerViewModel = VideoPlayerView.GetInstance().ViewModel;
        }
        catch { }

        //flyPanelContainer.IsOpen = true;
        //Thread thread = new Thread(() => HideFlyPanel());
        //thread.Start();

    }

    //private async void HideFlyPanel()
    //{
    //    Thread.Sleep(1000);
    //    //await Dispatcher.UIThread.InvokeAsync(() => { flyPanelContainer.IsOpen = false; }).ConfigureAwait(false);

    //}


    private void TimeSlider_PointerPressed(object? sender, PointerPressedEventArgs e)
    {
        ViewModel.Pause();
    }
    private void TimeSlider_PointerReleased(object? sender, PointerReleasedEventArgs e)
    {
        Thread t = new Thread(() =>
        {
            long time = (long)Math.Ceiling(timeSlider.Value * 1000);
            ViewModel.ChangeTime(time);
            ViewModel.Play();
        });
        t.Start();

    }

    private void VolumeSlider_PointerPressed(object? sender, PointerPressedEventArgs e)
    {
        ViewModel.XVolume = volumeSlider.Value;
    }

    private void VolumeSlider_PointerReleased(object? sender, PointerReleasedEventArgs e)
    {
        //int level = (int)Math.Ceiling(volumeSlider.Value);
        int level = (int)volumeSlider.Value;
        ViewModel.XVolume = volumeSlider.Value;
    }

    public void FullScreen_Click(object sender, RoutedEventArgs e)
    {
        var player = VideoPlayerView.GetInstance();
        //player.FullScreen_Click();
    }

    /*
    public void CloseDialog_Click(object sender, RoutedEventArgs e)
    {
        Thread thread = new Thread(() =>
        {
            var player = VideoPlayerViewControl.GetInstance();
            player.CloseDialog_Click();
        });
        thread.Start();
    }
    */
}
