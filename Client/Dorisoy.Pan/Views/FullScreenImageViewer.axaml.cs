using Dorisoy.PanClient.ViewModels;


namespace Dorisoy.PanClient.Views;

public partial class FullScreenImageViewer : ReactiveWindow<FullScreenImageViewerViewModel>
{
    //private static FullScreenImageViewer _this;

    public FullScreenImageViewer()
    {
        this.InitializeComponent();

        //_this = this;

        this.ExtendClientAreaChromeHints = Avalonia.Platform.ExtendClientAreaChromeHints.NoChrome; // 移除任何额外的装饰元素
        this.ExtendClientAreaTitleBarHeightHint = -1; // 标题栏高度设置为负值可以隐藏标题栏

        // 注册窗口打开事件处理程序以设置全屏
        this.Opened += HandleOpened;
        this.WhenActivated(disposables => { });
    }

    private void HandleOpened(object sender, EventArgs e)
    {

        var screen = this.Screens.ScreenFromVisual(this);
        this.Position = new PixelPoint(0, 0);
        this.Width = screen.Bounds.Width;
        this.Height = screen.Bounds.Height;

        // 单击任何地方关闭预览
        // 可以根据自己的情况选择合适的关闭机制，例如双击或按特定键
        PointerPressed += (s, e) =>
        {
            this.Close();
        };
    }

    //public static FullScreenImageViewer GetInstance()
    //{
    //    return _this;
    //}

    //private void MainWindow_Opened(object? sender, System.EventArgs e)
    //{
    //    //var tmp = YampView.GetInstance();
    //    //tmp.SetPlayerHandle();
    //}

    protected override  void OnClosed(EventArgs e)
    {
        //await this.ViewModel?.OnUnloadedAsync();
        base.OnClosed(e);
    }
}
