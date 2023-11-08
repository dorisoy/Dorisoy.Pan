using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Dorisoy.PanClient;

public partial class Spinner : UserControl
{
    public Spinner()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}
