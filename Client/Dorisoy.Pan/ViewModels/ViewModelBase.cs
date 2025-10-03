using Dialog = FluentAvalonia.UI.Controls.Dialog;
using Frame = FluentAvalonia.UI.Controls.Frame;

namespace Dorisoy.PanClient.ViewModels;

public abstract class ViewModelBase : ReactiveObject, IValidatableViewModel, IFrameNavigatedFrom, IDisposable
{
    public ViewModelBase CurrentViewModel { get; set; }

    /// <summary>
    /// 获取导航服务
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    protected IActivatableViewModel GetService(string name)
    {
        return Locator.Current.GetService<IFrameNavigatedFrom>(name);
    }


    private readonly ISettingsProvider<AppSettings> _settingsProvider;
    private readonly IOnlineUserService _onlineUserService;

    public ReactiveCommand<CancellationToken, Unit> LoadDataCommand { get; set; }

    public UserControl Host { get; set; }
    public IScreen HostScreen { get; set; }

    private UserAuthDto _userAuthDto;
    public UserAuthDto CurrentUser
    {
        get => Globals.CurrentUser;
        set { _userAuthDto = value; }
    }

    private PatientModel _patientModel;
    public PatientModel CurrentPatient
    {
        get => Globals.CurrentPatient;
        set { _patientModel = value; }
    }

    public virtual ViewModelActivator Activator { get; }
    public ValidationContext ValidationContext { get; }

    /// <summary>
    /// Hub
    /// </summary>
    protected HubConnection _connection { get; }

    /// <summary>
    /// 在线用户
    /// </summary>
    public ObservableCollection<OnlinUserUserModel> OnlineUsersItems { get; set; } = new();


    protected WAMPlayer wamPlayer { get; set; }
    [Reactive] public int OnlineUsers { get; set; }


    public ViewModelBase()
    {
        _settingsProvider = Locator.Current.GetService<ISettingsProvider<AppSettings>>();
        _onlineUserService = Locator.Current.GetService<IOnlineUserService>();

        Activator = new ViewModelActivator();
        ValidationContext = new ValidationContext();

        var setting = _settingsProvider.Settings;
        var querystringData = new Dictionary<string, string>() { { "contosochatversion", "1.0" } };

        LoadDataCommand = ReactiveCommand.Create<CancellationToken>(LoadDataAsync);

        #region Hub

        //初始化UserHub
        var _huburl = setting.GetHub();
        var user = CurrentUser;

        //AddMessagePackProtocol() 是使用快速和精简的二进制序列化格式进行传输。
        _connection = new HubConnectionBuilder()
                   .WithUrl(_huburl + "?userId=" + user?.Id + "&email=" + user?.Email + "&ip=" + user?.IP + "")
                   .WithAutomaticReconnect()
                   .AddMessagePackProtocol()
                   .AddNewtonsoftJsonProtocol(opts =>
                   {
                       opts.PayloadSerializerSettings.TypeNameHandling = TypeNameHandling.Auto;
                       opts.PayloadSerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                   })
                   .Build();

        //设置最大并发连接数
        ServicePointManager.DefaultConnectionLimit = 50;

        _connection.Closed += connectionOnClosed();

        //新用户加入
        _connection.On<UserInfoToken>("Joined", (user) =>
        {
            var localIP = CommonHelper.GetLocalIP();
            if (user.IP != localIP.ToString())
            {
                MessageBox($"用户 {user.Email} 上线了！");
            }
        });

        //在线用户
        _connection.On<IEnumerable<OnlinUserUserModel>>("OnlineUsers", async (users) =>
        {
            await Dispatcher.UIThread.InvokeAsync(() =>
            {
                if (users != null && users.Any())
                {
                    users.ToList().ForEach(s =>
                    {
                        s.RaleName = !string.IsNullOrEmpty(s.RaleName) && s.RaleName.Equals(CurrentUser?.RaleName) ? "我" : s.RaleName;
                    });
                    OnlineUsersItems = new ObservableCollection<OnlinUserUserModel>(users);
                    //包括自己
                    Globals.Onlines = users.Count();
                    OnlineUsers = users.Count();
                    Globals.OnlineUsers = users.Select(s => s.Id).ToList();
                }
            });
        });

        ////订阅消息通知
        //_connection.On<string, UserInfoToken>("OnSubscribeMessage", (msg, user) =>
        //{
        //    MessageBox($"你有新的消息！");
        //});

        ////订阅视频推送
        //_connection.On<string, Guid, string, UserInfoToken, bool, UserControl>("OnSubscribeVideo", async (url, suserId, suser, user, pushing, host) =>
        //{
        //    await Dispatcher.UIThread.InvokeAsync(async () =>
        //    {
        //        try
        //        {
        //            Globals.ReferenceUserId = suserId;
        //            if (!pushing)
        //            {
        //                var ok = await ConfirmBox($"{suser} 邀请你预览视频,是否接受？");
        //                if (ok == DialogResult.Primary)
        //                {
        //                    var chost = App.MainView;
        //                    //导航到视频
        //                    if (chost is MainView mianView)
        //                    {
        //                        if (mianView.mainPage is not MonitorPageViewModel)
        //                        {
        //                            mianView.NavigateTo(typeof(MonitorPageViewModel));
        //                        }
        //                        else
        //                        {
        //                            var mpv = mianView.mainPage as MonitorPageViewModel;

        //                            //开启接收UDP视频流
        //                            mpv?.StartReceived();

        //                        }
        //                    }
        //                }
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            MessageBox(ex.Message);
        //        }
        //    });
        //});

        #endregion
    }

