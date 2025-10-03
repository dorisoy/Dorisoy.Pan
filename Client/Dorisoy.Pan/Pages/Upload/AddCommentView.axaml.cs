using Dorisoy.PanClient.ViewModels;

namespace Dorisoy.PanClient.Pages.Upload;

public partial class AddCommentView : ReactiveUserControl<AddCommentViewModel>
{
    public AddCommentView()
    {
        this.WhenActivated(disposable =>
        {

        });
        this.InitializeComponent();
    }
}
