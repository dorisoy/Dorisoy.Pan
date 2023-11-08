using System.Drawing.Printing;
using System.Net;
using Avalonia.Controls;
using Avalonia.Platform;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using ReactiveUI.Validation.Contexts;
using Dorisoy.PanClient.Common;
using Dorisoy.PanClient.PrintToPDF;
using Dorisoy.PanClient.Views;

namespace Dorisoy.PanClient.ViewModels;

public abstract class ViewModelBase : ReactiveObject, IValidatableViewModel, IFrameNavigatedFrom, IDisposable
{

    //public new event PropertyChangedEventHandler PropertyChanged;
    //protected void OnPropertyChanged(string PropertyName)
    //{
    //    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(PropertyName));
    //}


    public ViewModelBase CurrentViewModel { get; set; }

    protected ObservableAsPropertyHelper<bool> isBusy;
    public bool IsBusy { get { return isBusy.Value; } }
    private readonly ISettingsProvider<AppSettings> _settingsProvider;

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

    public ViewModelBase()
    {
        _settingsProvider = Locator.Current.GetService<ISettingsProvider<AppSettings>>();

        Activator = new ViewModelActivator();
        ValidationContext = new ValidationContext();

        var setting = _settingsProvider.Settings;
        var querystringData = new Dictionary<string, string>() { { "contosochatversion", "1.0" } };


        #region Hub

        var _huburl = setting.HostUrl.EndsWith("/") ? (setting.HostUrl + "userHub") : (setting.HostUrl + "/userHub");

        //AddMessagePackProtocol() 是使用快速和精简的二进制序列化格式进行传输。
        _connection = new HubConnectionBuilder()
                   .WithUrl(_huburl)
                   .AddNewtonsoftJsonProtocol(opts =>
                   {
                       opts.PayloadSerializerSettings.TypeNameHandling = TypeNameHandling.Auto;
                       opts.PayloadSerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                   })
                   .Build();

        //设置最大并发连接数
        ServicePointManager.DefaultConnectionLimit = 10;

        _connection.Closed += connectionOnClosed();

        //新用户加入
        _connection.On<UserInfoToken>("Joined", async (user) =>
        {
            await Dispatcher.UIThread.InvokeAsync(async () =>
            {
                if (_connection.State == HubConnectionState.Connected)
                    await _connection.InvokeAsync("Join", user);

            });
        });

        //在线用户
        _connection.On<IEnumerable<UserInfoToken>>("OnlineUsers", async (users) =>
        {
            await Dispatcher.UIThread.InvokeAsync(() =>
            {
                //包括自己
                Globals.Onlines = users.Count() + 1;
                Globals.OnlineUsers = users.Select(s => s.Id).ToList();
            });
        });

        //订阅消息通知
        _connection.On<string, UserInfoToken>("OnSubscribeMessage", async (msg, user) =>
        {
            await Dispatcher.UIThread.InvokeAsync(() =>
            {
                MessageBox($"你有新的消息！");
            });
        });

        //订阅视频推送
        _connection.On<string, Guid, string, UserInfoToken, bool, UserControl>("OnSubscribeVideo", async (url, suserId, suser, user, pushing, host) =>
        {
            await Dispatcher.UIThread.InvokeAsync(async () =>
            {
                try
                {
                    Globals.ReferenceUserId = suserId;
                    if (!pushing)
                    {
                        var ok = await ConfirmBox($"{suser} 邀请你预览视频,是否接受？");
                        if (ok == DialogResult.Primary)
                        {
                            var chost = App.MainView;
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox(ex.Message);
                }
            });
        });

        #endregion



        //Activated
        this.WhenActivated(async (CompositeDisposable disposables) =>
        {
            try
            {
                if (_connection.State == HubConnectionState.Disconnected)
                    await _connection.StartAsync();
            }
            catch (Exception) { }

            //
            //var bits = Environment.Is64BitOperatingSystem ? "PC 64bit, " : "PC 32bit, ";
            //var operatingSystem = bits + RuntimeInformation.OSDescription;
            //var nameVersionClient = "Dorisoy.PanClient v1.0";
            //await _hub.Login(CurrentUser.BearerToken, operatingSystem, "127.0.0.0", nameVersionClient);
        });
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
                await _connection.StartAsync();
            }
            catch (Exception e)
            {
                MessageBox($"Error: {e}");
            }
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
    public virtual void OnUnloaded() { }
    public async virtual Task OnUnloadedAsync()
    {
        await _connection?.SendAsync("CanclePushVideo", Globals.ReferenceUserId);
    }



    private bool _disposed = false;
    public void Dispose()
    {
        Dispose(true);
        //This object will be cleaned up by the Dispose method.
        //Therefore, you should call GC.SupressFinalize to
        //take this object off the finalization queue
        //and prevent finalization code for this object
        //from executing a second time.
        GC.SuppressFinalize(this);
    }

    // Dispose(bool disposing) executes in two distinct scenarios.
    // If disposing equals true, the method has been called directly
    // or indirectly by a user's code. Managed and unmanaged resources
    // can be disposed.
    // If disposing equals false, the method has been called by the
    // runtime from inside the finalizer and you should not reference
    // other objects. Only unmanaged resources can be disposed.
    private void Dispose(bool disposing)
    {
        // Check to see if Dispose has already been called.
        if (_disposed)
        {
            // If disposing equals true, dispose all managed
            // and unmanaged resources.
            if (disposing)
            {
                // Dispose managed resources.
                DisposeManaged();
            }

            // Call the appropriate methods to clean up
            // unmanaged resources here.
            // If disposing is false,
            // only the following code is executed.
            DisposeUnmanaged();

            // Note disposing has been done.
            _disposed = true;
        }
    }

    // Use C# destructor syntax for finalization code.
    // This destructor will run only if the Dispose method
    // does not get called.
    // It gives your base class the opportunity to finalize.
    // Do not provide destructors in types derived from this class.
    protected abstract void DisposeManaged();
    protected abstract void DisposeUnmanaged();

    ~ViewModelBase()
    {
        // Do not re-create Dispose clean-up code here.
        // Calling Dispose(false) is optimal in terms of
        // readability and maintainability.
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
                        Controls.Dialog.Save("导出Excel数据", name, filename =>
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
                        Controls.Dialog.Save("导出Excel数据", name, filename =>
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
        Controls.Dialog.Save("将当前页面另存为图片", $"{pageName}.png", filename =>
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