    protected virtual void LoadDataAsync(CancellationToken token) { }

    //protected async void LoadOnlineUserData()
    //{
    //    //获取在线用户
    //    var users = await _onlineUserService.GetOnlineUsers();
    //    if (users != null && users.Count > 0)
    //    {
    //        users.ForEach(s => { s.RaleName = s.RaleName.Equals(CurrentUser?.RaleName) ? "我" : s.RaleName; });
    //        OnlineUsersItems = new ObservableCollection<OnlinUserUserModel>(users);
    //        Globals.Onlines = users.Count();
    //        Globals.OnlineUsers = users.Select(s => s.Id).ToList();
    //    }
    //}

    /// <summary>
    /// 异步方法以启动连接信令Hub
    /// </summary>
    public async void ConnectAsync()
    {
        try
        {
            if (_connection != null && _connection.State == HubConnectionState.Disconnected)
                await _connection.StartAsync();
        }
        catch (Exception)
        {
            //Show($"连接到SignalR服务时出错:{ex.Message}");
        }
    }

    /// <summary>
    /// 异步方法以停止连接
    /// </summary>
    /// <returns></returns>
    public async Task DisconnectAsync()
    {
        if (_connection != null)
        {
            await _connection.StopAsync();
            await _connection.DisposeAsync();
        }
    }

    /// <summary>
    /// 断开后重新连接
    /// </summary>
    /// <returns></returns>
    private Func<Exception, Task> connectionOnClosed()
    {
        return async (error) =>
        {
            await Task.Delay(new Random().Next(0, 5) * 1000);
            try
            {
                await _connection?.StartAsync();
            }
            catch (Exception e) { }
        };
    }



    protected string GetAssemblyResource(string name)
    {
        using var stream = AssetLoader.Open(new Uri(name));
        using StreamReader reader = new(stream);
        return reader.ReadToEnd();
    }

    /// <summary>
    /// 消息提示框
    /// </summary>
    /// <param name="msg"></param>
    /// <param name="title"></param>
    public void MessageBox(string msg, string title = "提示")
    {
        Dispatcher.UIThread.Invoke(() =>
        {
            try
            {
                var resultHint = new Dialog()
                {
                    Content = msg,
                    Title = title,
                    PrimaryButtonText = "确认"
                };
                _ = resultHint.ShowAsync();
            }
            catch (Exception) { }
        });
    }

    /// <summary>
    /// 确认框
    /// </summary>
    /// <param name="msg"></param>
    /// <param name="title"></param>
    /// <returns></returns>
    protected async Task<DialogResult> ConfirmBox(string msg, string title = "提示")
    {
        var resultHint = new Dialog()
        {
            Content = msg,
            Title = title,
            PrimaryButtonText = "确认",
            CloseButtonText = "取消"
        };
        return await resultHint.ShowAsync();
    }

    /// <summary>
    /// 授权检查
    /// </summary>
    protected bool CheckAuthorize(string policyName)
    {
        if (CurrentUser.IsAdmin)
            return true;

        var auth = CurrentUser.Claims.Select(s => s.ClaimValue).Contains(policyName);
        if (!auth)
            MessageBox("无操作权限");

        return auth;
    }


