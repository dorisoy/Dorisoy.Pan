using Dorisoy.PanClient.ViewModels;

namespace Dorisoy.PanClient.Pages;

public partial class AddUserView : ReactiveUserControl<AddUserViewModel>
{
    public AddUserView()
    {
        this.InitializeComponent();

        this.WhenActivated(disposable =>
        {
            try
            {
                //this.BindValidation(ViewModel, vm => vm.Model.UserName, v => v.UsernameValidation.Text)
                // .DisposeWith(disposable);

                //this.BindValidation(ViewModel, vm => vm.Model.Password, v => v.PasswordValidation.Text)
                //    .DisposeWith(disposable);

                //this.BindValidation(ViewModel, vm => vm.Model.PhoneNumber, v => v.PhoneNumberValidation.Text)
                //.DisposeWith(disposable);
            }
            catch (Exception ex)
            {
                //ViewModel.MessageBox(ex.Message);
            }
        });
    }
}
