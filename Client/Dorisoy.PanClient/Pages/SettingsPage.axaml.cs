using Avalonia.Interactivity;
using Avalonia.ReactiveUI;
using Dorisoy.PanClient.ViewModels;

namespace Dorisoy.PanClient.Pages;

public partial class SettingsPage : ReactiveUserControl<SettingsPageViewModel>
{
    public SettingsPage()
    {
        InitializeComponent();

        this.WhenActivated(disposable =>
        {

        });
    }

    protected override void OnLoaded(RoutedEventArgs e)
    {
        base.OnLoaded(e);

        //var dc = DataContext as SettingsPageViewModel;
        //dc.CurrentAppTheme = Application.Current.ActualThemeVariant;

        //if (TryGetResource("SystemAccentColor", null, out var value))
        //{
        //    var color = Unsafe.Unbox<Color>(value);
        //    dc.CustomAccentColor = color;
        //    dc.ListBoxColor = color;
        //}
    }
}
