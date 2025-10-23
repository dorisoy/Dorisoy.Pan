using Dorisoy.Pan.Core;

namespace Dorisoy.Pan.ViewModels;

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
    public string DevelopedBy => $"XXX器械集团（Dorisoy）版权所有 © 1956-{DateTime.Now.Year}";

    public ReactiveCommand<Unit, Unit> Login { get; }
    public ReactiveCommand<Unit, Unit> Exit { get; }
    public NavigationFactory NavigationFactory { get; }


    protected ObservableAsPropertyHelper<bool> isBusy;
    public bool IsBusy { get { return isBusy.Value; } }

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

#if LOCALHOST
        Username = "admin@dorisoy.com";
        Password = "admin@123";
#elif REMOTEHOST
        Username = "test@dorisoy.com";
        Password = "admin@123";
#else
        Username = "test@dorisoy.com";
        Password = "admin@123";
#endif

        var canLogin = this.WhenAnyValue(vm => vm.Username,
            vm => vm.Password,
            (userName, password) => !string.IsNullOrEmpty(userName)
            && !string.IsNullOrEmpty(password) && _authenticationService != null);

        //模式选择
        this.WhenAnyValue(x => x.ClientMode).Skip(1).Subscribe(x => { ClientMode = x; });

        Login = ReactiveCommand.CreateFromTask(LoginAsync, canLogin);
        Exit = ReactiveCommand.Create(ExitApplication);

        Login.IsExecuting.ToProperty(this, x => x.IsBusy, out isBusy);

        this.WhenActivated((CompositeDisposable disposables) =>
        {
        });
    }


    /// <summary>
    /// 登录
    /// </summary>
    /// <returns></returns>
    public async Task LoginAsync()
    {
        try
        {
            var server = _settingsProvider.Settings.ServerIP;
            if (!InternetCheck.PingIpOrDomainName(server))
            {
                MessageBox($"无法连接远程服务器::{server}.");
                return;
            }

            LoginButtonLabel = "正在登录...";
            var localIP = CommonHelper.GetLocalIP();
            var result = await _authenticationService?.LoginAsync(Username, Password, localIP.ToString());
            if (result != null)
            {
                //登记全局用户信息
                Globals.CurrentUser = result;
                Globals.CurrentUser.IP = localIP.ToString();

                LoginButtonLabel = "登录";
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

}
