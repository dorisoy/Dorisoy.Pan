using Dorisoy.Pan.ViewModels;


namespace Dorisoy.Pan.Pages;

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
