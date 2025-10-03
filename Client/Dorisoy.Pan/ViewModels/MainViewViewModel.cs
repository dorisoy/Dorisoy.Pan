using NetworkLibrary;
using Disposable = System.Reactive.Disposables.Disposable;
using Path = System.IO.Path;

namespace Dorisoy.PanClient.ViewModels;

/// <summary>
/// 主视图页模型
/// </summary>
public class MainViewViewModel : ViewModelBase, IRoutableViewModel
{
     public event PropertyChangedEventHandler PropertyChanged2;
    private void OnPropertyChanged(string PropertyName)
    {
        PropertyChanged2?.Invoke(this, new PropertyChangedEventArgs(PropertyName));
    }

    public NavigationFactory NavigationFactory { get; }

    private readonly ILocalizationManager _localizationManager;

    public string UrlPathSegment => nameof(MainViewViewModel);
    public RoutingState Router { get; } = new RoutingState();
    public UserAuthDto UserInformation { get; }
    public List<MainPageViewModelBase> Pages { get; set; } = new();

    [Reactive] 
    public ObservableCollection<NavigationViewItemBase> MenuItems { get; set; } = new();

    [Reactive] 
    public ObservableCollection<NavigationViewItemBase> FooterItems { get; set; } = new();

    [Reactive] 
    public ReactiveCommand<Unit, Unit> Logout { get; set; }

    [Reactive] 
    public ReactiveCommand<string, Unit> SelectLanguage { get; set; }

    #region ModelStates

    [Reactive] public ReactiveCommand<Unit, Unit> EndCallCommand { get; set; }
    [Reactive] public ReactiveCommand<Unit, Unit> DeclineCallCommand { get; set; }
    [Reactive] public ReactiveCommand<Unit, Unit> AcceptCallCommand { get; set; }
    [Reactive] public ReactiveCommand<Unit, Unit> VideoSharingCommand { get; set; }
    [Reactive] public ReactiveCommand<Unit, Unit> AudioSharingCommand { get; set; }
    [Reactive] public ReactiveCommand<Unit, Unit> ScreenSharingCommand { get; set; }
    [Reactive] public ReactiveCommand<Unit, Unit> MessageSendCommand { get; set; }
    [Reactive] public ReactiveCommand<Unit, Unit> MessageClearCommand { get; set; }
    [Reactive] public VoiceChatModel vcm { get; set; }
    [Reactive] public string RemoteIP { get; set; }

    [Reactive] public bool WaitCall { get; set; } = true;
    [Reactive] public bool OutgoingCall { get; set; }
    [Reactive] public bool IncomingCall { get; set; }
    [Reactive] public bool Talk { get; set; }

    [Reactive] public bool Connected { get; set; }
    [Reactive] public bool Disconnected { get; set; }
    [Reactive] public string CallTimeString { get; set; } = "00:00:00";

    [Reactive] public Bitmap RemoteFrame { get; set; }
    [Reactive] public Bitmap LocalFrame { get; set; }
    public WriteableBitmap ScreenFrame { get; set; }
    public WriteableBitmap RemoteScreenFrame { get; set; }

    [Reactive] public bool IsVideoSending { get; set; }
    [Reactive] public bool IsAudioSending { get; set; }
    [Reactive] public bool IsScreenSending { get; set; }
    [Reactive] public bool DisableMaxMin { get; set; } = true;


    #endregion

    /// <summary>
    /// 最大最小化
    /// </summary>
    public ReactiveCommand<Unit, Unit> MaxMinCommand { get; }

    #region Chat

    /// <summary>
    /// 清除历史聊天
    /// </summary>
    public ReactiveCommand<Unit, Unit> ClearChatHistoryCommand { get; }

    public ReactiveCommand<string, Unit> AddEmojiDocumentCommand { get; }

    /// <summary>
    /// 当前对话用户
    /// </summary>
    [Reactive] public OnlinUserUserModel RemoteUser { get; set; }

    /// <summary>
    /// 用于滚动到结束聊天窗口
    /// </summary>
    public Action SrollToEndChatWindow;

