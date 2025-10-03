using FluentAvalonia.Styling;
using OfficeOpenXml;
using Dorisoy.PanClient.Language;
using Dorisoy.PanClient.ViewModels;
using Color = Avalonia.Media.Color;

namespace Dorisoy.PanClient;

public partial class App : Application
{
    private IClassicDesktopStyleApplicationLifetime desktop = null!;
    public static MainWindow MainWindow { get; set; }
    public static MainView MainView { get; set; }


    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
        ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;
        (App.Current.Styles[0] as FluentAvaloniaTheme).CustomAccentColor = Color.Parse("#004883");
        ServiceExtensions.ConfigureServices(Locator.CurrentMutable, Locator.Current);
    }

    public override void RegisterServices()
    {
        base.RegisterServices();
        //注册本地化管理
        LocalizationManagerBuilder.Initialize(() =>
        {
            return LocalizationProviderExtensions.MakeResourceProvider(LanguageResourceHelper.LanguageResourceManager);
        });
    }


    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            this.desktop = desktop;

            this.ConfigureSuspensionDriver();

            RegisterViews();

            RxApp.MainThreadScheduler = AvaloniaScheduler.Instance;

            this.desktop.MainWindow = new MainWindow()
            {
                DataContext = new MainWindowViewModel(),
                //屏幕居中
                WindowStartupLocation = WindowStartupLocation.CenterScreen
            };

            this.desktop.Exit += this.OnExit;
        }
        else if (ApplicationLifetime is ISingleViewApplicationLifetime singleView)
        {
            singleView.MainView = new MainView();
        }

        base.OnFrameworkInitializationCompleted();
    }


    private void OnExit(object? sender, ControlledApplicationLifetimeExitEventArgs e)
    {
        try
        {
            //Shutting down the settings app
        }
        finally
        {

        }
    }

    /// <summary>
    /// 配置SuspensionDrive
    /// </summary>
    private void ConfigureSuspensionDriver()
    {
        var autoSuspendHelper = new AutoSuspendHelper(this.desktop);
        RxApp.SuspensionHost.CreateNewAppState = () => new AppState();
        RxApp.SuspensionHost.SetupDefaultSuspendResume();
        autoSuspendHelper.OnFrameworkInitializationCompleted();
    }


    private void RegisterViews()
    {
        Locator.CurrentMutable.Register<IViewFor<LoginViewModel>>(() => new LoginView());
        Locator.CurrentMutable.Register<IViewFor<MainViewViewModel>>(() => new MainView());
        Locator.CurrentMutable.Register<IViewFor<WhiteBoardViewModel>>(() => new WhiteBoard());
        Locator.CurrentMutable.Register<IViewFor<FullScreenImageViewerViewModel>>(() => new FullScreenImageViewer());
        Locator.CurrentMutable.Register<IViewFor<SettingsPageViewModel>>(() => new SettingsPage());
        Locator.CurrentMutable.Register<IViewFor<AddPatientViewModel>>(() => new AddPatientView());
    }


    public static void Shutdown(int exitCode = 0)
    {
        if (Current is null)
            throw new NullReferenceException(
            "Current Application was null when Shutdown called");

        if (Current.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime lifetime)
        {
            lifetime.Shutdown(exitCode);
        }
    }
}
