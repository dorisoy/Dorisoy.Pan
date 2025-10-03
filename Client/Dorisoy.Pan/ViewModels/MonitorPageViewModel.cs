using Utilities = Dorisoy.PanClient.Utils.Utilities;

namespace Dorisoy.PanClient.ViewModels;

[SupportedOSPlatform("windows")]
public class MonitorPageViewModel : MainPageViewModelBase
{
    private readonly IUsersService _usersService;
    private readonly IPatientService _patientService;
    private readonly ISettingsProvider<AppSettings> _settingsProvider;
    private readonly IVirtualFolderService _virtualFolderService;
    private readonly IPhysicalFolderService _physicalFolderService;
    private readonly IDocumentService _documentService;
    private readonly IOnlineUserService _onlineUserService;
    private readonly AutoMapper.IMapper _mapper;

    public ReactiveCommand<DocumentModel, Unit> CommentAdd { get; }
    public ReactiveCommand<DocumentModel, Unit> CommentView { get; }
    public ReactiveCommand<DocumentModel, Unit> ImageDelete { get; }
    public ReactiveCommand<DocumentModel, Unit> ViewCommand { get; }


    /// <summary>
    /// 画中画切换
    /// </summary>
    public ReactiveCommand<Unit, Unit> SwitchPipCommand { get; }

    /// <summary>
    /// 配置了栏选项卡
    /// </summary>
    [Reactive] public int TabSelectedIndex { get; set; }

    /// <summary>
    /// 当前文件夹
    /// </summary>
    [Reactive] public VirtualFolderModel RootFolder { get; set; }

    /// <summary>
    /// 启动
    /// </summary>
    public ReactiveCommand<Unit, Unit> PlayCommand { get; }

    /// <summary>
    /// 截屏
    /// </summary>
    public ReactiveCommand<Unit, Unit> ScreenshotCommand { get; }

    /// <summary>
    /// 录制
    /// </summary>
    public ReactiveCommand<Unit, Unit> RecordCommand { get; }

    /// <summary>
    /// 添加项目
    /// </summary>
    public ReactiveCommand<Unit, Unit> AddPatient { get; }

    /// <summary>
    /// 模拟消息发送
    /// </summary>
    public ReactiveCommand<Unit, Unit> SendMessage { get; }

    /// <summary>
    /// 最大最小化
    /// </summary>
    public ReactiveCommand<Unit, Unit> MaxMinCommand { get; }

    /// <summary>
    /// 画中画全屏
    /// </summary>
    public ReactiveCommand<Unit, Unit> PipFullScreenCommand { get; }

    /// <summary>
    /// 主视频全屏
    /// </summary>
    public ReactiveCommand<Unit, Unit> FullScreenCommand { get; }

    /// <summary>
    /// 主视频监视
    /// </summary>
    [Reactive] public Bitmap MainWebcamViewSource { get; set; }

    /// <summary>
    /// 画中画监视
    /// </summary>
    [Reactive] public WebcamStreamingPlayer PipWebcam { get; set; }
    [Reactive] public Bitmap PipWebcamViewSource { get; set; }


    /// <summary>
    /// 主视频录制
    /// </summary>
    public Recorder _mianRecorder { get; set; }

    [Reactive] public Stretch SelectetPipStretch { get; set; }

    /// <summary>
    /// 画面比例
    /// </summary>
    [Reactive] public Stretch SelectetStretch { get; set; } = Stretch.Uniform;
    [Reactive] public List<Stretch> StretchList { get; set; }

    /// <summary>
    /// 描述缩放内容时可以使用的缩放类型
    /// </summary>
    [Reactive] public StretchDirection SelectetStretchDirection { get; set; }
    [Reactive] public List<StretchDirection> StretchDirectionList { get; set; }

    /// <summary>
    /// 视频信号源控制
    /// </summary>
    [Reactive] public ObservableCollection<CameraDevice> CameraDevices { get; set; }
    [Reactive] public CameraDevice SelectCamera { get; set; }
    [Reactive] public CameraDevice SelectPipCamera { get; set; }

    [Reactive] public bool DisableSound { get; set; }
    [Reactive] public bool PlayOrPause { get; set; }
    [Reactive] public bool RecordOrPause { get; set; }

    [Reactive] public bool FrameLoading { get; set; }
    [Reactive] public bool PipFrameLoading { get; set; }
    [Reactive] public bool IsShowPipView { get; set; }


    [Reactive] public bool DisableMaxMin { get; set; }
    [Reactive] public int ColumnSpan { get; set; } = 1;

    [Reactive] public string RecordVideoText { get; set; } = "录制";
    [Reactive] public string RecordVideoIcon { get; set; } = "VideoFilled";

    [Reactive] public string PlayOrPauseText { get; set; } = "启动";
    [Reactive] public string PlayOrPauseIcon { get; set; } = "PlayFilled";

