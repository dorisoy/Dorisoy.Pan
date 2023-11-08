using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using Dorisoy.PanClient.ViewModels;

namespace Dorisoy.PanClient.Pages;

public partial class AddCommentView : ReactiveUserControl<AddCommentViewModel>
{
    public AddCommentView()
    {
        this.WhenActivated(disposable =>
        {

        });
        AvaloniaXamlLoader.Load(this);
    }
}
