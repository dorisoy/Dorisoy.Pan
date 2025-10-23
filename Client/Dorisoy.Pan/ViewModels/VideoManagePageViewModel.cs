namespace Dorisoy.Pan.ViewModels;

/// <summary>
/// 视频预览
/// </summary>
//[View(typeof(VideoManagePage))]
public class VideoManagePageViewModel : MainPageViewModelBase
{
    private readonly IUsersService _usersService;
    private readonly IPatientService _patientService;
    private readonly ISettingsProvider<AppSettings> _settingsProvider;
    private readonly IDocumentService _documentService;

    [Reactive] public virtual ObservableCollection<DocumentModel> Videos { get; set; } = new();

    /// <summary>
    /// 定义LibVLCSharp媒体播放器
    /// </summary>
    [Reactive] public MediaPlayer MediaPlayer { get; set; }

    /// <summary>
    /// 媒体地址
    /// </summary>
    [Reactive] public string MediaUrl { get; set; }


    private System.Timers.Timer timer;
    private System.DateTime TimeNow = new DateTime();
    private TimeSpan TimeCount = new TimeSpan();
    [Reactive] public string Ptimer { get; set; } = "00:00:00";
    public bool IsMuted
    {
        get => MediaPlayer.Mute;
        set => MediaPlayer.Mute = value;
    }

    /// <summary>
    /// 播放时间进度
    /// </summary>
    public TimeSpan CurrentTime => TimeSpan.FromMilliseconds(MediaPlayer.Time > -1 ? MediaPlayer.Time : 0);

    /// <summary>
    /// 
    /// </summary>
    public TimeSpan Duration => TimeSpan.FromMilliseconds(MediaPlayer.Length > -1 ? MediaPlayer.Length : 0);


    /// <summary>
    /// 当前播放状态
    /// </summary>
    public VLCState State => MediaPlayer.State;

    /// <summary>
    /// 媒体信息
    /// </summary>
    public string MediaInfo
    {
        get
        {
            var m = MediaPlayer.Media;

            if (m == null)
                return "";

            var vt = m.Tracks.FirstOrDefault(t => t.TrackType == TrackType.Video);
            var at = m.Tracks.FirstOrDefault(t => t.TrackType == TrackType.Audio);
            var videoCodec = m.CodecDescription(TrackType.Video, vt.Codec);
            var audioCodec = m.CodecDescription(TrackType.Audio, at.Codec);

            return $"{vt.Data.Video.Width}x{vt.Data.Video.Height} {vt.Description}video: {videoCodec} audio: {audioCodec}";
        }
    }

    public string Information => $"FPS:{MediaPlayer.Fps} {MediaInfo}";


    /// <summary>
    /// 播放位置点
    /// </summary>
    public float Position
    {
        get => MediaPlayer.Position * 100.0f;
        set
        {
            if (MediaPlayer.Position != value / 100.0f)
            {
                MediaPlayer.Position = value / 100.0f;
            }
        }
    }


    /// <summary>
    /// 声音
    /// </summary>
    public int Volume
    {
        get => MediaPlayer.Volume;
        set => MediaPlayer.Volume = value;
    }

    /// <summary>
    /// 播放
    /// </summary>
    public ICommand PlayCommand { get; }
    /// <summary>
    /// 停止
    /// </summary>
    //public ICommand StopCommand { get; }
    /// <summary>
    /// 暂停
    /// </summary>
    public ICommand PauseCommand { get; }
    /// <summary>
    /// 快进
    /// </summary>
    public ICommand ForwardCommand { get; }
    /// <summary>
    /// 快退
    /// </summary>
    public ICommand BackwardCommand { get; }
    /// <summary>
    /// 下一帧
    /// </summary>
    public ICommand NextFrameCommand { get; }
    /// <summary>
    /// 打开本地文件
    /// </summary>
    public ICommand OpenCommand { get; }
    /// <summary>
    /// 声音控制
    /// </summary>
    public ICommand SoundCommand { get; }

    public IEnumerable Played => _played;

    /// <summary>
    /// 播放记录
    /// </summary>
    private static void LoadPlayed()
    {
        try
        {
            if (File.Exists("playhistory.txt"))
                _played.AddRange(File.ReadAllLines("playhistory.txt"));
        }
        catch { }
    }

