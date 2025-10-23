using Dorisoy.Pan.ViewModels;

namespace Dorisoy.Pan.Pages;

public partial class ConnectionStatusView : ReactiveUserControl<WhiteBoardViewModel>
{
    public ConnectionStatusView()
    {
        this.InitializeComponent();
        this.WhenActivated(disposables => { });
    }
}
