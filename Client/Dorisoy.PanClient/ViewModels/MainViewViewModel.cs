using System.Globalization;
using Avalonia.Controls;
using LocalizationManager;
using Microsoft.AspNetCore.SignalR.Client;
using Dorisoy.PanClient.Views;

namespace Dorisoy.PanClient.ViewModels;

[View(typeof(MainView))]
public class MainViewViewModel : ViewModelBase, IRoutableViewModel
{
    public NavigationFactory NavigationFactory { get; }
    private readonly ILocalizationManager _localizationManager;
    public string UrlPathSegment => nameof(MainViewViewModel);
    public RoutingState Router { get; } = new RoutingState();
    public UserAuthDto UserInformation { get; }
    public List<MainPageViewModelBase> Pages { get; set; } = new();
    [Reactive] public ObservableCollection<NavigationViewItemBase> MenuItems { get; set; } = new();
    [Reactive] public ObservableCollection<NavigationViewItemBase> FooterItems { get; set; } = new();

    [Reactive] public ReactiveCommand<Unit, Unit> Logout { get; set; }
    [Reactive] public ReactiveCommand<string, Unit> SelectLanguage { get; set; }
    public MainViewViewModel(IScreen screen, UserAuthDto userInformation) : base()
    {
        _localizationManager = LocalizationManagerExtensions.Default!;

        NavigationFactory = new NavigationFactory(this);
        HostScreen = screen;
        UserInformation = userInformation;

        InitializeNavigationPages();

        //注销
        Logout = ReactiveCommand.CreateFromTask(async () =>
        {
            if (_connection.State == HubConnectionState.Connected)
                await _connection.InvokeAsync("Logout", Globals.CurrentUser.Id);

            Globals.CurrentUser = null;
            await HostScreen.Router.Navigate.Execute(new LoginViewModel(HostScreen));
        });

        //选择语言
        SelectLanguage = ReactiveCommand.CreateFromTask<string>(async (x) =>
        {
            await Dispatcher.UIThread.InvokeAsync(() =>
            {
                var lang = x.ToString();

                _localizationManager.CurrentCulture = new CultureInfo(lang);
                Globals.CultureInfo = new CultureInfo(lang);

                App.MainView.NavigateTo(typeof(HomePageViewModel));

            }, DispatcherPriority.Background);
        });
        this.WhenActivated((CompositeDisposable disposables) => { });
    }

    /// <summary>
    /// 初始化导航页面
    /// </summary>
    public void InitializeNavigationPages()
    {
        var mainPages = new List<MainPageViewModelBase>();
        var menuItems = new List<NavigationViewItemBase>();
        var footerItems = new List<NavigationViewItemBase>();

        var canUsersView = CurrentUser.Authorize(Permissions.Users.View);
        var canRolesView = CurrentUser.Authorize(Permissions.Roles.View);
        var canRoleClaimsView = CurrentUser.Authorize(Permissions.RoleClaims.View);
        var canDocumentsView = CurrentUser.Authorize(Permissions.Documents.View);
        var canSettingsView = CurrentUser.Authorize(Permissions.Settings.View);

        var home = GetService("HomePage") as HomePageViewModel;
        var user = GetService("UserPage") as UserPageViewModel;
        var role = GetService("RolePage") as RolePageViewModel;
        var permission = GetService("PermissionPage") as PermissionPageViewModel;
        var document = GetService("DocumentPage") as DocumentPageViewModel;
        var settings = GetService("SettingsPage") as SettingsPageViewModel;


        //首页
        home.NavHeader = "HomePage";
        home.IconKey = "HomeIconFilled";

        //用户
        user.NavHeader = "UserPage";
        user.IconKey = "PeopleIconFilled";

        //角色
        role.NavHeader = "RolePage";
        role.IconKey = "SpeechSolidBoldIconFilled";

        //权限
        permission.NavHeader = "PermissionPage";
        permission.IconKey = "DefenderAppIconFilled";

        //存储
        document.NavHeader = "DocumentPage";
        document.IconKey = "FolderIconFilled";

        //设置
        settings.NavHeader = "SettingsPage";
        settings.IconKey = "SettingsIconFilled";
        settings.ShowsInFooter = true;

        //home
        mainPages.Add(home);

        //user
        if (canUsersView)
            mainPages.Add(user);

        //role
        if (canRolesView)
            mainPages.Add(role);

        //permission 
        if (canRoleClaimsView)
            mainPages.Add(permission);

        //document
        if (canDocumentsView)
            mainPages.Add(document);


        //settings
        if (canSettingsView)
            mainPages.Add(settings);

        foreach (var pg in mainPages)
        {
            var nvi = new NavigationViewItem
            {
                Content = pg.NavHeader,
                Name = pg.NavHeader,
                Tag = pg,
                IconSource = Avalonia.Application.Current.FindResource("") as IconSource
            };

            ToolTip.SetTip(nvi, pg.NavHeader);

            nvi.Classes.Add("SinolAppNav");

            if (pg.ShowsInFooter)
                footerItems.Add(nvi);
            else
            {
                menuItems.Add(nvi);
            }
        }

        MenuItems = new ObservableCollection<NavigationViewItemBase>(menuItems);
        FooterItems = new ObservableCollection<NavigationViewItemBase>(footerItems);
    }

    private IActivatableViewModel GetService(string name)
    {
        return Locator.Current.GetService<IFrameNavigatedFrom>(name);
    }

    protected override void DisposeManaged()
    {
    }

    protected override void DisposeUnmanaged() 
    {
    }
}


public class NavigationFactory : INavigationPageFactory
{
    public MainViewViewModel Owner { get; }
    public NavigationFactory(MainViewViewModel owner)
    {
        Owner = owner;
    }

    public Control GetPage(Type srcType)
    {
        return null;
    }

    public Control GetPageFromObject(object target)
    {
        if (target is HomePageViewModel)
            return new HomePage { DataContext = target };
        else if (target is SettingsPageViewModel)
            return new SettingsPage { DataContext = target };
        else if (target is UserPageViewModel)
            return new UserPage { DataContext = target };
        else if (target is RolePageViewModel)
            return new RolePage { DataContext = target };
        else if (target is PermissionPageViewModel)
            return new PermissionPage { DataContext = target };
        else if (target is DocumentPageViewModel)
            return new DocumentPage { DataContext = target };
        else
            return ResolvePage(target as PageBaseViewModel);
    }

    /// <summary>
    /// 解析页面
    /// </summary>
    /// <param name="pbvm"></param>
    /// <returns></returns>
    private Control ResolvePage(PageBaseViewModel pbvm)
    {
        if (pbvm is null)
            return null;

        Control page = null;
        var key = pbvm.PageKey;
        return page;
    }
}
