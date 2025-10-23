namespace Dorisoy.Pan.Views;

public partial class ExceptionDialog : AppWindowBase
{
    public ExceptionDialog()
    {
        this.InitializeComponent();

        TitleBar.ExtendsContentIntoTitleBar = true;
        TitleBar.TitleBarHitTestType = TitleBarHitTestType.Complex;
    }

    private void InitializeComponent()
    {
        this.InitializeComponent();
    }

    [SuppressMessage("ReSharper", "UnusedParameter.Local")]
    private void ExitButton_OnClick(object? sender, RoutedEventArgs e)
    {
        Close();
    }
}
