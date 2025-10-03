using Color = Avalonia.Media.Color;


namespace Dorisoy.PanClient.ViewModels;

public class SettingsPageViewModel : MainPageViewModelBase
{

    private bool _ignoreSetListBoxColor = false;
    private readonly ILocalizationManager _localizationManager;
    private readonly ISettingsProvider<AppSettings> _settingsProvider;

    public PersistentSettingConfig Config { get; set; } = PersistentSettingConfig.Instance;

    public ReactiveCommand<Unit, Unit> SaveRSTPServer { get; }

    public ThemeVariant[] AppThemes { get; } = [ThemeVariant.Light];
    public FlowDirection[] AppFlowDirections { get; } = [
        FlowDirection.LeftToRight,
        FlowDirection.RightToLeft
    ];

    [Reactive] public bool UseCustomAccent { get; set; }
    [Reactive] public ThemeVariant CurrentAppTheme { get; set; } = ThemeVariant.Light;
    [Reactive] public FlowDirection CurrentFlowDirection { get; set; }
    [Reactive] public Color? ListBoxColor { get; set; }
    [Reactive] public Color CustomAccentColor { get; set; }


    /// <summary>
    /// 视频信号源控制
    /// </summary>
    [Reactive] public ObservableCollection<CameraDevice> CameraDevices { get; set; }
    [Reactive] public CameraDevice SelectCamera { get; set; }
    [Reactive] public CameraDevice SelectPipCamera { get; set; }


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


    //====================

    private static SettingsPageViewModel instance;
    public static SettingsPageViewModel Instance
    {
        get
        {
            instance ??= new SettingsPageViewModel();
            return instance;
        }
    }


    //private ReadOnlyObservableCollection<DocumentFolderModel> _items;
    //public ReadOnlyObservableCollection<DocumentFolderModel> Items => _items;

    private CancellationTokenSource _cts;

    public SettingsPageViewModel() : base()
    {
        _localizationManager = LocalizationManagerExtensions.Default!;
        _settingsProvider = Locator.Current.GetService<ISettingsProvider<AppSettings>>();

        CameraDevices = new();
        RSTPServer = _settingsProvider.Settings.GetRSTPServer();

        var setting = _settingsProvider.Settings;
        if (setting != null)
        {
            HostUrl = setting.GetHost();
            ServerHost = setting.ServerIP;
            ServerUdpPort = setting.ServerUdpPort;
            TCPSendReceiver = setting.TCPSendReceiver;
            RSTPServer = setting.GetRSTPServer();
            DB_Conn = setting.GetDBConn();
        }

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

        //选择切换视频设备
        this.WhenAnyValue(x => x.SelectCamera)
            .WhereNotNull()
            .Subscribe(async x =>
            {
                await Dispatcher.UIThread.InvokeAsync(() =>
                  {
                      _settingsProvider.Settings.CameraDefault = x;
                      _settingsProvider.Save();

                  }, DispatcherPriority.Background);
            });


        //保存
        SaveRSTPServer = ReactiveCommand.CreateFromTask(async () =>
        {
            await Dispatcher.UIThread.InvokeAsync(() =>
            {
                _settingsProvider.Settings.HostUrl = HostUrl;
                _settingsProvider.Settings.ServerIP = ServerHost;
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
            _cts = new CancellationTokenSource();

            //_documentService
            //.Connect()
            //.Sort(SortExpressionComparer<DocumentFolderModel>
            //.Descending(s => s.DocType).ThenByDescending(s => s.CreatedDate))
            //.Bind(out _items)
            //.Subscribe(s =>
            //{
            //    //
            //})
            //.DisposeWith(disposables);


            LoadDataCommand.Execute(_cts.Token)
            .Subscribe()
            .DisposeWith(disposables);
        });
    }


    /// <summary>
    /// 载入数据
    /// </summary>
    /// <returns></returns>
    protected override void LoadDataAsync(CancellationToken token)
    {
        Task.Run(async () =>
        {
            var setting = _settingsProvider.Settings;
            //读取摄像头
            var cameras = CameraDevicesEnumerator.EnumerateDevices();

            await Dispatcher.UIThread.InvokeAsync(() =>
            {
                CameraDevices = new ObservableCollection<CameraDevice>(cameras);
                SelectCamera = setting.CameraDefault;
                HostUrl = setting.GetHost();
                ServerHost = setting.ServerIP;
                ServerUdpPort = setting.ServerUdpPort;
                TCPSendReceiver = setting.TCPSendReceiver;
                RSTPServer = setting.GetRSTPServer();
                DB_Conn = setting.GetDBConn();
            });
        }, token);
    }


    public List<Color> PredefinedColors { get; private set; }
    public string CurrentVersion =>
        typeof(NavigationView).Assembly.GetName().Version?.ToString();
    public string CurrentAvaloniaVersion =>
        typeof(Application).Assembly.GetName().Version?.ToString();

}

