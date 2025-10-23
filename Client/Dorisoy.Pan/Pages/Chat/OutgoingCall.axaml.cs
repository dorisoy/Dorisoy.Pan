using Dorisoy.Pan.ViewModels;

namespace Dorisoy.Pan.Pages.Chat;

public partial class OutgoingCall : ReactiveUserControl<MainViewViewModel>
{
    public OutgoingCall()
    {
        this.InitializeComponent();
        this.WhenActivated(disposable => { });
    }
}
