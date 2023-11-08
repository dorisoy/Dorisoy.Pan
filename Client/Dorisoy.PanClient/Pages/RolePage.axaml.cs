using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.ReactiveUI;
using FluentAvalonia.UI.Navigation;
using Dorisoy.PanClient.ViewModels;

namespace Dorisoy.PanClient.Pages;

public partial class RolePage : ReactiveUserControl<RolePageViewModel>
{

    public RolePage()
    {
        InitializeComponent();

        AddHandler(Frame.NavigatingFromEvent, OnNavigatingFrom, RoutingStrategies.Direct);
        AddHandler(Frame.NavigatedToEvent, OnNavigatedTo, RoutingStrategies.Direct);
        TreeView ss = new TreeView();

        this.WhenActivated(disposable =>
        {
            this.OneWayBind(ViewModel, vm => vm.Items, v => v.DataGrid.ItemsSource)
                .DisposeWith(disposable);
        });


    }
    private void OnNavigatedTo(object sender, NavigationEventArgs e)
    {

    }

    private void OnNavigatingFrom(object sender, NavigatingCancelEventArgs e)
    {

    }
}
