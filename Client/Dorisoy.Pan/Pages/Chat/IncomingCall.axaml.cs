using Dorisoy.PanClient.ViewModels;

namespace Dorisoy.PanClient.Pages.Chat;

public partial class IncomingCall : ReactiveUserControl<MainViewViewModel>
{
    public IncomingCall()
    {
        this.InitializeComponent();
        this.WhenActivated(disposable => { });
    }
}
