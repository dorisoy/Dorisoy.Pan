using Dorisoy.PanClient.ViewModels;

namespace Dorisoy.PanClient.Pages;

public partial class ParticipantsPanelView : ReactiveUserControl<ParticipantsPanelViewModel>
{
    public ParticipantsPanelView()
    {
        this.InitializeComponent();
        this.WhenActivated(disposables => { });
    }
}
