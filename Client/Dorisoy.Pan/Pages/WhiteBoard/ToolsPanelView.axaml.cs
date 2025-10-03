using Dorisoy.PanClient.ViewModels;

namespace Dorisoy.PanClient.Pages;

public partial class ToolsPanelView : ReactiveUserControl<ToolsPanelViewModel>
{
    public ToolsPanelView()
    {
        this.InitializeComponent();
        this.WhenActivated(disposables => { });
    }
}
