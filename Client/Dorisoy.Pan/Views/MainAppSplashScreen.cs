namespace Dorisoy.Pan;

internal class MainAppSplashScreen : IApplicationSplashScreen
{
    public MainWindow Owner { get; }
    public MainAppSplashScreen(MainWindow owner)
    {
        Owner = owner;
    }

    public string AppName { get; }
    public IImage AppIcon { get; }

    public object SplashScreenContent => new MainAppSplashContent();

    public int MinimumShowTime => 2000;

    public Action InitApp { get; set; }

    public Task RunTasks(CancellationToken cancellationToken)
    {
        if (InitApp == null)
            return Task.CompletedTask;

        return Task.Run(InitApp, cancellationToken);
    }

}
