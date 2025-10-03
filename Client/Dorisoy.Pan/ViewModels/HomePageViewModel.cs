using System.Diagnostics;
using Disposable = System.Reactive.Disposables.Disposable;

namespace Dorisoy.PanClient.ViewModels;

public class HomePageViewModel : MainPageViewModelBase
{
    [Reactive] public string Welecom { get; set; }
    [Reactive] public string RunTime { get; set; } = "00:00:00:00";
    [Reactive] public double CpuRate { get; set; } = 0; //"0% (0.00 GHz)";
    [Reactive] public double CpuCore { get; set; } = 0; //"0%";
    [Reactive] public double MemeryRate { get; set; } = 0; //"0.0/0.0GB (0%)";
    [Reactive] public string MemeryUsed { get; set; } 
    [Reactive] public string ProcessName { get; set; } = "VCMS";

    /// <summary>
    /// 视频数
    /// </summary>
    [Reactive] public int VideoCount { get; set; } = 0;

    /// <summary>
    /// 用户数
    /// </summary>
    [Reactive] public int UserCount { get; set; } = 0;

    /// <summary>
    /// 文档数
    /// </summary>
    [Reactive] public int DocumentCount { get; set; } = 0;
    [Reactive] public double VideoStorageSpace { get; set; } = 50;
    [Reactive] public double ImageStorageSpace { get; set; } = 50;



    public ReactiveCommand<Unit, Unit> VideoCountCommand { get; }
    public ReactiveCommand<Unit, Unit> UserCountCommand { get; }
    public ReactiveCommand<Unit, Unit> DocumentCountCommand { get; }
    public ReactiveCommand<Unit, Unit> DepmentCountCommand { get; }
    public ReactiveCommand<Unit, Unit> RecordingCommand { get; }

    private readonly IDocumentService _documentService;
    private readonly IUsersService _usersService;
    private CancellationTokenSource _cts;
    private readonly ISettingsProvider<AppSettings> _settingsProvider;

    [Reactive] public SoundSliceData SliceData1 { get; set; }
    [Reactive] public SoundSliceData SliceData2 { get; set; }

    public HomePageViewModel() : base()
    {
        _documentService = Locator.Current.GetService<IDocumentService>();
        _usersService = Locator.Current.GetService<IUsersService>();
        _settingsProvider = Locator.Current.GetService<ISettingsProvider<AppSettings>>();


        VideoCountCommand = ReactiveCommand.Create(() =>
        {
            if (Host is MainView mianView)
                mianView.NavigateTo(typeof(MonitorPageViewModel));
        });

        UserCountCommand = ReactiveCommand.Create(() =>
        {
            if (Host is MainView mianView)
                mianView.NavigateTo(typeof(UserPageViewModel));
        });

        DocumentCountCommand = ReactiveCommand.Create(() =>
        {
            if (Host is MainView mianView)
                mianView.NavigateTo(typeof(DocumentPageViewModel));
        });

        DepmentCountCommand = ReactiveCommand.Create(() =>
        {
          
        });

        //屏幕录制
        RecordingCommand = ReactiveCommand.Create(() =>
        {
            try
            {
                var setting = _settingsProvider.Settings;
                string wpfExePath = @".\ScreenRecording\ScreenRecording.exe";
                string commandLineArgs = $"{CurrentUser.Id} {setting.ServerIP}";
                var startInfo = new ProcessStartInfo
                {
                    FileName = wpfExePath,
                    UseShellExecute = false,
                    Arguments = commandLineArgs,
                };
                App.MainWindow.myProcess = Process.Start(startInfo);
            }
            catch (Exception ex)
            {
                MessageBox(ex.Message);
            }
        });

        this.WhenAnyValue(x => x.CpuRate)
            .WhereNotNull()
            .Subscribe(x =>
            {
                SliceData1 = RandomData(20);
                SliceData2 = RandomData(20);
            });

        this.WhenActivated((CompositeDisposable disposables) =>
        {
            _cts = new CancellationTokenSource();

            ConnectAsync();

            //统计CPU 使用率
            Observable.Timer(TimeSpan.FromSeconds(1))
            .Subscribe(_ =>
            {
                ConsumeCPU(_cts.Token);
            }).DisposeWith(disposables);

            //Observable.Interval(TimeSpan.FromSeconds(5))
            //.ObserveOn(AvaloniaScheduler.Instance)
            //.Subscribe(_ =>
            //{
            //    LoadOnlineUserData();
            //    OnlineUsers = Globals.Onlines;

            //}).DisposeWith(disposables);

            SliceData1 = RandomData(20);
            SliceData2 = RandomData(20);
            Welecom = TimeFix();

            LoadDataCommand.Execute(_cts.Token)
            .Subscribe()
            .DisposeWith(disposables);

            Disposable.Create(() => _cts.Cancel()).DisposeWith(disposables);
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
            if (CurrentUser != null)
            {
                var docCount = await _documentService.GetDocumentsCount(CurrentUser.Id);
                var mCount = await _documentService.GetDocumentsCount(CurrentUser.Id, ".avi");
                var uCount = await _usersService.GetUsers();

                await Dispatcher.UIThread.InvokeAsync(() =>
                {
                    DocumentCount = docCount;
                    VideoCount = mCount;
                    UserCount = uCount?.Count ?? 0;
                });
            }
        }, token);
    }


    public SoundSliceData RandomData(int count)
    {
        var sums = new float[count];
        var sum = new Random();
        for (int i = 0; i < count; i++)
        {
            sums[i] = sum.Next(10, 100);
        }
        return new SoundSliceData(sums);
    }

    /// <summary>
    /// 欢迎语
    /// </summary>
    /// <returns></returns>
    public string TimeFix()
    {
        var time = DateTime.Now;
        var hour = time.Hour;
        var tag = hour < 9 ? "早上好" : hour <= 11 ? "上午好" : hour <= 13 ? "中午好" : hour < 20 ? "下午好" : "晚上好";
        return $"Hi , {CurrentUser?.RaleName ?? ""} {tag},欢迎访问系统！";
    }

    /// <summary>
    /// 统计CPU 使用率
    /// </summary>
    private void ConsumeCPU(CancellationToken token)
    {
        Task.Run(() =>
        {
            // 获取当前进程
            var process = Process.GetCurrentProcess();
         
            // CPU 使用率计算需要的变量
            var startCpuUsage = process.TotalProcessorTime;
            // 获取当前进程的内存使用量（工作集）
            long processMemory = process.WorkingSet64;
            // 获取系统的总物理内存量
            var sys = new SystemInfo();
            long totalPhysicalMemory = sys.PhysicalMemory;

            var curtime = new PerformanceCounter("Process", "% Processor Time", process.ProcessName);

            while (!token.IsCancellationRequested)
            {
                Thread.Sleep(1000);
                var residue = curtime.NextValue() / Environment.ProcessorCount;
                // 获取内存使用
                process.Refresh();
                processMemory = process.WorkingSet64;
                // 计算内存使用率
                double memoryUsagePercentage = (double)processMemory / totalPhysicalMemory * 100;

                Dispatcher.UIThread.InvokeAsync(() =>
                {
                    CpuRate = Math.Round(sys.CpuLoad, 1);
                    MemeryRate = Math.Round(memoryUsagePercentage, 1);
                    MemeryUsed = $"{processMemory / 1024 / 1024:0.0}";
                });

            }

        }, token);
    }

    protected override async void DisposeUnmanaged()
    {
        await DisconnectAsync();
    }
}

