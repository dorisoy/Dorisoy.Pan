using System.Diagnostics;
using FluentAvalonia.Styling;
using Dorisoy.PanClient.ViewModels;

namespace Dorisoy.PanClient.Views;

public partial class MainWindow : ReactiveCoreWindow<MainWindowViewModel>
{
    public Process myProcess;
    public MainWindow()
    {
        this.InitializeComponent();

#if DEBUG
        this.AttachDevTools();
#endif

        SplashScreen = new MainAppSplashScreen(this);
        TitleBar.ExtendsContentIntoTitleBar = true;
        TitleBar.TitleBarHitTestType = TitleBarHitTestType.Complex;

        Application.Current.ActualThemeVariantChanged += OnActualThemeVariantChanged;
        AppDomain.CurrentDomain.ProcessExit += (a, b) => Environment.Exit(0);
        AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

        //myProcess = new Process();
        //myProcess.StartInfo.FileName = @".\ScreenRecording\ScreenRecording.exe";

        //全屏最大化
        this.WindowState = WindowState.Maximized;
        //显示最大最小化
        this.ExtendClientAreaToDecorationsHint = true;

        // 处理窗口关闭事件
        this.Closing += MainWindow_Closing;

    }

    private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
    {
        if (!((Exception)e.ExceptionObject).Message.StartsWith("Cannot set Visibility"))
        {
            string ex = "\n--------\nCurrentDomainUnhandledException [" + DateTime.Now.ToString() + "]\n";
            ex += ((Exception)e.ExceptionObject).Message + ((Exception)e.ExceptionObject).StackTrace;
            string workingDir = System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            File.AppendAllText(workingDir + "/CrashDump.txt", ex);
        }

        //Shutdown(1);
        //Process.GetCurrentProcess().WaitForExit(2000);
        //Process.GetCurrentProcess().Kill();
    }

    public static void Shutdown(int exitCode = 0)
    {
        if (Application.Current is null)
            throw new NullReferenceException("Current Application was null when Shutdown called");

        if (Application.Current.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime lifetime)
        {
            //Exception_WasThrown
            //lifetime.Shutdown(exitCode);
        }
    }

    public PixelRect PrimaryScreenBounds { get; set; }

    protected override void OnOpened(EventArgs e)
    {
        base.OnOpened(e);

        // 获取主屏幕的边界信息
        PrimaryScreenBounds = Screens.Primary.Bounds;

        App.MainWindow = this;

        var thm = ActualThemeVariant;
        if (IsWindows11 && thm != FluentAvaloniaTheme.HighContrastTheme)
        {
            TryEnableMicaEffect();
        }
    }

    private void OnActualThemeVariantChanged(object sender, EventArgs e)
    {
        if (IsWindows11)
        {
            if (ActualThemeVariant != FluentAvaloniaTheme.HighContrastTheme)
            {
                TryEnableMicaEffect();
            }
            else
            {
                ClearValue(BackgroundProperty);
                ClearValue(TransparencyBackgroundFallbackProperty);
            }
        }
    }


    private void TryEnableMicaEffect()
    {
        return;
    }


    /// <summary>
    /// 关闭时Kill myProcess
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
    {
        if (myProcess != null && !myProcess.HasExited)
        {
            myProcess.Kill();
        }
    }
}
