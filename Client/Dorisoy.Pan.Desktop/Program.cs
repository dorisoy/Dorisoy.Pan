
using System.IO;

namespace Dorisoy.Pan.Desktop;

internal class Program
{
    public static bool IsDebugBuild { get; private set; }
    private const int TimeoutSeconds = 3;

    /// <summary>
    /// 初始化代码
    /// 在调用AppMain之前，
    /// 不要使用任何Avalonia、第三方API或任何SynchronizationContext依赖的代码：事情还没有初始化，可能会中断。
    /// </summary>
    /// <param name="args"></param>
    /// <returns></returns>
    [STAThread]
    public static void Main(string[] args)
    {
        //Directory.SetCurrentDirectory(Path.GetDirectoryName(AppContext.BaseDirectory) ?? String.Empty);

        var mutex = new Mutex(false, typeof(Program).FullName);

        // 为未处理的异常配置异常对话框
        if (!Debugger.IsAttached)
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;


        try
        {
            if (!mutex.WaitOne(TimeSpan.FromSeconds(TimeoutSeconds), true))
                return;

            //配置服务
            BuildAvaloniaApp()
                .StartWithClassicDesktopLifetime(args, ShutdownMode.OnMainWindowClose);

        }
        finally
        {
            mutex.ReleaseMutex();
        }
    }



    /// <summary>
    /// Avalonia配置
    /// </summary>
    /// <returns></returns>
    public static AppBuilder BuildAvaloniaApp()
    {
        // 从ViewModels注册View
        Locator.CurrentMutable.RegisterViewsForViewModels(Assembly.GetExecutingAssembly());

        var result = AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .LogToTrace()
            .With(new Win32PlatformOptions()
            {
                CompositionMode = new[] { Win32CompositionMode.WinUIComposition } ,
                //当您需要圆角模糊的Windows 10应用程序或无边框的Windows 11应用程序时，这很有用
                WinUICompositionBackdropCornerRadius = 8f
            })
            //默认情况下的vlc渲染
            .UseVLCSharp()
            //注册ReactiveUI
            .UseReactiveUI();

        return result;
    }

    /// <summary>
    ///  未处理异常
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
    {
        if (e.ExceptionObject is not Exception ex)
            return;
 
        if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime lifetime)
        {
            var dialog = new ExceptionDialog
            {
                DataContext = new ExceptionViewModel
                {
                    Exception = ex
                }
            };

            var mainWindow = lifetime.MainWindow;
            // 只有当主窗口存在并且可见时，我们才能显示对话
            if (mainWindow is { PlatformImpl: not null, IsVisible: true })
            {
                // 配置为对话框模式
                dialog.ShowAsDialog = true;
                dialog.WindowStartupLocation = WindowStartupLocation.CenterOwner;

                // 同步显示而不阻塞UI线程
                // https://github.com/AvaloniaUI/Avalonia/issues/4810#issuecomment-704259221
                var cts = new CancellationTokenSource();

                dialog.ShowDialog(mainWindow).ContinueWith(_ =>
                {
                    cts.Cancel();
                    ExitWithException(ex);
                }, TaskScheduler.FromCurrentSynchronizationContext());

                Dispatcher.UIThread.MainLoop(cts.Token);
            }
            else
            {
                // 没有可用的父窗口
                var cts = new CancellationTokenSource();
                // Token取消时退出
                cts.Token.Register(() => ExitWithException(ex));

                dialog.ShowWithCts(cts);

                Dispatcher.UIThread.MainLoop(cts.Token);
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="exception"></param>
    [DoesNotReturn]
    private static void ExitWithException(Exception exception)
    {
        App.Shutdown(1);
        Dispatcher.UIThread.InvokeShutdown();
        Environment.Exit(Marshal.GetHRForException(exception));
    }



    [Conditional("DEBUG")]
    private static void SetDebugBuild()
    {
        IsDebugBuild = true;
    }
}
