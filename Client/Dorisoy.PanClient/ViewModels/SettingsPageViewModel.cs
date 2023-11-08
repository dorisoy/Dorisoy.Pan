using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Media;
using Avalonia.Styling;
using LocalizationManager;
using Dorisoy.PanClient.Common;

namespace Dorisoy.PanClient.ViewModels;


[View(typeof(SettingsPage))]
public class SettingsPageViewModel : MainPageViewModelBase
{
    private bool _ignoreSetListBoxColor = false;
    private readonly ILocalizationManager _localizationManager;
    private readonly ISettingsProvider<AppSettings> _settingsProvider;
    public ReactiveCommand<Unit, Unit> SaveRSTPServer { get; }

    public ThemeVariant[] AppThemes { get; } = new[] { ThemeVariant.Light };
    public FlowDirection[] AppFlowDirections { get; } = new[] {
        FlowDirection.LeftToRight,
        FlowDirection.RightToLeft
    };

    [Reactive] public bool UseCustomAccent { get; set; }
    [Reactive] public ThemeVariant CurrentAppTheme { get; set; } = ThemeVariant.Light;
    [Reactive] public FlowDirection CurrentFlowDirection { get; set; }
    [Reactive] public Color? ListBoxColor { get; set; }
    [Reactive] public Color CustomAccentColor { get; set; }


    /// <summary>
    /// 远程主机配置
    /// </summary>
    [Reactive] public string HostUrl { get; set; }

    /// <summary>
    /// 远程服务器地址
    /// </summary>
    [Reactive] public string ServerHost { get; set; }

    /// <summary>
    /// 远程服务器UDP端口
    /// </summary>
    [Reactive] public int ServerUdpPort { get; set; }

    /// <summary>
    /// TCP发送接收端口
    /// </summary>
    [Reactive] public int TCPSendReceiver { get; set; }

    /// <summary>
    /// RTMP 服务器地址
    /// </summary>
    [Reactive] public string RSTPServer { get; set; }

    /// <summary>
    /// 远程数据库连接字符串
    /// </summary>
    [Reactive] public string DB_Conn { get; set; }



    public SettingsPageViewModel() : base()
    {
        _localizationManager = LocalizationManagerExtensions.Default!;
        _settingsProvider = Locator.Current.GetService<ISettingsProvider<AppSettings>>();

        RSTPServer = _settingsProvider.Settings.RSTPServer;
        var setting = _settingsProvider.Settings;
        if (setting != null)
        {
            HostUrl = setting.HostUrl;
            ServerHost = setting.ServerHost;
            ServerUdpPort = setting.ServerUdpPort;
            TCPSendReceiver = setting.TCPSendReceiver;
            RSTPServer = setting.RSTPServer;
            DB_Conn = setting.DB_Conn;
        }


        this.WhenAnyValue(x => x.UseCustomAccent)
          .Subscribe(async x =>
          {
              await Dispatcher.UIThread.InvokeAsync(() =>
              {
                  CustomAccentColor = Color.Parse("#004883");
                  ListBoxColor = Color.Parse("#004883");

              }, DispatcherPriority.Background);
          });

        this.WhenAnyValue(x => x.CurrentAppTheme)
            .Subscribe(async x =>
            {
                await Dispatcher.UIThread.InvokeAsync(() =>
                {
                    if (Application.Current.ActualThemeVariant != x)
                    {
                        Application.Current.RequestedThemeVariant = x;
                    }
                }, DispatcherPriority.Background);
            });

        this.WhenAnyValue(x => x.CurrentFlowDirection)
           .Subscribe(async x =>
           {
               await Dispatcher.UIThread.InvokeAsync(() =>
               {
                   var lifetime = Application.Current.ApplicationLifetime;
                   if (lifetime is IClassicDesktopStyleApplicationLifetime cdl)
                   {
                       if (cdl.MainWindow.FlowDirection == x)
                           return;
                       cdl.MainWindow.FlowDirection = x;
                   }
                   else if (lifetime is ISingleViewApplicationLifetime single)
                   {
                       var mainWindow = TopLevel.GetTopLevel(single.MainView);
                       if (mainWindow.FlowDirection == x)
                           return;
                       mainWindow.FlowDirection = x;
                   }
               }, DispatcherPriority.Background);
           });

        this.WhenAnyValue(x => x.ListBoxColor)
           .Subscribe(async x =>
           {
               await Dispatcher.UIThread.InvokeAsync(() =>
               {
                   if (x != null)
                   {
                       CustomAccentColor = x.Value;
                       //UpdateAppAccentColor(x.Value);
                   }
               }, DispatcherPriority.Background);
           });

        this.WhenAnyValue(x => x.CustomAccentColor)
         .Subscribe(async x =>
         {
             await Dispatcher.UIThread.InvokeAsync(() =>
             {
                 ListBoxColor = x;
                 //UpdateAppAccentColor(x);
             }, DispatcherPriority.Background);
         });

        ////选择语言
        //this.WhenAnyValue(x => x.CurrentAppLanguage)
        //    .Skip(1)
        //    .Subscribe(async x =>
        //    {
        //        await Dispatcher.UIThread.InvokeAsync(() =>
        //        {
        //            var lang = x.ToString();

        //            //Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfo(lang);
        //            //_localizer.EditLn(lang);

        //            _localizationManager.CurrentCulture = new CultureInfo(lang);
        //            Globals.CultureInfo = new CultureInfo(lang);

        //            if (Host is MainView mianView)
        //            {
        //                mianView.NavigateTo(typeof(HomePageViewModel));
        //            }

        //        }, DispatcherPriority.Background);
        //    });




        //保存
        SaveRSTPServer = ReactiveCommand.CreateFromTask(async () =>
        {
            await Dispatcher.UIThread.InvokeAsync(() =>
            {
                _settingsProvider.Settings.HostUrl = HostUrl;
                _settingsProvider.Settings.ServerHost = ServerHost;
                _settingsProvider.Settings.ServerUdpPort = ServerUdpPort;
                _settingsProvider.Settings.TCPSendReceiver = TCPSendReceiver;
                _settingsProvider.Settings.RSTPServer = RSTPServer;
                _settingsProvider.Settings.DB_Conn = DB_Conn;
                _settingsProvider.Save();

            }, DispatcherPriority.Background);

            MessageBox("配置已经更改！");
        });


        this.WhenActivated((CompositeDisposable disposables) =>
        {
            RxApp.MainThreadScheduler.Schedule(LoadData).DisposeWith(disposables);
        });
    }

    public List<Color> PredefinedColors { get; private set; }

    public string CurrentVersion =>
        typeof(FluentAvalonia.UI.Controls.NavigationView).Assembly.GetName().Version?.ToString();

    public string CurrentAvaloniaVersion =>
        typeof(Application).Assembly.GetName().Version?.ToString();


    /// <summary>
    /// 读取摄像头设备信息
    /// </summary>
    private async void LoadData()
    {
        await Dispatcher.UIThread.InvokeAsync(() =>
        {
            var setting = _settingsProvider.Settings;
            HostUrl = setting.HostUrl;
            ServerHost = setting.ServerHost;
            ServerUdpPort = setting.ServerUdpPort;
            TCPSendReceiver = setting.TCPSendReceiver;
            RSTPServer = setting.RSTPServer;
            DB_Conn = setting.DB_Conn;


        }, DispatcherPriority.Background);
    }

    protected override void DisposeManaged() { }
    protected override void DisposeUnmanaged() { }
}

