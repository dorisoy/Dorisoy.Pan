using Dorisoy.Pan.ViewModels;


namespace Dorisoy.Pan.Views;

public partial class FullScreenImageViewer : ReactiveWindow<FullScreenImageViewerViewModel>
{
    //private static FullScreenImageViewer _this;

    public FullScreenImageViewer()
    {
        this.InitializeComponent();

        //_this = this;

        this.ExtendClientAreaChromeHints = Avalonia.Platform.ExtendClientAreaChromeHints.NoChrome; // �Ƴ��κζ����װ��Ԫ��
        this.ExtendClientAreaTitleBarHeightHint = -1; // �������߶�����Ϊ��ֵ�������ر�����

        // ע�ᴰ�ڴ��¼���������������ȫ��
        this.Opened += HandleOpened;
        this.WhenActivated(disposables => { });
    }

    private void HandleOpened(object sender, EventArgs e)
    {

        var screen = this.Screens.ScreenFromVisual(this);
        this.Position = new PixelPoint(0, 0);
        this.Width = screen.Bounds.Width;
        this.Height = screen.Bounds.Height;

        // �����κεط��ر�Ԥ��
        // ���Ը����Լ������ѡ����ʵĹرջ��ƣ�����˫�����ض���
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
