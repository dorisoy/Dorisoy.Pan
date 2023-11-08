using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using Avalonia.ReactiveUI;
using FluentAvalonia.UI.Navigation;
using Dorisoy.PanClient.ViewModels;

namespace Dorisoy.PanClient.Pages;

public partial class DocumentPage : ReactiveUserControl<DocumentPageViewModel>
{
    Window GetWindow() => TopLevel.GetTopLevel(this) as Window ?? throw new NullReferenceException("Invalid Owner");
    TopLevel GetTopLevel() => TopLevel.GetTopLevel(this) ?? throw new NullReferenceException("Invalid Owner");

    public DocumentPage()
    {
        InitializeComponent();

        AddHandler(Frame.NavigatingFromEvent, OnNavigatingFrom, RoutingStrategies.Direct);
        AddHandler(Frame.NavigatedToEvent, OnNavigatedTo, RoutingStrategies.Direct);

        //Ë«»÷
        this.DataGrid.DoubleTapped += (sender, args) =>
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
            RegisterStorageProvider(Locator.CurrentMutable);
            this.OneWayBind(ViewModel, vm => vm.Items, v => v.DataGrid.ItemsSource)
               .DisposeWith(disposable);
        });
    }




    private IStorageProvider GetStorageProvider()
    {
        return GetTopLevel().StorageProvider;
    }

    private IMutableDependencyResolver RegisterStorageProvider(IMutableDependencyResolver services)
    {
        services.RegisterLazySingleton(() => GetStorageProvider());
        return services;
    }

    private void OnNavigatedTo(object sender, NavigationEventArgs e)
    {
        ViewModel?.Refresh();
    }

    private void OnNavigatingFrom(object sender, NavigatingCancelEventArgs e)
    {

    }
}
