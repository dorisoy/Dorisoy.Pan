using Dorisoy.PanClient.ViewModels;

namespace Dorisoy.PanClient.Views;

public partial class LoginView : ReactiveUserControl<LoginViewModel>
{


    public LoginView()
    {
        this.InitializeComponent();
        this.WhenActivated(disposables => { });
    }

    protected override void OnLoaded(RoutedEventArgs e)
    {
        base.OnLoaded(e);

        Dispatcher.UIThread.Post(() =>
        {
            if (App.Current.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                var win = desktop.MainWindow;
                //全屏最大化
                win.WindowState = WindowState.Maximized;
                //显示最大最小化
                win.ExtendClientAreaToDecorationsHint = true;
            }

        }, DispatcherPriority.Background);
    }

    //private async void TosaClick(object? sender, RoutedEventArgs e)
    //{
    //    try
    //    {
    //        _lastNotification = nf;
    //    }
    //    catch (Exception ex)
    //    {
    //        //Log(ex.Message);
    //    }
    //}
}
