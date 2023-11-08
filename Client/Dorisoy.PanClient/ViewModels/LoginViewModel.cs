using System.Diagnostics;
using Microsoft.AspNetCore.SignalR.Client;
using Newtonsoft.Json;
using Dorisoy.PanClient.Common;
using Dorisoy.PanClient.Utils;
using Dorisoy.PanClient.Views;

namespace Dorisoy.PanClient.ViewModels;

[View(typeof(LoginView))]
public class LoginViewModel : ViewModelBase, IRoutableViewModel
{
    private readonly IApplicationInfo _applicationInfo;
    private readonly ILoginService _loginService;
    private readonly ISettingsProvider<AppSettings> _settingsProvider;
    private readonly IAuthenticationService _authenticationService;
    public string UrlPathSegment { get; } = "Login";
    [Reactive] public string Username { get; set; }
    [Reactive] public string Password { get; set; }

    [Reactive] public ClientMode ClientMode { get; set; }
    [Reactive] public ClientMode[] ClientModes { get; set; } = new[] { ClientMode.Academy, ClientMode.Hospital };

    [Reactive] public string ErrorMessage { get; set; }
    [Reactive] public string LoginButtonLabel { get; set; } = "登录";

    public string Version => string.Format("Version {0}", _applicationInfo.Version);
    public string DevelopedBy => $"西诺医疗器械集团（Sinol）版权所有 © 1956-{DateTime.Now.Year}";

    public ReactiveCommand<Unit, Unit> Login { get; }
    public ReactiveCommand<Unit, Unit> Exit { get; }
    public NavigationFactory NavigationFactory { get; }

    public LoginViewModel(IScreen screen) : this(screen,
            Locator.Current.GetService<ILoginService>(),
            Locator.Current.GetService<IApplicationInfo>(),
            Locator.Current.GetService<IAuthenticationService>(),
            Locator.Current.GetService<ISettingsProvider<AppSettings>>())
    { }

    public LoginViewModel(IScreen screen,
        ILoginService loginService,
        IApplicationInfo applicationInfo,
        IAuthenticationService authenticationService,
        ISettingsProvider<AppSettings> settingsProvider) : base()
    {
        HostScreen = screen;

        _loginService = loginService;
        _applicationInfo = applicationInfo;
        _settingsProvider = settingsProvider;
        _authenticationService = authenticationService;

        ClientMode = _settingsProvider.Settings.ClientMode;

#if DEBUG
        Username = "admin@sinol.com";
        Password = "admin@123";
#endif

        var canLogin = this.WhenAnyValue(vm => vm.Username,
            vm => vm.Password,
            (userName, password) => !string.IsNullOrEmpty(userName) && !string.IsNullOrEmpty(password));

        //模式选择
        this.WhenAnyValue(x => x.ClientMode).Skip(1).Subscribe(x => { ClientMode = x; });

        Login = ReactiveCommand.CreateFromTask(LoginAsync, canLogin);
        Exit = ReactiveCommand.Create(ExitApplication);
        Login.IsExecuting.ToProperty(this, x => x.IsBusy, out isBusy);

        this.WhenActivated((CompositeDisposable disposables) => { GC.Collect(); });
    }


    /// <summary>
    /// 登录
    /// </summary>
    /// <returns></returns>
    public async Task LoginAsync()
    {
        try
        {
            var server = _settingsProvider.Settings.ServerHost;
            if (!InternetCheck.PingIpOrDomainName(server))
            {
                MessageBox($"无法连接远程服务器::{server}.");
                return;
            }

            if (_authenticationService == null)
            {
                MessageBox($"程序需要重新启动.");
                return;
            }

            LoginButtonLabel = "正在登录...";
            UserInfoToken token;

            var result = await _authenticationService.LoginAsync(Username, Password);
            if (result != null)
            {
                //登记全局用户信息
                Globals.CurrentUser = result;
                LoginButtonLabel = "登录";
                try
                {
                    token = new UserInfoToken
                    {
                        Id = result.Id,
                        Email = result.Email,
                        IP = Utilities.GetLocalIP()
                    };

                    if (_connection.State == HubConnectionState.Disconnected)
                        await _connection.StartAsync();

                    //登录通知
                    await _connection.SendAsync("Join", JsonConvert.SerializeObject(token));

                    //UDP服务登录ChatServer
                    //await LoginInCommandTask(result.Id.ToString(), result.UserName);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                }

                var mvv = new MainViewViewModel(HostScreen, result);

                RegisterMainView(Locator.CurrentMutable, mvv);

                await HostScreen.Router.Navigate.Execute(mvv);
            }
            else
            {
                MessageBox("无效的用户名或者密码！");
            }
        }
        catch (Exception ex)
        {
            MessageBox("服务器错误！");

        }
    }

    public IMutableDependencyResolver RegisterMainView(IMutableDependencyResolver services,
        MainViewViewModel mvv)
    {
        services.Register<IActivatableViewModel>(() => mvv, "MainView");
        return services;
    }

    private void ExitApplication()
    {
        Locator.Current.GetService<IApplicationCloser>().Shutdown();
    }
    protected override void DisposeManaged() { }
    protected override void DisposeUnmanaged() { }
}
