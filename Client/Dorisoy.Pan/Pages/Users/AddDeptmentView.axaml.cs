using Dorisoy.PanClient.ViewModels;


namespace Dorisoy.PanClient.Pages;

public partial class AddDeptmentView : ReactiveUserControl<AddDeptmentViewModel>
{
    public AddDeptmentView()
    {
        this.InitializeComponent();

        this.WhenActivated(disposable =>
        {
        });
    }
}