    private void SetPlayStatus(bool status)
    {
        PlayOrPauseText = status ? "断开" : "启动";
        PlayOrPauseIcon = status ? "PauseFilled" : "PlayFilled";
        PlayOrPause = status;
    }

    private void SetRecordStatus(bool status)
    {
        RecordVideoText = status ? "暂停" : "录制";
        RecordVideoIcon = status ? "IconCircleShapeSolid" : "PauseFilled";
        RecordOrPause = status;
    }

    /// <summary>
    /// 分辨率
    /// </summary>
    [Reactive] public List<Tuple<int, int>> Resolutions { get; set; }
    [Reactive] public Tuple<int, int> SelectetResolution { get; set; }
    private int FrameWidth { get; set; } = 1280;
    private int FrameHeight { get; set; } = 720;

    /// <summary>
    /// 监视器时间统计
    /// </summary>
    private System.Timers.Timer timer;
    private System.DateTime TimeNow = new();
    private TimeSpan TimeCount = new();

    /// <summary>
    /// 分段记录时长(分钟)
    /// </summary>
    [Reactive] public int SegmentDuration { get; set; } = 1;
    [Reactive] public string Ptimer { get; set; } = "00:00:00";
    [Reactive] public string Recordtimer { get; set; } = "00:00:00";

    /// <summary>
    /// 当前项目信息
    /// </summary>
    [Reactive] public PatientModel Patient { get; set; }
    [Reactive] public bool ShowPatient { get; set; }
    [Reactive] public bool SwitchPip { get; set; }


    public ReactiveCommand<OnlinUserUserModel, Unit> SelectedOnlineUserCommand { get; }


    #region ModelStates
    public ReactiveCommand<OnlinUserUserModel, Unit> CallCommand { get; }
    public ReactiveCommand<Unit, Unit> EndCallCommand { get; }

    #endregion


    private Task _receiverTask;
    private CancellationTokenSource _cancellationTokenSource;
    private static UdpClient _udpClient;
    private bool _isStartCaptureRecording;

    /// <summary>
    /// 声明一个简单UDP会话
    /// </summary>
    private UdpSession m_udpSession;
    private IPEndPoint _remoteEndPoint;


