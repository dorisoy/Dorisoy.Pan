using Avalonia.ReactiveUI;
using MsBox.Avalonia;
using Dorisoy.PanClient.ViewModels;

namespace Dorisoy.PanClient.Pages;

public partial class AddPatientView : ReactiveUserControl<AddPatientViewModel>
{
    public AddPatientView()
    {
        InitializeComponent();

        this.WhenActivated(async disposable =>
        {
            try
            {
                //this.BindValidation(ViewModel, vm => vm.Items, v => v.DataGrid.ItemsSource)
                //.DisposeWith(disposable);

                //this.BindValidation(ViewModel, vm => vm.Model.RaleName, v => v.UsernameValidation.Text)
                // .DisposeWith(disposable);

                //this.BindValidation(ViewModel, vm => vm.Model.PhoneNumber, v => v.PhoneNumberValidation.Text)
                //.DisposeWith(disposable);
            }
            catch (Exception ex)
            {
                var mbs = MessageBoxManager.GetMessageBoxStandard("Exception", ex.Message);
                await mbs.ShowAsync();
            }
        });
    }
}
