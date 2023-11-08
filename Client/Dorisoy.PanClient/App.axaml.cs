using System.Diagnostics;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using FluentAvalonia.Styling;
using LocalizationManager;
using MsBox.Avalonia;
using OfficeOpenXml;
using Dorisoy.PanClient.Language;
using Dorisoy.PanClient.ViewModels;
using Dorisoy.PanClient.Views;

namespace Dorisoy.PanClient;

public partial class App : Application
{
    public static MainWindow MainWindow { get; set; }
    public static MainView MainView { get; set; }

    public override void Initialize()
    {
        ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;

        AvaloniaXamlLoader.Load(this);
        (App.Current.Styles[0] as FluentAvaloniaTheme).CustomAccentColor = Color.Parse("#004883");

        ApplicationConfigurator.ConfigureServices(Locator.CurrentMutable, Locator.Current);

        Locator.CurrentMutable.Register(() => new SettingsPage(), typeof(IViewFor<SettingsPageViewModel>));
        Locator.CurrentMutable.Register(() => new LoginView(), typeof(IViewFor<LoginViewModel>));
        Locator.CurrentMutable.Register(() => new MainView(), typeof(IViewFor<MainViewViewModel>));
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
            var viewModel = new MainWindowViewModel();
            desktop.MainWindow = new MainWindow()
            {
                DataContext = viewModel,
                //屏幕居中
                WindowStartupLocation = WindowStartupLocation.CenterScreen
            };
        }
        else if (ApplicationLifetime is ISingleViewApplicationLifetime singleView)
        {
            singleView.MainView = new MainView();
        }

        base.OnFrameworkInitializationCompleted();
    }

    private void Startup(object sender, ControlledApplicationLifetimeStartupEventArgs e)
    {
        if (e.Args.Length > 0)
        {
            Globals.RenderingIntervalMs = short.Parse(e.Args[0]);
        }
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

        //Settings.BackendYtDlpPath
        var pcs = Process.GetProcessesByName("yt-dlp.exe");
        if (pcs != null && pcs.Any())
        {
            foreach (var item in pcs)
            {
                //TerminateProcess(item.Handle, 1);
            }
        }
    }

    public static void MessageBox(string msg)
    {
        Dispatcher.UIThread.Invoke(async () =>
        {
            var box = MessageBoxManager.GetMessageBoxStandard("提示", msg, MsBox.Avalonia.Enums.ButtonEnum.Ok);
            await box.ShowAsync();
        });
    }
}