    public MonitorPageViewModel(
        IUsersService usersService,
        IPatientService patientService) : base()
    {
        _usersService = usersService;
        _patientService = patientService;
        _settingsProvider = Locator.Current.GetService<ISettingsProvider<AppSettings>>();
        _virtualFolderService = Locator.Current.GetService<IVirtualFolderService>();
        _physicalFolderService = Locator.Current.GetService<IPhysicalFolderService>();
        _documentService = Locator.Current.GetService<IDocumentService>();
        _onlineUserService = Locator.Current.GetService<IOnlineUserService>();
        _mapper = Locator.Current.GetService<AutoMapper.IMapper>();

        CameraDevices = [];

        //创建简单UDP会话实例
        m_udpSession = new UdpSession();

        if (timer == null)
        {
            timer = new() { Interval = 1 };
            timer.Elapsed += timer_Elapsed;
        }

        //画中画
        PipWebcam = new WebcamStreamingPlayer(640, 480);
        PipWebcam.Playing += PipWebcamStreaming_Playing;

        //分辨率
        Resolutions = [
            new(1920, 1080),
            new(1600, 1200),
            new(1360, 768),
            new(1280, 1024),
            new(1280, 960),
            new(1280, 720),
            new(1024, 768),
            new(800, 600),
            new(720, 480),
            new(720, 567),
            new(640, 480),
        ];

        //最大最小化
        MaxMinCommand = ReactiveCommand.Create(() =>
        {
            ShowDisableMaxMin();
        });

        //全屏播放
        FullScreenCommand = ReactiveCommand.Create(() =>
        {
            var win = new FullScreenImageViewer
            {
                WindowState = WindowState.FullScreen,
                WindowStartupLocation = WindowStartupLocation.CenterScreen,
                DataContext = new FullScreenImageViewerViewModel(1, this)
            };
            win.Show();
            win.Activate();
        });

        //画中画全屏播放
        PipFullScreenCommand = ReactiveCommand.Create(() =>
        {
            var win = new FullScreenImageViewer
            {
                WindowState = WindowState.FullScreen,
                WindowStartupLocation = WindowStartupLocation.CenterScreen,
                DataContext = new FullScreenImageViewerViewModel(2, this)
            };
            win.Show();
            win.Activate();
        });

        //选择/添加项目
        AddPatient = ReactiveCommand.CreateFromTask(async () =>
        {
            var user = Globals.CurrentUser;
            var dialog = new ContentDialog()
            {
                FullSizeDesired = true,
                Title = "选择/添加项目",
                PrimaryButtonText = "确定",
                CloseButtonText = "取消"
            };

            dialog.Content = new AddPatientView()
            {
                DataContext = new AddPatientViewModel(dialog)
            };

            var ok = await dialog.ShowAsync();
            if (ok == ContentDialogResult.Primary)
            {
                Patient = Globals.CurrentPatient;
                RefreshPatienterDocuments(user.Id, Patient.Id);
            }
        });

        //启动暂停
        PlayCommand = ReactiveCommand.Create(() =>
        {
            var check = CheckWork();
            if (SelectCamera != null && check)
            {
                PlayOrPause = !PlayOrPause;
                if (PlayOrPause)
                    StartCapture(SelectCamera);
                else
                {
                    StopCapture();
                    SelectCamera = null;
                }
            }
        });

        //画面填充
        StretchList = EnumHelper.GetEnumValues<Stretch>().ToList();
        StretchDirectionList = EnumHelper.GetEnumValues<StretchDirection>().ToList();

        if (StretchList.Any())
            SelectetStretch = StretchList.First();

        if (StretchDirectionList.Any())
            SelectetStretchDirection = StretchDirectionList.First();

        //截屏
        ScreenshotCommand = ReactiveCommand.CreateFromTask(async () =>
        {
            try
            {
                if (_isStartCaptureRecording)
                {
                    Screenshot();
                }
                else
                {
                    var check = CheckWork();
                    var check2 = await CheckPatient();
                    if (check & check2)
                    {
                        Screenshot();
                    }
                }
            }
            catch (Exception)
            {
                MessageBox("抱歉，系统错误！");
            }
        });

        //录制
        RecordCommand = ReactiveCommand.CreateFromTask(async () =>
        {
            var check = CheckWork();
            var check2 = await CheckPatient();
            if (check & check2)
            {
                RecordVideo();
            }
        });

        //添加评论
        CommentAdd = ReactiveCommand.Create<DocumentModel>((item) =>
        {
            if (item == null)
                return;

            AddComment(item.Id);
        });

        //预览评论
        CommentView = ReactiveCommand.Create<DocumentModel>(async (item) =>
        {
            if (item == null)
                return;

            try
            {
                var dialog = new ContentDialog() { Title = "备注信息", CloseButtonText = "关闭" };
                dialog.Content = new CommentsView()
                {
                    DataContext = new CommentsViewModel(dialog, item)
                };
                await dialog.ShowAsync();

            }
            catch (Exception) { }

        });

        //删除图片
        ImageDelete = ReactiveCommand.Create<DocumentModel>((item) =>
        {
            //MessageBox("功能开发中");
        });

        //预览视频图片
        ViewCommand = ReactiveCommand.Create<DocumentModel>((item) =>
        {
            if (item != null && Host is MainView mianView)
            {
                if (item.FileType == FileType.Video)
                    mianView.NavigateTo(typeof(VideoManagePageViewModel));
                else if (item.FileType == FileType.Image)
                    mianView.NavigateTo(typeof(ImagePageViewModel));
            }
        });


        //项目
        this.WhenAnyValue(x => x.Patient)
            .WhereNotNull()
            .Subscribe(x =>
            {
                ShowPatient = !string.IsNullOrEmpty(x.Code);
            });


        //选择切换主视频设备
        this.WhenAnyValue(x => x.SelectCamera)
            .WhereNotNull()
            .ObserveOn(RxApp.MainThreadScheduler)
            .Subscribe(async x =>
            {
                Globals.LastSelectCamera = x;
                FrameLoading = true;

                await Task.Delay(1000);

                if (_mianRecorder != null && _mianRecorder.IsVideoCaptureValid)
                    StopCapture();

                StartCapture(x);
            });


        //启用画中画
        this.WhenAnyValue(x => x.IsShowPipView)
            .Subscribe(async x =>
            {
                if (!x)
                    await PipWebcam?.Stop();
            });

        //选择切换画中画
        this.WhenAnyValue(x => x.SelectPipCamera)
           .WhereNotNull()
           .ObserveOn(RxApp.MainThreadScheduler)
           .Subscribe(async x =>
           {
               if (!IsShowPipView)
               {
                   MessageBox("设备没有启用！");
               }
               else
               {
                   if (PipWebcam != null)
                   {
                       try
                       {
                           PipFrameLoading = true;

                           if (PipWebcam.IsPlaying)
                               await PipWebcam?.Stop();

                           await Task.Delay(1000);

                           //画中画启动
                           PipWebcam?.Start(x.OpenCvId, 1280, 720);

                           await Task.Delay(1000);
                       }
                       catch (Exception ex)
                       {
                           MessageBox(ex.Message);
                       }
                       finally
                       {
                           PipFrameLoading = false;
                       }
                   }
               }
           });


        //切换分辨率
        this.WhenAnyValue(x => x.SelectetResolution)
           .WhereNotNull()
           .ObserveOn(RxApp.MainThreadScheduler)
           .Subscribe(async x =>
           {
               if (_mianRecorder != null && _mianRecorder.IsVideoCaptureValid)
               {
                   FrameWidth = x.Item1;
                   FrameHeight = x.Item2;
                   StopCapture();
                   await Task.Delay(200);
                   StartCapture(SelectCamera);
               }
           });

        //选择用户分享视频
        SelectedOnlineUserCommand = ReactiveCommand.Create<OnlinUserUserModel>( (user) =>
        {
            if (user == null)
                return;

            if (user.Id == CurrentUser.Id)
            {
                MessageBox("拒绝操作，你不能给自己发送视频！");
                return;
            }

            if (_mianRecorder == null || !_mianRecorder.IsVideoCaptureValid)
            {
                MessageBox("拒绝操作，视频没有开启！");
                return;
            }
        });

        //切换画中画
        SwitchPipCommand = ReactiveCommand.Create(() =>
        {
            SwitchPip = !SwitchPip;
        });


        this.WhenActivated((CompositeDisposable disposables) =>
        {
            RxApp.MainThreadScheduler
            .Schedule(LoadData)
            .DisposeWith(disposables);

            //开始UDP数据接收
            StartUDPReceiver();
            //开始UDP服务
            SetupUDPService();
        });

        // 选择远程用户呼叫
        CallCommand = ReactiveCommand.Create<OnlinUserUserModel>(async (user) =>
        {
            if (user == null || user.UserName == CurrentUser.UserName)
                return;

            var vcm = App.MainView.ViewModel.vcm;
            //选择远程用户呼叫
            await Task.Run(() => vcm.BeginCall(user));
        });

        // 结束呼叫
        EndCallCommand = ReactiveCommand.Create(() =>
        {
            var vcm = App.MainView.ViewModel.vcm;
            vcm.State = ModelStates.WaitCall;
        });
    }