    private static void SavePlayed()
    {
        File.WriteAllLines("playhistory.txt", _played.ToArray());
    }

    private readonly LibVLC _libVLC;
    private CompositeDisposable _subscriptions;
    private readonly static AvaloniaList<string> _played = new();

    [Reactive] public bool DisableSound { get; set; } = false;
    [Reactive] public string PlayCommandIcon { get; set; } = "PlayFilled";
    [Reactive] public bool PlayStatus { get; set; } = false;
    [Reactive] public string PlayCommandText { get; set; } = "播放";
    [Reactive] public string PauseCommandIcon { get; set; } = "PauseFilled";
    [Reactive] public bool PauseStatus { get; set; } = false;
    [Reactive] public string PauseCommandText { get; set; } = "暂停";

    /// <summary>
    /// 添加项目
    /// </summary>
    public ReactiveCommand<Unit, Unit> AddPatient { get; }

    public VideoManagePageViewModel(
        IUsersService usersService,
        IPatientService patientService) : base()
    {
        _usersService = usersService;
        _patientService = patientService;
        _settingsProvider = Locator.Current.GetService<ISettingsProvider<AppSettings>>();
        _documentService = Locator.Current.GetService<IDocumentService>();

        if (timer == null)
        {
            timer = new() { Interval = 1 };
            timer.Elapsed += timer_Elapsed;
        }

        //初始化MediaPlayer
        _libVLC = new LibVLC();
        MediaPlayer = new MediaPlayer(_libVLC);

        //默认地址
        MediaUrl = "";

        bool operationActive = false;
        var refresh = new Subject<Unit>();

        //某些操作处于活动状态时禁用事件，因为有时会导致死锁
        IObservable<Unit> Wrap(IObservable<Unit> obs) => obs
            .Where(_ => !operationActive)
            .Merge(refresh)
            .ObserveOn(RxApp.MainThreadScheduler);

        IObservable<Unit> VLCEvent(string name) => Observable
            .FromEventPattern(MediaPlayer, name)
            .Select(_ => Unit.Default);

        void Op(Action action)
        {
            operationActive = true;
            action();
            operationActive = false;
            refresh.OnNext(Unit.Default);
        };

        //进度更改
        var positionChanged = VLCEvent(nameof(MediaPlayer.PositionChanged));
        //播放器播放中
        var playingChanged = VLCEvent(nameof(MediaPlayer.Playing));
        //停止播放
        var stoppedChanged = VLCEvent(nameof(MediaPlayer.Stopped));
        //播放时间已更改
        var timeChanged = VLCEvent(nameof(MediaPlayer.TimeChanged));
        //播放的长度已更改
        var lengthChanged = VLCEvent(nameof(MediaPlayer.LengthChanged));
        //播放器已静音
        var muteChanged = VLCEvent(nameof(MediaPlayer.Muted)).Merge(VLCEvent(nameof(MediaPlayer.Unmuted)));
        //播放器播放到最后
        var endReachedChanged = VLCEvent(nameof(MediaPlayer.EndReached));
        //播放暂停
        var pausedChanged = VLCEvent(nameof(MediaPlayer.Paused));
        //声音更改
        var volumeChanged = VLCEvent(nameof(MediaPlayer.VolumeChanged));
        //
        var stateChanged = Observable.Merge(playingChanged, endReachedChanged);
        var hasMediaObservable = this.WhenAnyValue(v => v.MediaUrl, v => !string.IsNullOrEmpty(v));

        //全部状态
        var fullState = Observable.Merge(
                            stateChanged,
                            VLCEvent(nameof(MediaPlayer.NothingSpecial)),
                            VLCEvent(nameof(MediaPlayer.Buffering)),
                            VLCEvent(nameof(MediaPlayer.EncounteredError))
                            );

        _subscriptions = new CompositeDisposable
        {
            Wrap(positionChanged).DistinctUntilChanged(_ => Position).Subscribe(_ => this.RaisePropertyChanged(nameof(Position))),
            Wrap(timeChanged).DistinctUntilChanged(_ => CurrentTime).Subscribe(_ => this.RaisePropertyChanged(nameof(CurrentTime))),
            Wrap(lengthChanged).DistinctUntilChanged(_ => Duration).Subscribe(_ => this.RaisePropertyChanged(nameof(Duration))),
            Wrap(muteChanged).DistinctUntilChanged(_ => IsMuted).Subscribe(_ => this.RaisePropertyChanged(nameof(IsMuted))),
            Wrap(fullState).DistinctUntilChanged(_ => State).Subscribe(_ => this.RaisePropertyChanged(nameof(State))),
            Wrap(volumeChanged).DistinctUntilChanged(_ => Volume).Subscribe(_ => this.RaisePropertyChanged(nameof(Volume))),
            Wrap(fullState).DistinctUntilChanged(_ => Information).Subscribe(_ => this.RaisePropertyChanged(nameof(Information)))
        };

        bool active() => _subscriptions != null && (MediaPlayer.IsPlaying || MediaPlayer.CanPause);
        stateChanged = Wrap(stateChanged);

        //播放
        PlayCommand = ReactiveCommand.Create(() => Op(() =>
        {
            if (string.IsNullOrEmpty(MediaUrl))
            {
                MessageBox("无效视频信息！");
                return;
            }

            PlayStatus = !PlayStatus;
            if (PlayStatus)
            {
                string absolute = new Uri(MediaUrl).AbsoluteUri;
                bool isfile = absolute.StartsWith("file://");
                MediaPlayer.Media = new Media(_libVLC, MediaUrl, FromType.FromPath);
                MediaPlayer.Play();
                PlayCommandIcon = "StopFilled";
                PlayCommandText = "停止";
            }
            else
            {
                MediaPlayer.Stop();
                PlayCommandIcon = "PlayFilled";
                PlayCommandText = "播放";
            }

        }), hasMediaObservable);

        //声音
        SoundCommand = ReactiveCommand.Create(() => Op(() =>
        {
            DisableSound = !DisableSound;
            if (DisableSound)
            {
                Volume = 100;
            }
            else
            {
                Volume = 0;
            }
        }), stateChanged.Select(_ => active()));

        //StopCommand = ReactiveCommand.Create(
        //    () => Op(() => MediaPlayer.Stop()),
        //    stateChanged.Select(_ => active()));

        //PauseCommand = ReactiveCommand.Create(
        //    () => MediaPlayer.Pause(),
        //     stateChanged.Select(_ => active()));

        PauseCommand = ReactiveCommand.Create(() => Op(() =>
        {
            PauseStatus = !PauseStatus;
            if (PauseStatus)
            {
                MediaPlayer.Pause();
                PauseCommandIcon = "RepeatAll";
                PauseCommandText = "恢复";
            }
            else
            {
                MediaPlayer.Pause();
                PauseCommandIcon = "PauseFilled";
                PauseCommandText = "暂停";
            }

        }), stateChanged.Select(_ => active()));


        ForwardCommand = ReactiveCommand.Create(
            () => MediaPlayer.Time += 1000,
            stateChanged.Select(_ => active()));

        BackwardCommand = ReactiveCommand.Create(
            () => MediaPlayer.Time -= 1000,
            stateChanged.Select(_ => active()));

        NextFrameCommand = ReactiveCommand.Create(
            () => MediaPlayer.NextFrame(),
            stateChanged.Select(_ => active()));


        //添加项目
        AddPatient = ReactiveCommand.CreateFromTask(async () =>
        {
            var user = Globals.CurrentUser;
            var dialog = new ContentDialog()
            {
                FullSizeDesired = true,
                Title = "选择项目",
                PrimaryButtonText = "确定",
                CloseButtonText = "取消"
            };

            dialog.Content = new AddPatientView()
            {
                DataContext = new AddPatientViewModel(dialog, showAdd: false)
            };

            var ok = await dialog.ShowAsync();
            if (ok == ContentDialogResult.Primary)
            {
                CurrentPatient = Globals.CurrentPatient;
                LoadVideos(CurrentUser.Id, CurrentPatient.Id);
            }
        });

        this.WhenActivated((CompositeDisposable disposables) =>
        {
            //播放完成时
            Wrap(endReachedChanged).Skip(1).Subscribe(p =>
            {
                if (State == VLCState.Ended)
                {
                    UpdateUI(() =>
                    {
                        PlayStatus = false;
                        MediaPlayer.Stop();
                        PlayCommandIcon = "PlayFilled";
                        PlayCommandText = "播放";
                    });
                }

            }).DisposeWith(disposables);


            Wrap(playingChanged).Subscribe(_ =>
            {
                UpdateUI(() =>
                {
                    if (_played != null)
                    {
                        if (!_played.Contains(MediaUrl))
                        {
                            _played.Add(MediaUrl);
                            SavePlayed();
                        }
                    }
                });
            }).DisposeWith(disposables);

            //LoadData
            RxApp.MainThreadScheduler.Schedule(LoadData)
            .DisposeWith(disposables);
        });
    }

