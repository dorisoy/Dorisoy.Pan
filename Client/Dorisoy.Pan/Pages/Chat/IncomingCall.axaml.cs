using Dorisoy.Pan.ViewModels;

namespace Dorisoy.Pan.Pages.Chat;

public partial class IncomingCall : ReactiveUserControl<MainViewViewModel>
{
    public IncomingCall()
    {
        this.InitializeComponent();
        this.WhenActivated(disposable => { });
    }
}
