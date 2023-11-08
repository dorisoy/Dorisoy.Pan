using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Interactivity;
using Avalonia.ReactiveUI;
using Dorisoy.PanClient.ViewModels;

namespace Dorisoy.PanClient.Views;

public partial class LoginView : ReactiveUserControl<LoginViewModel>
{
    public LoginView()
    {
        InitializeComponent();
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
}
