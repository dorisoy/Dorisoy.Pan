using Avalonia.Interactivity;
using Avalonia.ReactiveUI;
using FluentAvalonia.UI.Navigation;
using Dorisoy.PanClient.ViewModels;

namespace Dorisoy.PanClient.Pages;

public partial class CommentsView : ReactiveUserControl<CommentsViewModel>
{
    public CommentsView()
    {
        InitializeComponent();

        AddHandler(Frame.NavigatingFromEvent, OnNavigatingFrom, RoutingStrategies.Direct);
        AddHandler(Frame.NavigatedToEvent, OnNavigatedTo, RoutingStrategies.Direct);

        this.WhenActivated(disposable =>
        {
            if (ViewModel != null)
            {
                this.OneWayBind(ViewModel, vm => vm.Items, v => v.ItemsControl.ItemsSource)
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
