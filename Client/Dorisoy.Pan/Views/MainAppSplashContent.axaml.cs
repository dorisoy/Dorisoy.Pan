namespace Dorisoy.PanClient.Views;
public partial class MainAppSplashContent : UserControl
{
    public MainAppSplashContent()
    {
        this.InitializeComponent();
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