    /// <summary>
    /// 侧栏控制
    /// </summary>
    public void ShowDisableMaxMin()
    {
        DisableMaxMin = !DisableMaxMin;
        ColumnSpan = DisableMaxMin ? 2 : 1;
    }


    /// <summary>
    /// 读取摄像头设备信息
    /// </summary>
    private async void LoadData()
    {
        //在线用户
        OnlineUsersItems?.Clear();

        var user = Globals.CurrentUser;
        var patient = Globals.CurrentPatient;

        //获取在线用户
        var users = await _onlineUserService.GetOnlineUsers();
        if (users != null && users.Count > 0)
        {
            users.ForEach(s => { s.RaleName = s.RaleName.Equals(CurrentUser.RaleName) ? "我" : s.RaleName; });
            OnlineUsersItems.Add(users);
        }

        //获取虚拟根目录
        var rootFolder = await _virtualFolderService.GetRootFolder();
        if (rootFolder != null)
            RootFolder = _mapper.Map<VirtualFolderModel>(rootFolder);


        //读取摄像头
        var cameras = CameraDevicesEnumerator.EnumerateDevices();
        if (cameras != null)
            CameraDevices = new ObservableCollection<CameraDevice>(cameras);

        //提示添加项目
        if (patient == null)
            await AddPatient.Execute();
        else
        {
            Patient = patient;
            RefreshPatienterDocuments(user.Id, patient.Id);
        }
    }

    #region 接收UDP视频流

    /// <summary>
    /// UDP接收
    /// </summary>
    public unsafe void StartUDPReceiver()
    {
        //Stopwatch stopWatch = new Stopwatch();
        //stopWatch.Start();
        /*
         * byteBlock <- JPEG 压缩 byte[] bytes = img_trans.ToJpegData(50);
         */
        this.m_udpSession.Received = (client, e) =>
        {
            try
            {
                // 接收数据包
                byte[] data = e.ByteBlock.ToArray();

                Debug.WriteLine($"Received：{data.Length}");

                Dispatcher.UIThread.Invoke(() =>
                {
                    //var thd = Thread.CurrentThread.CurrentCulture;
                    using var ms = new MemoryStream(data);
                    var bitmap = new Avalonia.Media.Imaging.Bitmap(ms);
                    if (bitmap != null)
                    {
                        //自接收发送方的视频
                        MainWebcamViewSource = bitmap;
                    }
                });

                //ffmpeg -f dshow -i video="AFN_Cap video" -vcodec libx264 -acodec copy -preset:v ultrafast -tune:v zerolatency -f flv  rtmp://192.168.0.2/live/livestream
                //ffmpeg -f dshow -i video="Surface Camera Rear" -preset ultrafast -vcodec libx264 -tune zerolatency -b 900k -f mpegts udp://192.168.0.2:9933


            }
            catch (Exception ex)
            {
                Debug.WriteLine($"解码失败：{ex.Message}");
            }

            return Task.CompletedTask;
        };
    }

