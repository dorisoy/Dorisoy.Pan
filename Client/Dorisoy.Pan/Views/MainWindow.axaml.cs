using System.Diagnostics;
using FluentAvalonia.Styling;
using Dorisoy.Pan.ViewModels;

namespace Dorisoy.Pan.Views;

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

        //ȫ�����
        this.WindowState = WindowState.Maximized;
        //��ʾ�����С��
        this.ExtendClientAreaToDecorationsHint = true;

        // �������ڹر��¼�
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

        // ��ȡ����Ļ�ı߽���Ϣ
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
    /// �ر�ʱKill myProcess
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
