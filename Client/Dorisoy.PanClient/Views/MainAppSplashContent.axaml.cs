using Avalonia.Controls;
using Avalonia.Interactivity;

namespace Dorisoy.PanClient.Views;
public partial class MainAppSplashContent : UserControl
{
    public MainAppSplashContent()
    {
        InitializeComponent();
    }

    protected override void OnLoaded(RoutedEventArgs e)
    {
        base.OnLoaded(e);

        if (!Design.IsDesignMode)
        {
            Dispatcher.UIThread.Post(() =>
            {


            }, DispatcherPriority.Background);
        }
    }
}
