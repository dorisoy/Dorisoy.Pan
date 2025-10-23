using Dorisoy.Pan.ViewModels;
namespace Dorisoy.Pan.Pages;

public partial class HomePage : ReactiveUserControl<HomePageViewModel>
{
    public HomePage()
    {
        this.InitializeComponent();
        this.SizeChanged += HomePage_SizeChanged;
        this.WhenActivated(disposable => { });
    }

    private void HomePage_SizeChanged(object sender, SizeChangedEventArgs e)
    {
        
    }
}
