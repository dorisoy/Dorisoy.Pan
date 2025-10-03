using FluentAvalonia.UI.Navigation;
using Dorisoy.PanClient.ViewModels;
using Frame = FluentAvalonia.UI.Controls.Frame;
namespace Dorisoy.PanClient.Pages;


/// <summary>
/// 项目信息
/// </summary>
public partial class PatientPage : ReactiveUserControl<PatientPageViewModel>
{
    public PatientPage()
    {
        this.InitializeComponent();

        AddHandler(Frame.NavigatingFromEvent, OnNavigatingFrom, RoutingStrategies.Direct);
        AddHandler(Frame.NavigatedToEvent, OnNavigatedTo, RoutingStrategies.Direct);

        ImagesItems.PointerPressed += ImagesItems_PointerPressed;
        VideosItems.PointerPressed += VideosItems_PointerPressed;

        this.WhenActivated(disposable =>
        {

        });

    }

    private void VideosItems_PointerPressed(object sender, Avalonia.Input.PointerPressedEventArgs e)
    {
        if (sender != null)
        {
            var border = e.Source as Border;
            if (border != null)
            {
                var rowData = border.DataContext as DocumentModel;
                if (rowData != null)
                {
                    ViewModel.Execute(rowData);
                }
            }
        }
    }

    private void ImagesItems_PointerPressed(object sender, Avalonia.Input.PointerPressedEventArgs e)
    {
        if (sender != null)
        {
            var border = e.Source as Border;
            if (border != null)
            {
                var rowData = border.DataContext as DocumentModel;
                if (rowData != null)
                {
                    ViewModel.Execute(rowData);
                }
            }
        }
    }

    private void OnNavigatedTo(object sender, NavigationEventArgs e)
    {

    }

    private void OnNavigatingFrom(object sender, NavigatingCancelEventArgs e)
    {

    }
}
