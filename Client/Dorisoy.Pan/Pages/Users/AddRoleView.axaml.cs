using Dorisoy.Pan.ViewModels;
namespace Dorisoy.Pan.Pages;

public partial class AddRoleView : ReactiveUserControl<AddRoleViewModel>
{
    public AddRoleView()
    {
        this.InitializeComponent();

        this.WhenActivated(disposable =>
        {
            //this.BindValidation(ViewModel, vm => vm.Model.Name, v => v.RoleNameValidation.Text)
            //   .DisposeWith(disposable);
        });

    }
}
