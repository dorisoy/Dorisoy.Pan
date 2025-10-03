using Dorisoy.PanClient.ViewModels;

namespace Dorisoy.PanClient.Pages;

public partial class AddPatientView : ReactiveUserControl<AddPatientViewModel>
{
    public AddPatientView()
    {
        this.InitializeComponent();
        this.WhenActivated(disposable =>
        {
            try
            {
                this.BindValidation(ViewModel, vm => vm.Items, v => v.DataGrid.ItemsSource)
                .DisposeWith(disposable);
            }
            catch (Exception)
            {
            }
        });
    }
}