    /// <summary>
    /// UDP配置
    /// </summary>
    public void SetupUDPService()
    {
        var localIP = Utilities.GetLocalIP();
        //配置服务器
        this.m_udpSession.Setup(new TorchSocketConfig()
                 .SetBindIPHost(new IPHost(IPAddress.Parse(localIP), 9933))
                 .SetRemoteIPHost(new IPHost(IPAddress.Parse(localIP), 9933))
                 .UseBroadcast()
                 .SetNoDelay(true)
                 .SetUdpDataHandlingAdapter(() => new UdpPackageAdapter())
                 .ConfigureContainer(a =>
                 {
                     a.SetSingletonLogger(new LoggerGroup(new EasyLogger((msg) =>
                     {
                         //日志
                         Debug.WriteLine(msg);

                     }), new FileLogger()));
                 }));
    }


    /// <summary>
    /// 开始接收服务
    /// </summary>
    public void StartReceived()
    {
        if (m_udpSession?.ServerState != ServerState.Running)
        {
            m_udpSession?.Start();
        }
    }

    /// <summary>
    /// 停止接收服务
    /// </summary>
    public void StopReceived()
    {
        if (m_udpSession?.ServerState != ServerState.Stopped)
        {
            m_udpSession?.Stop();
        }
    }


    /*

    private Task _waitForNewFramesTask;
    private CancellationTokenSource _wffCts;

    /// <summary>
    /// 接收UDP推送流
    /// </summary>
    public async void StartUDPReceiver()
    {
        if (_waitForNewFramesTask != null && !_waitForNewFramesTask.IsCompleted)
            return;

        _wffCts = new CancellationTokenSource();
        IPEndPoint _remoteUdpIpEndPoint = null;
        if (_udpClient == null)
        {
            _udpClient = new UdpClient(9935);
        }

        _waitForNewFramesTask = Task.Run(() =>
        {
            //定义数据缓存区
            byte[] buffer = new byte[64 * 1024];
            while (!_wffCts.IsCancellationRequested)
            {
                try
                {
                    buffer = _udpClient.Receive(ref _remoteUdpIpEndPoint);

                    ////更新播放源
                    //await Dispatcher.UIThread.InvokeAsync(() =>
                    //{
                    //    try
                    //    {
                    //        //自接收发送方的视频
                    //        using var ms = new MemoryStream(buffer);
                    //        var bitmap = new System.Drawing.Bitmap(ms);
                    //        if (bitmap != null)
                    //            WebcamViewSource = bitmap.ConvertToAvaloniaBitmap(true);
                    //    }
                    //    catch { }
                    //});
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.ToString());
                }
            }

        }, _wffCts.Token);

        if (_waitForNewFramesTask.IsFaulted)
        {
            // 异常退出
            await _waitForNewFramesTask;
        }
    }

    /// <summary>
    /// 停止接收UDP推送流
    /// </summary>
    /// <returns></returns>
    public async Task StopReceiveUDPStreaming()
    {
        if (_wffCts == null || _wffCts.IsCancellationRequested)
            return;

        if (!_waitForNewFramesTask.IsCompleted)
        {
            _wffCts.Cancel();
            m_udpSession?.Dispose();
            await _waitForNewFramesTask;
        }
    }

    */

    #endregion

    #region 主监控

    /// <summary>
    /// 启动主监控摄像头
    /// </summary>
    public void StartCapture(CameraDevice camera)
    {
        try
        {
            if (_mianRecorder == null && !PlayOrPause)
            {
                FrameLoading = true;
                if (camera == null || camera.OpenCvId < 0)
                {
                    MessageBox($"没有可用设备!");
                    return;
                }

                TimeNow = DateTime.Now;
                _mianRecorder = new Recorder(camera, FrameWidth, FrameHeight);
                _mianRecorder.Playing += MianRecorder_Playing;
                _mianRecorder.Timing += MianRecorder_Timing;
                _mianRecorder.Recorded += RecordedUpload;
                //启动视频帧环出线程
                _mianRecorder.StartStreaming();
            }
        }
        catch (Exception ex)
        {
            MessageBox(ex.Message);
        }
    }

    /// <summary>
    /// 停止主监控摄像头
    /// </summary>
    public void StopCapture()
    {
        try
        {
            //工作或者准备中时
            if (_mianRecorder?.IsVideoCaptureValid ?? false)
            {
                timer?.Close();
                //停止输出流
                _mianRecorder.StopStreaming();
                //停止录制
                _mianRecorder.StopRecording();
                _mianRecorder.Playing -= MianRecorder_Playing;
                _mianRecorder.Timing -= MianRecorder_Timing;
                _mianRecorder.Recorded -= RecordedUpload;
                _mianRecorder.Dispose();
                _mianRecorder = null;

                SetPlayStatus(false);
                SetRecordStatus(false);
            }
        }
        catch (Exception ex)
        {
            MessageBox(ex.Message);
        }
    }

