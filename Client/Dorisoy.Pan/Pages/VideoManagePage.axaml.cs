using FluentAvalonia.UI.Navigation;
using Dorisoy.PanClient.ViewModels;
using Frame = FluentAvalonia.UI.Controls.Frame;
namespace Dorisoy.PanClient.Pages;

public partial class VideoManagePage : ReactiveUserControl<VideoManagePageViewModel>
{
    Window GetWindow() => TopLevel.GetTopLevel(this) as Window ?? throw new NullReferenceException("Invalid Owner");
    TopLevel GetTopLevel() => TopLevel.GetTopLevel(this) ?? throw new NullReferenceException("Invalid Owner");

    public VideoManagePage()
    {
        this.InitializeComponent();

        AddHandler(Frame.NavigatingFromEvent, OnNavigatingFrom, RoutingStrategies.Direct);
        AddHandler(Frame.NavigatedToEvent, OnNavigatedTo, RoutingStrategies.Direct);
    }


    private void OnNavigatedTo(object sender, NavigationEventArgs e)
    {
    }

    private void OnNavigatingFrom(object sender, NavigatingCancelEventArgs e)
    {
    }
}
