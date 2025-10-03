﻿using FluentAvalonia.UI.Navigation;
using Dorisoy.PanClient.ViewModels;
using Frame = FluentAvalonia.UI.Controls.Frame;

namespace Dorisoy.PanClient.Pages;

public partial class AddUserRolesView : ReactiveUserControl<AddUserRolesViewModel>
{
    public AddUserRolesView()
    {
        this.InitializeComponent();

        AddHandler(Frame.NavigatingFromEvent, OnNavigatingFrom, RoutingStrategies.Direct);
        AddHandler(Frame.NavigatedToEvent, OnNavigatedTo, RoutingStrategies.Direct);

        this.WhenActivated(disposable =>
        {
            if (ViewModel != null)
            {
                this.OneWayBind(ViewModel, vm => vm.Items, v => v.DataGrid.ItemsSource)
                .DisposeWith(disposable);
            }
        });
    }

    private void OnNavigatedTo(object sender, NavigationEventArgs e)
    {

    }

    private void OnNavigatingFrom(object sender, NavigatingCancelEventArgs e)
    {

    }
}
