using Dorisoy.Pan.ViewModels;

namespace Dorisoy.Pan.Pages;

public partial class SettingsPage : ReactiveUserControl<SettingsPageViewModel>
{
    public SettingsPage()
    {
        this.InitializeComponent();

        this.WhenActivated(disposable =>
        {

        });
    }

    protected override void OnLoaded(RoutedEventArgs e)
    {
        base.OnLoaded(e);
    }
}
