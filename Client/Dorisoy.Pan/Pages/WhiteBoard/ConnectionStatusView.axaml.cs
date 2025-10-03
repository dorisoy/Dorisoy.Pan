using Dorisoy.PanClient.ViewModels;

namespace Dorisoy.PanClient.Pages;

public partial class ConnectionStatusView : ReactiveUserControl<WhiteBoardViewModel>
{
    public ConnectionStatusView()
    {
        this.InitializeComponent();
        this.WhenActivated(disposables => { });
    }
}
