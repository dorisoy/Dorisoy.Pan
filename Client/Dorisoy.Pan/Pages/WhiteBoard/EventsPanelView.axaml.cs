using Dorisoy.PanClient.ViewModels;

namespace Dorisoy.PanClient.Pages;

public partial class EventsPanelView : ReactiveUserControl<EventsPanelViewModel>
{
    public EventsPanelView()
    {
        this.InitializeComponent();
        this.WhenActivated(disposables => { });
    }

}