    private async void UpdateUI(Action action)
    {
        await Dispatcher.UIThread.InvokeAsync(action, DispatcherPriority.Background);
    }

    /// <summary>
    /// 计时器
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
    {
        Dispatcher.UIThread.InvokeAsync(new Action(() =>
        {
            try
            {
                TimeCount = DateTime.Now - TimeNow;
                Ptimer = string.Format("{0:00}:{1:00}:{2:00}:{3:000}", TimeCount.Hours, TimeCount.Minutes, TimeCount.Seconds, TimeCount.Milliseconds);
                //Ptimer = TimeCount.ToString("%H:mm:s:fff");
            }
            catch (Exception) { }
        }));
    }

    private void LoadData()
    {
        //MediaUrl = @"C:\Users\Administrator\Desktop\Dorisoy\demo.mp4";
        if (CurrentUser.Id != Guid.Empty && CurrentPatient.Id != Guid.Empty)
            LoadVideos(CurrentUser.Id, CurrentPatient.Id);
    }


    /// <summary>
    /// 刷新项目图片视频
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="patientId"></param>
    private async void LoadVideos(Guid userId, Guid patientId)
    {
        var setting = _settingsProvider.Settings;
        await Dispatcher.UIThread.InvokeAsync(async () =>
        {
            try
            {
                Videos.Clear();
                var _pathHelper = new PathHelper();
                //获取位图文档
                var docs = await _documentService.GetPatienterDocuments(userId, patientId);
                if (docs != null && docs.Any())
                {
                    foreach (var doc in docs)
                    {
                        if (doc.Extension.Contains(".avi"))
                        {
                            //获取视频缩略图
                            var name = doc.Name.Replace(".avi", ".bmp");
                            var tdoc = docs.Where(s => s.Name == name).FirstOrDefault();
                            if (tdoc != null)
                            {
                                //更新待预览区
                                tdoc.FileType = FileType.Video;

                                //视频缩略图
                                tdoc.Path = setting.GetHost() + $"/document/{tdoc.Id}/download";

                                //视频地址
                                tdoc.PathURL = setting.GetHost() + $"/document/{doc.Id}/download";

                                //选择视频播放
                                tdoc.PlayCommand = ReactiveCommand.Create<DocumentModel>((item) =>
                                {
                                    UpdateUI(() =>
                                    {
                                        try
                                        {
                                            MediaUrl = tdoc.PathURL;
                                            MediaPlayer.Media = new Media(_libVLC, MediaUrl, FromType.FromLocation);
                                            MediaPlayer.Play();
                                            PlayCommandIcon = "StopFilled";
                                            PlayCommandText = "停止";
                                            PlayStatus = true;
                                        }
                                        catch (Exception ex)
                                        {
                                            MessageBox(ex.Message);

                                        }
                                    });
                                });
                                Videos.Add(tdoc);
                            }
                        }
                    }
                }
            }
            catch (Exception) { }
        });
    }

    protected override void DisposeUnmanaged()
    {
        if (_subscriptions != null)
        {
            _subscriptions.Dispose();
            _subscriptions = null;
        }

        if (MediaPlayer != null)
        {
            MediaPlayer.Stop();
            MediaPlayer.Dispose();
        }
    }
}
