using Dorisoy.Pan.ViewModels;

namespace Dorisoy.Pan.Pages;

public partial class EventsPanelView : ReactiveUserControl<EventsPanelViewModel>
{
    public EventsPanelView()
    {
        this.InitializeComponent();
        this.WhenActivated(disposables => { });
    }

}
