using Dorisoy.Pan.ViewModels;

namespace Dorisoy.Pan.Views;

public partial class WAMPlayer : ReactiveCoreWindow<WAMPlayerViewModel>
{
    private static WAMPlayer _this;
    public WAMPlayer()
    {
        this.InitializeComponent();
        _this = this;

        if (!Design.IsDesignMode)
        {
            Opened += MainWindow_Opened;
        }

        this.WhenActivated(disposables => { });
    }

    public static WAMPlayer GetInstance()
    {
        return _this;
    }

    private void MainWindow_Opened(object? sender, System.EventArgs e)
    {
        var tmp = YampView.GetInstance();
        tmp.SetPlayerHandle();
    }

    protected override void OnClosed(EventArgs e)
    {
        //await this.ViewModel?.OnUnloadedAsync();
        base.OnClosed(e);
    }

}
