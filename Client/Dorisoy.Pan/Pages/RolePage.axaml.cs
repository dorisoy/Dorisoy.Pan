using FluentAvalonia.UI.Navigation;
using Dorisoy.Pan.ViewModels;
using Frame = FluentAvalonia.UI.Controls.Frame;
namespace Dorisoy.Pan.Pages;

public partial class RolePage : ReactiveUserControl<RolePageViewModel>
{

    public RolePage()
    {
        this.InitializeComponent();

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
