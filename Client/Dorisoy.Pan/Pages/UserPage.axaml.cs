using FluentAvalonia.UI.Navigation;
using Dorisoy.Pan.ViewModels;
//using Frame = FluentAvalonia.UI.Controls.Frame;

namespace Dorisoy.Pan.Pages;

public partial class UserPage : ReactiveUserControl<UserPageViewModel>
{
    public Button AddUserButton => this.FindControl<Button>(nameof(AddUserButton));
    public UserPage()
    {
        this.InitializeComponent();

        //AddHandler(Frame.NavigatingFromEvent, OnNavigatingFrom, RoutingStrategies.Direct);
        //AddHandler(Frame.NavigatedToEvent, OnNavigatedTo, RoutingStrategies.Direct);

        //ss.ExpandSubTree()
        this.WhenActivated(disposable =>
        {
            this.OneWayBind(this.ViewModel, vm => vm.Departments, v => v.myDepartments.ItemsSource)
            .DisposeWith(disposable);

            this.BindCommand(this.ViewModel, vm => vm.AddUser, v => v.AddUserButton)
                .DisposeWith(disposable);

            this.OneWayBind(this.ViewModel, vm => vm.Items, v => v.UserDataGrid.ItemsSource)
            .DisposeWith(disposable);

            this.Bind(this.ViewModel, vm => vm.SelectedUser, v => v.UserDataGrid.SelectedItem)
                .DisposeWith(disposable);
        });


    }

    //private void OnNavigatedTo(object sender, NavigationEventArgs e)
    //{
    //}

    //private void OnNavigatingFrom(object sender, NavigatingCancelEventArgs e)
    //{
    //}
}
