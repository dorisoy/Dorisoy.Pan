using Dorisoy.PanClient.ViewModels;

namespace Dorisoy.PanClient.Player;

public partial class YampToolView : ReactiveUserControl<YampToolViewModel>
{
    public Slider volumeSlider;
    public Slider timeSlider;

    public FlyoutPanel flyPanelContainer;

    public YampViewModel? playerViewModel;

    static YampToolView? _this;
    public YampToolView()
    {
        this.InitializeComponent();

        volumeSlider = this.Get<Slider>("SliderVolume");
        //timeSlider = this.Get<Slider>("SliderTime");

        //_full = this.Get<Button>("Full");

        flyPanelContainer = this.Get<FlyoutPanel>("ControlsPanel");

        ViewModel = new YampToolViewModel();
        DataContext = ViewModel;

        _this = this;

        //Opened += ControlsPanelViewControl_Opened;

        //timeSlider.Value = 0.0;
        //timeSlider.AddHandler(PointerReleasedEvent, TimeSlider_PointerReleased, RoutingStrategies.Tunnel);
        //timeSlider.AddHandler(PointerPressedEvent, TimeSlider_PointerPressed, RoutingStrategies.Tunnel);

        volumeSlider.AddHandler(PointerPressedEvent, VolumeSlider_PointerPressed, RoutingStrategies.Tunnel);
        volumeSlider.AddHandler(PointerReleasedEvent, VolumeSlider_PointerReleased, RoutingStrategies.Tunnel);

        PointerEntered += Controls_PointerEnter;
        PointerExited += Controls_PointerLeave;

        //_full.IsVisible = true;

        this.WhenActivated(disposable => { });
    }


    public void Controls_PointerEnter(object? sender, PointerEventArgs e)
    {
        //this.Opacity = 1;
        flyPanelContainer.IsOpen = true;
        ControlsPanelViewControl_Opened();
    }

    public void Controls_PointerLeave(object? sender, PointerEventArgs e)
    {
        //this.Opacity = 0;
        flyPanelContainer.IsOpen = false;
    }

    public static YampToolView GetInstance()
    {
        return _this;
    }

    private void ControlsPanelViewControl_Opened()
    {
        try
        {
            var v = YampView.GetInstance();
            if (v != null)
                playerViewModel = YampView.GetInstance().ViewModel;
        }
        catch { }
    }


    private void TimeSlider_PointerPressed(object? sender, PointerPressedEventArgs e)
    {
        ViewModel.Pause();
    }
    private void TimeSlider_PointerReleased(object? sender, PointerReleasedEventArgs e)
    {
        long time = (long)Math.Ceiling(timeSlider.Value * 1000);
        ViewModel.ChangeTime(time);
        ViewModel.Play();
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

    //public void FullScreen_Click(object sender, RoutedEventArgs e)
    //{
    //    var player = YampView.GetInstance();
    //    player.FullScreen_Click();
    //}

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
