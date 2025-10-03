using Dorisoy.PanClient.ViewModels;

namespace Dorisoy.PanClient.Pages.Upload;

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