    public virtual void OnLoaded() { }
    public virtual Task OnLoadedAsync()
    {
        return Task.CompletedTask;
    }


    private bool _disposed = false;
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
    private void Dispose(bool disposing)
    {
        if (_disposed)
        {
            if (disposing)
            {
                DisposeManaged();
            }

            DisposeUnmanaged();
            _disposed = true;
        }
    }

    /// <summary>
    /// 释放托管资源
    /// </summary>
    protected virtual void DisposeManaged() { }

    /// <summary>
    /// 释放非托管资源
    /// </summary>
    protected virtual void DisposeUnmanaged() { }

    ~ViewModelBase()
    {
        Dispose(false);
    }
}


public abstract class ViewModelBase<TModel> : ViewModelBase
{
    [Reactive] public TModel Model { get; set; }
    public ViewModelBase(TModel model) : base()
    {
        Model = model;
    }
}

public enum ExportType
{
    Excel,
    CSV
}
public abstract class MainPageViewModelBase : ViewModelBase
{
    [Reactive] public string NavHeader { get; set; }
    [Reactive] public string IconKey { get; set; }
    [Reactive] public bool ShowsInFooter { get; set; }

    protected ReactiveCommand<Unit, Unit> PrintPage { get; set; }
    protected ReactiveCommand<Unit, Unit> ExportCvs { get; set; }
    protected ReactiveCommand<Unit, Unit> ExportExcel { get; set; }


    public string PageName { get; }

    public MainPageViewModelBase() : base()
    {
        PageName = this.GetType().Name;
    }


    /// <summary>
    /// 导出数据
    /// </summary>
    public void Export<T>(List<T> data, string[] columns, ExportType type) where T : class
    {
        try
        {
            var ext = ".xlsx";
            switch (type)
            {
                case ExportType.Excel:
                    {
                        ext = ".xlsx";
                        var name = $"Export_{DateTime.Now.ToString("yyyyMMdd")}{ext}";
                        byte[] filecontent = ExcelExportHelper.ExportExcel(data, "", false, columns);
                        Dorisoy.PanClient.Controls.Dialog.Save("导出Excel数据", name, filename =>
                        {
                            using var fs = new FileStream(filename, FileMode.Create, FileAccess.Write);
                            fs.Write(filecontent, 0, filecontent.Length);
                            MessageBox($"导出成功！");
                        });
                    }
                    break;
                case ExportType.CSV:
                    {
                        ext = ".csv";
                        var name = $"Export_{DateTime.Now.ToString("yyyyMMdd")}{ext}";
                        Dorisoy.PanClient.Controls.Dialog.Save("导出Excel数据", name, filename =>
                        {
                            ExcelExportHelper.ExportExcel(data, filename, columns);
                            MessageBox($"导出成功！");
                        });
                    }
                    break;
            }
        }
        catch (Exception ex)
        {
            MessageBox($"导出数据失败:{ex.Message}");
        }
    }

    /// <summary>
    /// 打印页面
    /// </summary>
    /// <param name="pageName"></param>
    public void Printing(string pageName)
    {
        Dorisoy.PanClient.Controls.Dialog.Save("将当前页面另存为图片", $"{pageName}.png", filename =>
        {
            var overlayHost = App.MainWindow.FindAllVisuals<Frame>();
            var ok = Print.ToFile(filename, overlayHost);
            if (ok)
            {
                //MessageBox("保存成功！");
                PrintFilePath = filename;
                PrintDocument pd = new PrintDocument();
                pd.PrintPage += Pd_PrintPage;
                pd.Print();
            }
        });
    }

    private string PrintFilePath { get; set; }
    private void Pd_PrintPage(object sender, PrintPageEventArgs e)
    {
        try
        {
            //打开文件
            using FileStream fs = File.OpenRead(PrintFilePath);
            var filelength = (int)fs.Length;
            var imageByte = new byte[filelength];
            fs.Read(imageByte, 0, filelength);
            using System.Drawing.Image image = System.Drawing.Image.FromStream(fs);
            fs.Close();
            e.Graphics.DrawImage(image, 0, 0);
            e.HasMorePages = false;
        }
        catch (Exception ex)
        { }
    }

}
public abstract class PageBaseViewModel : ViewModelBase
{
    [Reactive] public MainPageViewModelBase Parent { get; set; }
    public string PageKey { get; init; }
    public PageBaseViewModel() : base() { }
}

