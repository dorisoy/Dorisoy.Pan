using FluentAvalonia.UI.Navigation;
using Dorisoy.PanClient.ViewModels;
using Frame = FluentAvalonia.UI.Controls.Frame;
using Image = Avalonia.Controls.Image;
namespace Dorisoy.PanClient.Pages;

public partial class MonitorPage : ReactiveUserControl<MonitorPageViewModel>
{
    public Button rewind;
    public Grid pipViewGrid;
    public Image mainWebcamViewer;

    public MonitorPage()
    {
        this.InitializeComponent();

        rewind = this.Get<Button>("Rewind");
        pipViewGrid = this.Get<Grid>("PipViewGrid");
        mainWebcamViewer = this.Get<Image>("MainWebcamViewer");

        mainWebcamViewer.RenderTransformOrigin = new RelativePoint(0.5, 0.5, RelativeUnit.Relative);

        AddHandler(Frame.NavigatingFromEvent, OnNavigatingFrom, RoutingStrategies.Direct);
        AddHandler(Frame.NavigatedToEvent, OnNavigatedTo, RoutingStrategies.Direct);

        if (pipViewGrid != null)
        {
            pipViewGrid.PointerEntered += Controls_PointerEnter;
            pipViewGrid.PointerExited += Controls_PointerLeave;
        }

        this.WhenActivated(disposable =>
        {

        });
    }

    public void Controls_PointerEnter(object? sender, PointerEventArgs e)
    {
        rewind.Opacity = 1;
        ControlsPanelViewControl_Opened();
    }

    public void Controls_PointerLeave(object? sender, PointerEventArgs e)
    {
        rewind.Opacity = 0;
    }

    private void OnMyImageButtonContextRequested(object sender, ContextRequestedEventArgs e)
    {
        ShowMenu(false);
        e.Handled = true;
    }

    private void ControlsPanelViewControl_Opened()
    {

    }


    private void ShowMenu(bool isTransient)
    {
        var flyout = Resources["ImageCommandBarFlyout"] as CommandBarFlyout;
        flyout.ShowMode = isTransient ? FlyoutShowMode.Transient : FlyoutShowMode.Standard;
        flyout.ShowAt(this.FindControl<Image>("CommandBarImage"));
    }

    /// <summary>
    /// 正在导航到的页面
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void OnNavigatedTo(object sender, NavigationEventArgs e)
    {
        System.Diagnostics.Debug.Print($"到达:------>-------MonitorPage");
        //if (Globals.LastSelectCamera != null)
        //    ViewModel.StartCapture(Globals.LastSelectCamera);
    }

    /// <summary>
    /// 指示要离开的页面
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void OnNavigatingFrom(object sender, NavigatingCancelEventArgs e)
    {
        ViewModel?.StopReceived();
        ViewModel?.StopCapture();
        ViewModel?.StopPipWebcam();
        System.Diagnostics.Debug.Print($"离开:------>-------MonitorPage");
    }

    private bool isScaleX, isScaleY;

    /// <summary>
    /// 水平翻转
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="args"></param>
    private void HorizontalX_Click(object sender, RoutedEventArgs args)
    {
        var scaleTransform = new ScaleTransform
        {
            ScaleX = isScaleX ? 1 : -1, // 水平翻转
            ScaleY = 1
        };
        isScaleX = scaleTransform.ScaleX != 1;
        mainWebcamViewer.RenderTransform = scaleTransform;
    }
    /// <summary>
    /// 垂直翻转
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="args"></param>

    private void VerticalY_Click(object sender, RoutedEventArgs args)
    {
        var scaleTransform = new ScaleTransform
        {
            ScaleX = 1,
            ScaleY = isScaleY ? 1 : -1 // 垂直翻转
        };
        isScaleY = scaleTransform.ScaleY != 1;
        mainWebcamViewer.RenderTransform = scaleTransform;
    }

    private DoubleClickHelper doubleClickHelper = new DoubleClickHelper();
    private void Border_PointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if (e.ClickCount == 2 || doubleClickHelper.IsDoubleClick())
        {
            this.ViewModel.ShowDisableMaxMin();
        }
    }
}
