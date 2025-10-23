using Dorisoy.Pan.ViewModels;

namespace Dorisoy.Pan.Pages.Upload;

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
