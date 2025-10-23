using Dorisoy.Pan.ViewModels;

namespace Dorisoy.Pan.Pages;

public partial class ToolsPanelView : ReactiveUserControl<ToolsPanelViewModel>
{
    public ToolsPanelView()
    {
        this.InitializeComponent();
        this.WhenActivated(disposables => { });
    }
}
