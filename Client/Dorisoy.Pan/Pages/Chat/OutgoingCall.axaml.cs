using Dorisoy.PanClient.ViewModels;

namespace Dorisoy.PanClient.Pages.Chat;

public partial class OutgoingCall : ReactiveUserControl<MainViewViewModel>
{
    public OutgoingCall()
    {
        this.InitializeComponent();
        this.WhenActivated(disposable => { });
    }
}
