using Dorisoy.Pan.ViewModels;

namespace Dorisoy.Pan.Pages.Upload;

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