    /// <summary>
    /// 定义Chat序列化器
    /// </summary>
    private ChatSerializer chatSerializer;

    /// <summary>
    /// 对话数据
    /// </summary>
    [Reactive] public ObservableCollection<ChatDataModel> ChatData { get; set; } = new();
    /// <summary>
    /// 视频信号源控制
    /// </summary>
    [Reactive] public ObservableCollection<CameraDevice> CameraDevices { get; set; } = new();
    [Reactive] public CameraDevice SelectetCamera { get; set; }

    [Reactive] public int CaretIndex { get; set; }

    [Reactive] public ChatDataModel ChatDataSelectedItem { get; set; }

    [Reactive] public string ChatInputText { get; set; }

    [Reactive] public double UploadPercent { get; set; }

    private CancellationTokenSource Cts = new CancellationTokenSource();

    public event Action<string> InsertTextAtCaret;


    #endregion

    public MainViewViewModel(IScreen screen, UserAuthDto userInformation) : base()
    {
        //本地化管理器
        _localizationManager = LocalizationManagerExtensions.Default!;
   
        NavigationFactory = new NavigationFactory(this);
        HostScreen = screen;
        UserInformation = userInformation;

        #region Chat

        //序列化器
        chatSerializer = new ChatSerializer(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location));

        #endregion

        //表示语音聊天模型
        vcm = new VoiceChatModel(this);
        vcm.OnStateChange += (e, arg) =>
        {
            try
            {
                var s = arg.States;
                var that = e as VoiceChatModel;

                Dispatcher.UIThread.Invoke(() =>
                {
                    try
                    {
                        RemoteIP = that.RemoteIP?.ToString();

                        if (App.MainView != null)
                        {
                            App.MainView.myTitleBarHost.IsVisible = s == ModelStates.WaitCall;
                            App.MainView.myNavView.IsVisible = s == ModelStates.WaitCall;
                            //
                            App.MainView.myOutgoingCall.IsVisible = s == ModelStates.OutgoingCall;
                            App.MainView.myIncomingCall.IsVisible = s == ModelStates.IncomingCall;
                            App.MainView.myTalk.IsVisible = s == ModelStates.Talk;
                        }
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine(ex.Message);
                    }
                });
            }
            catch (Exception) { }
        };
        vcm.OnCallTimerChange += (e, arg) =>
        {
            CallTimeString = arg.Value;
        };
       
        InitializeNavigationPages();
     
        //最大最小化
        MaxMinCommand = ReactiveCommand.Create(ChangeMaxMin);

