using Dorisoy.Pan.ViewModels;

namespace Dorisoy.Pan.Pages.Upload;

public partial class AddFolderView : ReactiveUserControl<AddFolderViewModel>
{
    public AddFolderView()
    {
        this.WhenActivated(disposable =>
        {
            //this.BindValidation(ViewModel, vm => vm.FolderName, v => v.FolderNameValidation.Text)
            //   .DisposeWith(disposable);
        });
        this.InitializeComponent();
    }
}
