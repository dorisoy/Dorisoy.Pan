using Avalonia.Animation;
using FluentAvalonia.Core;
using FluentAvalonia.UI.Media.Animation;
using FluentAvalonia.UI.Navigation;
using Dorisoy.PanClient.ViewModels;
using Cue = Avalonia.Animation.Cue;
using Frame = FluentAvalonia.UI.Controls.Frame;
using Image = Avalonia.Controls.Image;

namespace Dorisoy.PanClient.Views;

public partial class MainView : ReactiveUserControl<MainViewViewModel>
{

    public MainView()
    {
        this.InitializeComponent();

        myTitleBarHost = this.FindControl<Grid>("myTitleBarHost");
        myNavView = this.FindControl<NavigationView>("myNavView");
        myFrameView = this.FindControl<Frame>("myFrameView");
        myOutgoingCall = this.FindControl<OutgoingCall>("myOutgoingCall");
        myIncomingCall = this.FindControl<IncomingCall>("myIncomingCall");
        myTalk = this.FindControl<Talk>("myTalk");
        myWindowIcon = this.FindControl<Image>("myWindowIcon");
        myTitleBarHost.IsVisible = true;
        myNavView.IsVisible = true;

        this.WhenActivated(disposables =>
        {
            var vm = ViewModel;
            myOutgoingCall.DataContext = vm;
            myIncomingCall.DataContext = vm;
            myTalk.DataContext = vm;
        });
    }


    public IStorageProvider GetStorageProvider()
    {
        return TopLevel.GetTopLevel(this).StorageProvider;
    }