    /// <summary>
    /// 主监控摄像头播放时
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void MianRecorder_Playing(object sender, WebcamPlayReadEventArgs e)
    {
        try
        {
            //输出时更新状态
            if (!_mianRecorder.IsLooping)
            {
                timer?.Start();
                FrameLoading = false;
                SetPlayStatus(true);
                _mianRecorder.IsLooping = true;
            }

            var frame = e.Mat;

            if (frame.IsEmpty)
                return;

            //更新UI
            //using var ms = new MemoryStream(bytes);
            //return new Avalonia.Media.Imaging.Bitmap(ms);
            using var bitmap = frame.ToBitmap();
            var abitmap = bitmap.ConvertToAvaloniaImage();
            if (abitmap != null)
                MainWebcamViewSource = abitmap;
        }
        catch (Exception ex)
        {
            Debug.Print(ex.ToString());
        }
    }

    /// <summary>
    /// 录制计时器(监视录制时间)
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void MianRecorder_Timing(object sender, RecorderEventArgs e)
    {
        lock (mlock)
        {
            try
            {
                Recordtimer = e?.FormatString;

                //如果录制分钟数 >= 分段记录时长(分钟) 时，重新开始录制
                if (e.TimeCounter.Minutes >= SegmentDuration && !reSet)
                {
                    //标记当前确保执行至少一次
                    reSet = true;

                    RecordTiming(_mianRecorder);

                    reSet = false;
                }
            }
            catch (Exception ex)
            {
                MessageBox(ex.Message);
            }

        }
    }


    #endregion

    /// <summary>
    ///画中画播放
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void PipWebcamStreaming_Playing(object sender, WebcamPlayReadEventArgs e)
    {
        //Debug.Print($"画中画:{e.Mat.Total}");

        using var bitmap = e.Mat.ToBitmap();
        var abitmap = bitmap.ConvertToAvaloniaImage();
        if (abitmap != null)
        {
            PipWebcamViewSource = abitmap;
        }
    }


    /// <summary>
    /// 停止画中画播放
    /// </summary>
    public async void StopPipWebcam()
    {
        if (PipWebcam.IsPlaying)
            await PipWebcam?.Stop();
    }

    private bool reSet = false;
    private object mlock = new object();

    /// <summary>
    /// 录制视频
    /// </summary>
    public void RecordVideo()
    {
        try
        {
            //设置状态
            SetRecordStatus(true);

            var user = Globals.CurrentUser;
            var basePath = Environment.ExpandEnvironmentVariables(_settingsProvider.Settings.TempFolder);

            var path = System.IO.Path.Combine(basePath, $"{user.Id}");

            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            //文件名
            var name = $"{DateTime.Now:yyyyMMddHHmmss}";
            //视频文件
            var filePath = System.IO.Path.Combine(path, $"{name}.avi");
            //视频缩略图
            var fileThumbnail = System.IO.Path.Combine(path, $"{name}.bmp");

            //保存视频缩略图到本地
            MainWebcamViewSource?.Save(fileThumbnail);

            //录制
            if (_mianRecorder != null)
            {
                if (RecordOrPause)
                    _mianRecorder.StartRecording(new Tuple<string, string>(filePath, fileThumbnail), SegmentDuration);
                else
                    _mianRecorder.StopRecording();
            }
        }
        catch (Exception ex)
        {
            MessageBox(ex.Message);
        }
    }

    /// <summary>
    /// 录制计时器(监视录制时间)
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void PushRecord_Timing(object sender, RecorderEventArgs e)
    {
        lock (mlock)
        {
            try
            {
                Recordtimer = e?.FormatString;

                //如果录制分钟数 >= 分段记录时长(分钟) 时，重新开始录制
                if (e.TimeCounter.Minutes >= SegmentDuration && !reSet)
                {
                    //标记当前确保执行至少一次
                    reSet = true;

                    //RecordTiming();

                    reSet = false;
                }
            }
            catch (Exception ex)
            {
                MessageBox(ex.Message);
            }

        }
    }

    /// <summary>
    /// 录制时间间隔处理
    /// </summary>
    /// <param name="recorder"></param>
    public void RecordTiming(Recorder recorder)
    {
        //停止记录器，调用次方法时会触发Recorded事件
        recorder.StopRecording();

        var user = Globals.CurrentUser;
        var patient = Globals.CurrentPatient;
        var basePath = Environment.ExpandEnvironmentVariables(_settingsProvider.Settings.TempFolder);
        var path = System.IO.Path.Combine(basePath, $"{user.Id}", $"{patient.Id}");

        if (!Directory.Exists(path))
            Directory.CreateDirectory(path);

        //文件名
        var name = $"{DateTime.Now:yyyyMMddHHmmss}";
        //视频文件
        var filePath = System.IO.Path.Combine(path, $"{name}.avi");
        //视频缩略图
        var fileThumbnail = System.IO.Path.Combine(path, $"{name}.bmp");

        //保存视频缩略图到本地
        MainWebcamViewSource?.Save(fileThumbnail);

        //重新开启记录器
        recorder.StartRecording(new Tuple<string, string>(filePath, fileThumbnail), SegmentDuration);
    }

