using FluentAvalonia.UI.Navigation;
using Dorisoy.PanClient.ViewModels;
using Frame = FluentAvalonia.UI.Controls.Frame;

namespace Dorisoy.PanClient.Pages;

public partial class DocumentPage : ReactiveUserControl<DocumentPageViewModel>
{
    public DocumentPage()
    {
        this.InitializeComponent();

        AddHandler(Frame.NavigatingFromEvent, OnNavigatingFrom, RoutingStrategies.Direct);
        AddHandler(Frame.NavigatedToEvent, OnNavigatedTo, RoutingStrategies.Direct);

        //Ë«»÷
        this.myDataGrid.DoubleTapped += (sender, args) =>
        {
            if (sender != null)
            {
                var border = args.Source as Border;
                if (border != null)
                {
                    var rowData = border.DataContext as DocumentFolderModel;
                    if (rowData != null)
                    {
                        ViewModel.Execute(rowData);
                    }
                }
            }
        };

        this.WhenActivated(disposable =>
        {
            Locator.CurrentMutable.RegisterLazySingleton(() => TopLevel.GetTopLevel(this).StorageProvider);

            this.OneWayBind(ViewModel, vm => vm.Items, v => v.myDataGrid.ItemsSource)
               .DisposeWith(disposable);
        });
    }

    private void OnNavigatedTo(object sender, NavigationEventArgs e)
    {
        ViewModel?.Refresh();
    }

    private void OnNavigatingFrom(object sender, NavigatingCancelEventArgs e)
    {

    }
}
