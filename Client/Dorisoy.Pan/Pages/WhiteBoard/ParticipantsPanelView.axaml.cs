using Dorisoy.Pan.ViewModels;

namespace Dorisoy.Pan.Pages;

public partial class ParticipantsPanelView : ReactiveUserControl<ParticipantsPanelViewModel>
{
    public ParticipantsPanelView()
    {
        this.InitializeComponent();
        this.WhenActivated(disposables => { });
    }
}