    /// <summary>
    /// 当前录制完毕时上传视频
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    /// <exception cref="NotImplementedException"></exception>
    private async void RecordedUpload(object sender, RecorderEventArgs e)
    {
        try
        {
            var user = Globals.CurrentUser;
            var patient = Globals.CurrentPatient;

            //视频路径
            var fileName = e.Path.Item1;
            var fileThumbnail = e.Path.Item2;

            //上传文件
            var videofile = new FileInfo(fileName);
            var thumbnailFile = new FileInfo(fileThumbnail);

            //视频和缩略
            if (videofile.Exists && thumbnailFile.Exists)
            {
                var vuf = UploadFile.Create(patient, fileName, false);
                var vtuf = UploadFile.Create(patient, fileThumbnail, true);

                if (vuf.VirtualFolderId == Guid.Empty)
                {
                    vuf.VirtualFolderId = RootFolder.Id;
                    vuf.VirtualThumbnailFolderId = RootFolder.Id;
                }

                if (vuf.PhysicalFolderId == Guid.Empty)
                {
                    vuf.PhysicalFolderId = RootFolder.PhysicalFolderId;
                    vuf.PhysicalThumbnailFolderId = RootFolder.PhysicalFolderId;
                }

                if (vtuf.VirtualFolderId == Guid.Empty)
                {
                    vtuf.VirtualFolderId = RootFolder.Id;
                    vtuf.VirtualThumbnailFolderId = RootFolder.Id;
                }

                if (vtuf.PhysicalFolderId == Guid.Empty)
                {
                    vtuf.PhysicalFolderId = RootFolder.PhysicalFolderId;
                    vtuf.PhysicalThumbnailFolderId = RootFolder.PhysicalFolderId;
                }

                //要上传的视频和缩略
                var filePaths = new List<UploadFile>() { vuf, vtuf };

                //上传至项目文件夹
                if (patient.PhysicalFolderId != Guid.Empty)
                    RootFolder.PhysicalFolderId = patient.PhysicalFolderId;

                if (patient.VirtualFolderId != Guid.Empty)
                    RootFolder.Id = patient.VirtualFolderId;

                var dialog = new ContentDialog() { Title = "上传视频", CloseButtonText = "取消" };
                dialog.Content = new AddFilesView()
                {
                    DataContext = new AddFilesViewModel(patient,
                    dialog,
                    RootFolder,
                    filePaths,
                    autoClose: true)
                };
                await dialog.ShowAsync();

                //删除本地临时视频文件
                videofile.Delete();
                thumbnailFile.Delete();

                //刷新视频预览
                TabSelectedIndex = 0;

                RefreshPatienterDocuments(user.Id, patient.Id);
            }
        }
        catch (Exception ex)
        {
            MessageBox(ex.Message);
        }
    }

    /// <summary>
    /// 截屏上传
    /// </summary>
    private async void Screenshot()
    {
        var user = Globals.CurrentUser;
        var patient = Globals.CurrentPatient;

        var basePath = Environment.ExpandEnvironmentVariables(_settingsProvider.Settings.TempFolder);
        var path = System.IO.Path.Combine(basePath, $"{user.Id}", $"{patient.Id}");

        if (patient.Id == Guid.Empty)
            path = System.IO.Path.Combine(basePath, $"{user.Id}");

        if (!Directory.Exists(path))
            Directory.CreateDirectory(path);

        var fileName = System.IO.Path.Combine(path, $"{DateTime.Now:yyyyMMddHHmmss}.bmp");
        var extension = System.IO.Path.GetExtension(fileName);


        //保存图片到本地
        if (MainWebcamViewSource != null)
            MainWebcamViewSource.Save(fileName);

        //上传文件
        var file = new FileInfo(fileName);
        var filePaths = new List<UploadFile>() { new UploadFile { Path = fileName, IsThumbnail = false } };

        //上传至项目文件夹
        if (patient.PhysicalFolderId != Guid.Empty)
            RootFolder.PhysicalFolderId = patient.PhysicalFolderId;

        if (patient.VirtualFolderId != Guid.Empty)
            RootFolder.Id = patient.VirtualFolderId;

        var dialog = new ContentDialog() { Title = "上传文件", CloseButtonText = "取消" };
        dialog.Content = new AddFilesView()
        {
            DataContext = new AddFilesViewModel(patient,
            dialog,
            RootFolder,
            filePaths,
            autoClose: true)
        };

        await dialog.ShowAsync();

        //删除本地
        file.Delete();

        //刷新
        TabSelectedIndex = 0;

        RefreshPatienterDocuments(user.Id, patient.Id);
    }