        //注销
        Logout = ReactiveCommand.CreateFromTask(async () =>
        {
            try
            {
                //Microsoft.AspNetCore.SignalR.HubException:“Failed to invoke 'LeaveRoom'
                // due to an error on the server. HubException: Method does not exist.”

                //if (_connection.State == HubConnectionState.Connected)
                // await _connection.InvokeAsync("LeaveRoom", Globals.CurrentUser.Id);

                DisposeUnmanaged();

                Globals.CurrentUser = null;
                await HostScreen.Router.Navigate.Execute(new LoginViewModel(HostScreen));
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            };
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

        //拒绝呼叫
        DeclineCallCommand = ReactiveCommand.Create(vcm.DeclineCall);

        //接受呼叫
        AcceptCallCommand = ReactiveCommand.Create(() => 
        {
            var rup = vcm.RemoteIP.ToString();
            var remoteUser = OnlineUsersItems.Where(s => s.IP == rup).FirstOrDefault();
            if (remoteUser != null)
            {
                this.RemoteUser = remoteUser;
                vcm.AcceptCall();
            }
        });

        //挂断通话
        EndCallCommand = ReactiveCommand.Create(vcm.EndCall);

        //开启视频
        VideoSharingCommand = ReactiveCommand.Create(() => 
        {
            //先中断屏幕
            if (vcm.screenSharing.IsSending)
                vcm.screenSharing.SwitchSendingState();

            vcm.video.SwitchSendingState();
        });

        //开启语音
        AudioSharingCommand = ReactiveCommand.Create(vcm.audio.SwitchSendingState);

        //屏幕共享
        ScreenSharingCommand = ReactiveCommand.Create(() =>
        {
            DisableMaxMin = false;

            //先中断视频
            if (vcm.video.IsSending)
                vcm.video.SwitchSendingState();

            vcm.screenSharing.SwitchSendingState();
        });

        //清理
        MessageClearCommand = ReactiveCommand.Create(() =>
        {
            ChatInputText = "";
        });

        //发送消息
        MessageSendCommand = ReactiveCommand.Create(() =>
        {
            WriteMessage(ChatInputText);
        });

        //发送表情
        AddEmojiDocumentCommand = ReactiveCommand.Create<string>((s) => 
        {
            InsertTextAtCaret.Invoke(s);
        });

        //清理聊天历史
        ClearChatHistoryCommand = ReactiveCommand.Create(() => ClearChatHistory());

        //接收消息
        _connection.On<string, UserInfoToken>("OnReceiveMessage", (msg, user) =>
        {
            //写入本地消息
            WriteRemoteChat(user.Name, msg);
            if (!DisableMaxMin)
                DisableMaxMin = true;
        });

        this.WhenAnyValue(s => s.vcm.Connected)
            .Subscribe(s =>
            {
                Connected = s;
                Disconnected = !s;
            });

        //选择切换主视频设备
        this.WhenAnyValue(x => x.SelectetCamera)
            .WhereNotNull()
            .ObserveOn(RxApp.MainThreadScheduler)
            .Subscribe( async x =>
            {
                try
                {
                    vcm.video.OpenCvId = x.OpenCvId;
                    await vcm.video.StopOpenCvCapture();
                    await vcm.video.StartOpenCvCapture();
                }
                catch (Exception ex) { }
            });

        this.WhenActivated((CompositeDisposable disposables) => 
        {
            ConnectAsync();


            InitializeEvents();

            //加载数据
            var cts = new CancellationTokenSource();

            Dispatcher.UIThread.Invoke(() =>
            {
                SrollToEndChatWindow?.Invoke();
            });

            Observable.Interval(TimeSpan.FromSeconds(5))
              .ObserveOn(AvaloniaScheduler.Instance)
              .Subscribe(_ =>
              {
                  _connection?.SendAsync("GetOnlineUsers", cts.Token);

              }).DisposeWith(disposables);

            LoadDataCommand.Execute(cts.Token)
                .Subscribe()
                .DisposeWith(disposables);

            Disposable.Create(() => cts.Cancel()).DisposeWith(disposables);
        });

        LoadDataCommand.ThrownExceptions.Subscribe(ex =>
        {
            MessageBox(ex.Message);
        });
    }

    /// <summary>
    /// 载入数据
    /// </summary>
    /// <returns></returns>
    protected override  void LoadDataAsync(CancellationToken token)
    {
        Task.Run(() =>
        {    //读取摄像头
            var cameras = CameraDevicesEnumerator.EnumerateDevices();
            Dispatcher.UIThread.Invoke(() =>
            {
                LoadMoreMessages();
                if (cameras != null)
                    CameraDevices = new ObservableCollection<CameraDevice>(cameras);
            });
        }, token);
    }

    /// <summary>
    /// 写入消息
    /// </summary>
    /// <param name="msg"></param>
    public void WriteMessage(string msg)
    {
        Dispatcher.UIThread.Invoke(() =>
        {
            var chatData = WriteLocalChat(msg, DateTime.Now);
            chatSerializer.SerializeLocalEntry(msg, DateTime.Now, chatData.Sender);
            //处理发送
            HandleUserChatSend(msg);
            ChatData.Add(chatData);
            ChatInputText = "";
            SrollToEndChatWindow?.Invoke();
        });
    }

    /// <summary>
    /// 侧栏布局切换
    /// </summary>
    public void ChangeMaxMin()
    {
        DisableMaxMin = !DisableMaxMin;
    }

    public void AddEmojiDocument(string arg)
    {
        InsertTextAtCaret.Invoke(arg);
    }

    /// <summary>
    /// 上传文件
    /// </summary>
    public async void UploadDocument()
    {
        try
        {
            var _storageProvider = Locator.Current.GetService<IStorageProvider>();
            if (_storageProvider == null)
            {
                MessageBox("没有找到文件提供程序！");
                return;
            }

            if (_storageProvider != null)
            {
                var option = new FilePickerOpenOptions()
                {
                    Title = "上传文件",
                    AllowMultiple = true,
                    FileTypeFilter = [new FilePickerFileType("All files (.*)")
                    {
                        Patterns = ["mp4", "jepg", "*"]
                    }]
                };

                var spder = await _storageProvider.OpenFilePickerAsync(option);
                var filePaths = new List<UploadFile>();
                if (spder != null && spder.Any())
                {
                    foreach (var sd in spder)
                    {
                        filePaths.Add(new UploadFile
                        {
                            Path = sd.Path.ToString().Replace("file:///", ""),
                            IsThumbnail = false
                        });
                    }
                }

                if (filePaths != null && filePaths.Count > 0)
                {
                    var progressReporter = new Progress<double>(ReportProgress);
                    await vcm.fileShare.StartSend(this, filePaths, progressReporter, Cts.Token);
                }
            }
        }
        catch (Exception ex)
        {
            MessageBox(ex.Message);
        }
    }

    /// <summary>
    /// 报告上传进度
    /// </summary>
    /// <param name="val"></param>
    private void ReportProgress(double val)
    {
        UploadPercent = val;
    }

    /// <summary>
    /// 清理聊天历史
    /// </summary>
    private void ClearChatHistory()
    {
        chatSerializer.ClearAllHistory();
        ChatData.Clear();
    }

    /// <summary>
    /// 载入聊天消息
    /// </summary>
    /// <returns></returns>
    public bool LoadMoreMessages()
    {

        bool succes = chatSerializer.LoadFromEnd(20, out var messages);
        if (!succes)
            return succes;

        foreach (var item in messages)
        {
            switch (item.MessageType)
            {
                case ChatSerializationData.MsgType.Local:
                    var chat = WriteLocalChat(item.Message, item.TimeStamp);
                    chat.Sender = item.Sender;
                    ChatData.Insert(0, chat);
                    //SrollToEndChatWindow?.Invoke();
                    break;
                case ChatSerializationData.MsgType.Remote:
                    var msg = CreateRemoteChatItem(item.Sender, item.Message);
                    msg.Time = item.TimeStamp.ToShortTimeString();
                    msg.Sender = item.Sender;
                    ChatData.Insert(0, msg);
                    break;
                case ChatSerializationData.MsgType.Info:
                    break;
            }
        }
        return succes;
    }

    /// <summary>
    /// 处理消息发送
    /// </summary>
    /// <param name="chatInputText"></param>
    private async void HandleUserChatSend(string chatInputText)
    {
        try
        {
            string textToSend = chatInputText;
            if (RemoteUser != null)
            {
                var uid = RemoteUser.Id;
                await _connection.SendAsync("SendMessage", CurrentUser.Id, uid, textToSend);
            }
        }
        catch (Exception ex)
        { }
    }


    /// <summary>
    /// 写入本地Chat
    /// </summary>
    /// <param name="message"></param>
    /// <param name="timeStamp"></param>
    /// <returns></returns>
    private ChatDataModel WriteLocalChat(string message, DateTime timeStamp)
    {
        var chatData = new ChatDataModel();
        chatData.CreateLocalChatEntry(message, timeStamp);
        if (ChatData.Count > 0 && ChatData.Last().Allignment == "Right"
        && (ChatData.Last().Sender == "你" || ChatData.Last().Sender == null))
        {
            chatData.Sender = null;
        }
        return chatData;
    }

    private void HandleText(MessageEnvelope message)
    {
        foreach (var item in message.KeyValuePairs)
        {
            WriteRemoteChat(item.Key, item.Value);
        }
    }

    /// <summary>
    /// 写入远程Chat
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="message"></param>
    public void WriteRemoteChat(string sender, string message)
    {
        Dispatcher.UIThread.Invoke(() => 
        {
            var chatData = CreateRemoteChatItem(sender, message);
            chatSerializer.SerializeRemoteEntry(chatData.Sender, message, DateTime.Now);
            ChatData.Add(chatData);
            //滚动到
            SrollToEndChatWindow?.Invoke();
        }, DispatcherPriority.Background);

    }

    /// <summary>
    /// 创建远程Chat项
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="message"></param>
    /// <returns></returns>
    private ChatDataModel CreateRemoteChatItem(string sender, string message)
    {
        var chatData = new ChatDataModel();
        chatData.CreateRemoteChatEntry(sender, message);
        if (ChatData.Count > 0 && ChatData.Last().Allignment == "Left" &&
        (ChatData.Last().Sender == sender || ChatData.Last().Sender == null))
        {
            chatData.Sender = null;
        }
        return chatData;
    }


    /// <summary>
    /// 初始化事件
    /// </summary>
    private void InitializeEvents()
    {
        vcm.callTimer.PropertyChanged += (sender, e) =>
        {
            var ct = sender as CallTimer;
            OnPropertyChanged("CallTimeString");
            CallTimeString = ct.CallTime.ToString("c");
        };

        vcm.video.PropertyChanged += (sender, e) =>
        {
            OnPropertyChanged("RemoteFrame");
            OnPropertyChanged("LocalFrame");
         
            RemoteFrame = vcm.video.RemoteFrame;
            LocalFrame = vcm.video.LocalFrame;

            OnPropertyChanged("IsSending");
            IsVideoSending = vcm.video.IsSending;
        };

        vcm.audio.PropertyChanged += (sender, e) =>
        {
            OnPropertyChanged("IsAudioSending");
            IsAudioSending = vcm.audio.IsSending;
        };

        vcm.screenSharing.PropertyChanged += (sender, e) =>
        {
            OnPropertyChanged("IsScreenSending");
            IsScreenSending = vcm.screenSharing.IsSending;
        };

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
        var canVideosView = CurrentUser.Authorize(Permissions.Videos.View);
        var canDocumentsView = CurrentUser.Authorize(Permissions.Documents.View);
        var canSettingsView = CurrentUser.Authorize(Permissions.Settings.View);

        var home = GetService("HomePage") as HomePageViewModel;
        var user = GetService("UserPage") as UserPageViewModel;
        var role = GetService("RolePage") as RolePageViewModel;
        var permission = GetService("PermissionPage") as PermissionPageViewModel;
        var document = GetService("DocumentPage") as DocumentPageViewModel;
        var monitor = GetService("MonitorPage") as MonitorPageViewModel;
        var video = GetService("VideoManagePage") as VideoManagePageViewModel;
        var image = GetService("ImagePage") as ImagePageViewModel;
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

        //检查
        monitor.NavHeader = "MonitorPage";
        monitor.IconKey = "PlaybackRateOtherIconFilled";

        //存储
        document.NavHeader = "DocumentPage";
        document.IconKey = "FolderIconFilled";

        //录制
        video.NavHeader = "VideoManagePage";
        video.IconKey = "VideoIconFilled";

        //图像
        image.NavHeader = "ImagePage";
        image.IconKey = "ImageIconFilled";

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

        //monitor
        if (canVideosView)
            mainPages.Add(monitor);

        //document
        if (canDocumentsView)
            mainPages.Add(document);

        //video
        if (canVideosView)
            mainPages.Add(video);

        //image
        if (canVideosView)
            mainPages.Add(image);

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

            nvi.Classes.Add("DorisoyAppNav");

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

    protected override async void DisposeUnmanaged()
    {
        //释放HUB
        await DisconnectAsync();
        vcm?.Closing();
        vcm?.Dispose();
    }
}


public class NavigationFactory(MainViewViewModel owner) : INavigationPageFactory
{
    public MainViewViewModel Owner { get; } = owner;

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
        else if (target is MonitorPageViewModel)
            return new MonitorPage { DataContext = target };
        else if (target is VideoManagePageViewModel)
            return new VideoManagePage { DataContext = target };
        else if (target is ImagePageViewModel)
            return new ImagePage { DataContext = target };
        else
            return null;
    }
}
