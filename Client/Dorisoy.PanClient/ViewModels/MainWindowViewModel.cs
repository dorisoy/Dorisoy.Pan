namespace Dorisoy.PanClient.ViewModels;

[View(typeof(MainWindow))]
public class MainWindowViewModel : ReactiveObject, IActivatableViewModel, IScreen
{
    public ViewModelActivator Activator { get; } = new ViewModelActivator();
    public RoutingState Router { get; } = new RoutingState();

    public MainWindowViewModel()
    {
        Router.Navigate.Execute(new LoginViewModel(this));
    }
}
