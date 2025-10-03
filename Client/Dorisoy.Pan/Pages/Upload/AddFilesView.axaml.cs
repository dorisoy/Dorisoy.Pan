using Dorisoy.PanClient.ViewModels;

namespace Dorisoy.PanClient.Pages.Upload;

public partial class AddFilesView : ReactiveUserControl<AddFilesViewModel>
{
    public AddFilesView()
    {
        this.InitializeComponent();
        this.WhenActivated(disposable =>
        {
        });
    }
}
