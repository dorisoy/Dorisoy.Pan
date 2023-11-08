using Avalonia.ReactiveUI;
using Dorisoy.PanClient.ViewModels;


namespace Dorisoy.PanClient.Pages;

public partial class AddDeptmentView : ReactiveUserControl<AddDeptmentViewModel>
{
    public AddDeptmentView()
    {
        InitializeComponent();
        this.WhenActivated(disposable =>
        {
        });
    }
}