    public IMutableDependencyResolver RegisterStorageProvider(IMutableDependencyResolver services)
    {
        services.RegisterLazySingleton(() => GetStorageProvider());
        return services;
    }

    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);

        //ճ����
        ClipboardService.Owner = TopLevel.GetTopLevel(this);

        // �򵥵ļ��-��Ӧ�ó������������汾������һ�����ڣ���ΪTopLevel Mobile��WASM������������
        _isDesktop = TopLevel.GetTopLevel(this) is Window;

        var vm = Locator.Current.GetService<IActivatableViewModel>("MainView") as MainViewViewModel;
        if (vm != null)
        {
            DataContext = vm;
            myFrameView.NavigationPageFactory = vm.NavigationFactory;
        }

        // �������ϣ����ڽ���splashscreen�ڼ���ô�����
        //if (e.Root is AppWindow aw)
        //{
        //    //var mass = aw.SplashScreen as MainAppSplashScreen;
        //    //mass.InitApp += () =>
        //    //{
        //    //    Dispatcher.UIThread.Post(() =>
        //    //    {
        //    //    });
        //    //};
        //    //mass.RunTasks(new CancellationToken());
        //}

        myNavView.Classes.Add("DorisoyAppNav");

        //NavView.PaneDisplayMode = NavigationViewPaneDisplayMode.LeftCompact;

        myFrameView.NavigateFromObject((myNavView.MenuItemsSource.ElementAt(0) as Control).Tag);

        myFrameView.Navigated += OnFrameViewNavigated;
        myFrameView.Navigating += Frame_Navigating;

        myNavView.ItemInvoked += OnNavigationViewItemInvoked;
        myNavView.BackRequested += OnNavigationViewBackRequested;

        NavigationService.Instance.SetFrame(myFrameView);


        //Ĭ�ϵ�����HomePage
        //myFrameView.Navigate(typeof(HomePage));

        //
        NavigationService.Instance.Navigate(typeof(HomePage));

        //ģ�ⴰ��
        //NavigationService.Instance.SetOverlayHost(OverlayHost);
    }

    private void Frame_Navigating(object sender, NavigatingCancelEventArgs e)
    {
        UpdateNavigationLocalization();
    }

    protected override void OnLoaded(RoutedEventArgs e)
    {
        base.OnLoaded(e);
        if (VisualRoot is AppWindow aw)
        {
            myTitleBarHost.ColumnDefinitions[4].Width = new GridLength(aw.TitleBar.RightInset, GridUnitType.Pixel);
            App.MainView = this;
        }
    }

    protected override void OnPointerReleased(PointerReleasedEventArgs e)
    {
        var pt = e.GetCurrentPoint(this);

        // ֡����X1->BackRequested�Զ������ǿ����ڴ˴�����X2������ǰ�򵼺�
        if (pt.Properties.PointerUpdateKind == PointerUpdateKind.XButton2Released)
        {
            if (myFrameView.CanGoForward)
            {
                myFrameView.GoForward();
                e.Handled = true;
            }
        }
        base.OnPointerReleased(e);
    }

    /// <summary>
    /// ����
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void OnNavigationViewBackRequested(object sender, NavigationViewBackRequestedEventArgs e)
    {
        myFrameView.GoBack();
    }

    /// <summary>
    /// ��������
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void OnNavigationViewItemInvoked(object sender, NavigationViewItemInvokedEventArgs e)
    {
        // ����ǰ��ѡ��Ŀ���Ļ�������SetNVIIcon�����ͷ�ΪNavigationViewItem��false����
        if (e.InvokedItemContainer is NavigationViewItem nvi)
        {
            NavigationTransitionInfo info;
            info = e.RecommendedNavigationTransitionInfo;
            NavigationService.Instance.NavigateFromContext(nvi.Tag, info, this);
        }
    }

    public MainPageViewModelBase mainPage = null;

    private void OnFrameViewNavigated(object sender, NavigationEventArgs e)
    {
        var localize = LocalizationManagerExtensions.Default;
        var page = e.Content as Control;
        var dc = page.DataContext;

        if (dc is MainPageViewModelBase mpvmb)
            mainPage = mpvmb;

        if (dc is PageBaseViewModel pbvm)
            mainPage = pbvm.Parent;

        foreach (NavigationViewItem nvi in myNavView.MenuItemsSource)
        {
            var nt = nvi.Tag.ToString();
            var mn = mainPage.GetType().FullName;
            SetNVIIcon(nvi, (nt == mn));
        }

        foreach (NavigationViewItem nvi in myNavView.FooterMenuItemsSource)
        {
            if (nvi.Name == "SettingsPage")
            {
                nvi.Content = localize.GetValue("SettingsPage");
                SetNVIIcon(nvi, true);
            }
            else
            {
                SetNVIIcon(nvi, false);
            }
        }

        if (myFrameView.BackStackDepth > 0 && !myNavView.IsBackButtonVisible)
        {
            AnimateContentForBackButton(true);
        }
        else if (myFrameView.BackStackDepth == 0 && myNavView.IsBackButtonVisible)
        {
            AnimateContentForBackButton(false);
        }
    }

    /// <summary>
    /// ���µ��������ػ�
    /// </summary>
    public void UpdateNavigationLocalization()
    {
        var localize = LocalizationManagerExtensions.Default;

        foreach (var item in myNavView.MenuItemsSource.Cast<NavigationViewItem>())
        {
            var token = ((ViewModelBase)item.Tag).GetType().Name.Replace("ViewModel","");
            item.Content = localize.GetValue(token);
        }

        foreach (var item in myNavView.FooterMenuItems.Cast<NavigationViewItem>())
        {
            item.Content = localize.GetValue("SettingsPage");
        }
    }

    /// <summary>
    /// ʵ��Frame����ת
    /// </summary>
    /// <param name="view"></param>
    public void NavigateTo(object view)
    {
        var menuItems = myNavView.MenuItemsSource;
        NavigationViewItem ctyp = null;
        foreach (NavigationViewItem cnvi in menuItems)
        {
            var tagName = cnvi.Tag.GetType().FullName;
            var vbtName = view.ToString();
            if (tagName == vbtName)
            {
                ctyp = cnvi;
                break;
            }
        }

        if (ctyp != null && ctyp is NavigationViewItem nvi)
        {
            var info = new SuppressNavigationTransitionInfo();
            myNavView.SelectedItem = nvi;
            NavigationService.Instance.NavigateFromContext(nvi.Tag, info, this);
        }
    }


    /// <summary>
    /// ���õ����˵�ͼ��
    /// </summary>
    /// <param name="item"></param>
    /// <param name="selected"></param>
    private void SetNVIIcon(NavigationViewItem item, bool selected)
    {
        //��������ð󶨺�ת�����ȵȣ���ͼ�����ѡ��������δ���֮��仯������Ҫ�򵥵ö�
        if (item == null)
            return;

        switch (item.Tag)
        {
            case HomePageViewModel:
                item.IconSource = ParseR(selected ? "HomeIconFilled" : "HomeIconFilled");
                break;
            case SettingsPageViewModel:
                item.IconSource = ParseR(selected ? "SettingsIconFilled" : "SettingsIconFilled");
                break;
            case UserPageViewModel:
                item.IconSource = ParseR(selected ? "PeopleIconFilled" : "PeopleIconFilled");
                break;
            case RolePageViewModel:
                item.IconSource = ParseR(selected ? "SpeechSolidBoldIcon" : "SpeechSolidBoldIcon");
                break;
            case PermissionPageViewModel:
                item.IconSource = ParseR(selected ? "DefenderAppIconFilled" : "DefenderAppIconFilled");
                break;
            case DocumentPageViewModel:
                item.IconSource = ParseR(selected ? "FolderIconFilled" : "FolderIconFilled");
                break;
            case MonitorPageViewModel:
                item.IconSource = ParseR(selected ? "PlaybackRateOtherIconFilled" : "PlaybackRateOtherIconFilled");
                break;
            case PatientPageViewModel:
                item.IconSource = ParseR(selected ? "IconFolderLinkIconFilled" : "IconFolderLinkIconFilled");
                break;
            case VideoManagePageViewModel:
                item.IconSource = ParseR(selected ? "VideoIconFilled" : "VideoIconFilled");
                break;
            case ImagePageViewModel:
                item.IconSource = ParseR(selected ? "ImageIconFilled" : "ImageIconFilled");
                break;
        }
    }


    private IconSource ParseR(string s)
    {
        return this.TryFindResource(s, out var value) ? (IconSource)value : null;
    }

    private async void AnimateContentForBackButton(bool show)
    {
        if (!myWindowIcon.IsVisible)
            return;

        if (show)
        {
            var ani = new Animation
            {
                Duration = TimeSpan.FromMilliseconds(250),
                FillMode = FillMode.Forward,
                Children =
                {
                    new KeyFrame
                    {
                        Cue = new Cue(0d),
                        Setters =
                        {
                            new Setter(MarginProperty, new Thickness(12, 4, 12, 4))
                        }
                    },
                    new KeyFrame
                    {
                        Cue = new Cue(1d),
                        KeySpline = new KeySpline(0,0,0,1),
                        Setters =
                        {
                            new Setter(MarginProperty, new Thickness(48,4,12,4))
                        }
                    }
                }
            };

            await ani.RunAsync(myWindowIcon);

            myNavView.IsBackButtonVisible = true;
        }
        else
        {
            myNavView.IsBackButtonVisible = false;

            var ani = new Animation
            {
                Duration = TimeSpan.FromMilliseconds(250),
                FillMode = FillMode.Forward,
                Children =
                {
                    new KeyFrame
                    {
                        Cue = new Cue(0d),
                        Setters =
                        {
                            new Setter(MarginProperty, new Thickness(48, 4, 12, 4))
                        }
                    },
                    new KeyFrame
                    {
                        Cue = new Cue(1d),
                        KeySpline = new KeySpline(0,0,0,1),
                        Setters =
                        {
                            new Setter(MarginProperty, new Thickness(12,4,12,4))
                        }
                    }
                }
            };

            await ani.RunAsync(myWindowIcon);
        }
    }

    private bool _isDesktop;
}