    /// <summary>
    /// 计时器
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private async void timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
    {
        try
        {
            await Dispatcher.UIThread.InvokeAsync(() =>
             {
                 try
                 {
                     //System.Threading.Tasks.TaskCanceledException:“A task was canceled.”
                     TimeCount = DateTime.Now.Subtract(TimeNow);
                     Ptimer = string.Format("{0:00}:{1:00}:{2:00}", TimeCount.Hours, TimeCount.Minutes, TimeCount.Seconds);
                 }
                 catch (TaskCanceledException) { }
             });
        }
        catch (TaskCanceledException) { }
    }

    #region  刷新项目


    /// <summary>
    /// 刷新项目图片视频
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="patientId"></param>
    private async void RefreshPatienterDocuments(Guid userId, Guid patientId)
    {
        var setting = _settingsProvider.Settings;

        await Dispatcher.UIThread.InvokeAsync(async () =>
        {
            try
            {
                Patient.Images.Clear();
                Patient.Videos.Clear();

                var _pathHelper = new PathHelper();
                //获取位图文档
                var docs = await _documentService.GetPatienterDocuments(userId, patientId);
                if (docs != null && docs.Any())
                {
                    foreach (var doc in docs)
                    {
                        if (doc.Extension.Contains(".bmp") && doc.IsAttachment == false)
                        {
                            //var path = Path.Combine(_pathHelper.ContentRootPath, _pathHelper.DocumentPath, doc?.Path);
                            //if (File.Exists(path))
                            //{

                            //    //读取
                            //    var filebyte = AesOperation.ReadFile(path);
                            //    //转换成位图
                            //    using var stream = new MemoryStream(filebyte);
                            //    doc.Cover = Avalonia.Media.Imaging.Bitmap.DecodeToWidth(stream, 300, BitmapInterpolationMode.HighQuality);
                            //    //更新待预览区
                            //    doc.FileType = FileType.Image;
                            //    Patient.Images.Add(doc);
                            //}

                            doc.PathURL = setting.GetHost() + $"/document/{doc.Id}/download";
                            //更新待预览区
                            doc.FileType = FileType.Image;
                            Patient.Images.Add(doc);

                        }
                        else if (doc.Extension.Contains(".avi"))
                        {
                            //获取视频缩略图
                            var name = doc.Name.Replace(".avi", ".bmp");
                            var tdoc = docs.Where(s => s.Name == name).FirstOrDefault();
                            if (tdoc != null)
                            {
                                doc.PathURL = setting.GetHost() + $"/document/{tdoc.Id}/download";
                                //更新待预览区
                                doc.FileType = FileType.Video;
                                Patient.Videos.Add(tdoc);
                            }
                        }
                    }

                    //添加评论
                    //var firstDoc = Patient.Images.FirstOrDefault();
                    //if (firstDoc != null && firstDoc.Comments == 0)
                    //{
                    //    AddComment(firstDoc.Id);
                    //}
                }
            }
            catch (Exception) { }
        });
    }

    /// <summary>
    /// 添加备注
    /// </summary>
    /// <param name="documentId"></param>
    private async void AddComment(Guid documentId)
    {
        await Dispatcher.UIThread.InvokeAsync(async () =>
        {
            try
            {
                var dialog = new ContentDialog()
                {
                    Title = "添加备注",
                    PrimaryButtonText = "保存",
                    CloseButtonText = "取消"
                };

                dialog.Content = new AddCommentView()
                {
                    DataContext = new AddCommentViewModel(dialog, documentId)
                };
                await dialog.ShowAsync();

            }
            catch (Exception) { }
        });
    }

    /// <summary>
    /// 监视设备是否启动
    /// </summary>
    private bool CheckWork()
    {
        if (_mianRecorder == null || (!_mianRecorder?.IsVideoCaptureValid ?? false))
        {
            MessageBox("监视信视频尚未工作！");
            TabSelectedIndex = 1;
            return false;
        }
        else
        {
            return true;
        }
    }

    /// <summary>
    ///  检查项目
    /// </summary>
    private async Task<bool> CheckPatient()
    {
        if (Globals.CurrentPatient.Id == Guid.Empty)
        {
            //提示添加项目
            await AddPatient.Execute();
            return false;
        }
        else
        {
            return true;
        }
    }

    #endregion 

    protected override void DisposeUnmanaged()
    {
        if (PipWebcam != null)
            PipWebcam.Playing -= PipWebcamStreaming_Playing;

        if (_mianRecorder != null)
        {
            _mianRecorder.Playing -= MianRecorder_Playing;
            _mianRecorder.Timing -= MianRecorder_Timing;
            _mianRecorder.Recorded -= RecordedUpload;
            _mianRecorder.StopRecording();
            _mianRecorder.Dispose();
        }
    }
}
