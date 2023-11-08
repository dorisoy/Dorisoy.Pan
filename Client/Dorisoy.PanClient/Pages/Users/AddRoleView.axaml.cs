using Avalonia.ReactiveUI;
using Dorisoy.PanClient.ViewModels;
namespace Dorisoy.PanClient.Pages;

public partial class AddRoleView : ReactiveUserControl<AddRoleViewModel>
{
    public AddRoleView()
    {
        InitializeComponent();

        this.WhenActivated(disposable =>
        {
            //this.BindValidation(ViewModel, vm => vm.Model.Name, v => v.RoleNameValidation.Text)
            //   .DisposeWith(disposable);
        });

    }
}
