using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using Dorisoy.PanClient.ViewModels;

namespace Dorisoy.PanClient.Pages;

public partial class AddFilesView : ReactiveUserControl<AddFilesViewModel>
{
    public AddFilesView()
    {
        AvaloniaXamlLoader.Load(this);
        this.WhenActivated(disposable =>
        {
        });
    }
}
