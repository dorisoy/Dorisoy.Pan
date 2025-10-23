using Dorisoy.Pan.ViewModels;

namespace Dorisoy.Pan.Pages;

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
