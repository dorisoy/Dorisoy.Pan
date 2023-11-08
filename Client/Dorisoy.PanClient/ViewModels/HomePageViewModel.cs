using System.Diagnostics;
using Dorisoy.PanClient.Views;

namespace Dorisoy.PanClient.ViewModels;

[View(typeof(HomePage))]
public class HomePageViewModel : MainPageViewModelBase
{
    [Reactive] public string Welecom { get; set; }
    [Reactive] public string RunTime { get; set; } = "00:00:00:00";
    [Reactive] public string CpuRate { get; set; } = "0% (0.00 GHz)";
    [Reactive] public string CpuCore { get; set; } = "0%";
    [Reactive] public string MemeryRate { get; set; } = "0.0/0.0GB (0%)";
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

    [Reactive] public int OnlineUsers { get; set; }

    public ReactiveCommand<Unit, Unit> VideoCountCommand { get; }
    public ReactiveCommand<Unit, Unit> UserCountCommand { get; }
    public ReactiveCommand<Unit, Unit> DocumentCountCommand { get; }
    public ReactiveCommand<Unit, Unit> DepmentCountCommand { get; }


    /// <summary>
    /// 时间统计
    /// </summary>
    private System.Timers.Timer timer;
    private System.DateTime TimeNow = new DateTime();
    private TimeSpan TimeCount = new TimeSpan();

    private readonly IDocumentService _documentService;
    private readonly IUsersService _usersService;

    public HomePageViewModel() : base()
    {
        _documentService = Locator.Current.GetService<IDocumentService>();
        _usersService = Locator.Current.GetService<IUsersService>();


        if (timer == null)
        {
            timer = new() { Interval = 1 };
            timer.Elapsed += timer_Elapsed;
            timer.Start();
        }

        VideoCountCommand = ReactiveCommand.Create(() =>
        {
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

        this.WhenActivated((CompositeDisposable disposables) =>
        {
            try
            {
                OnlineUsers = Globals.Onlines;
                Welecom = TimeFix();

                Task.Run(() => ConsumeCPU());

                Task.Run(async () =>
                {
                    var statistics = await _documentService.GetDocumentsCount(CurrentUser.Id);
                    DocumentCount = statistics;
                });

                Task.Run(async () =>
                {
                    var statistics = await _documentService.GetDocumentsCount(CurrentUser.Id, ".avi");
                    VideoCount = statistics;
                });

                Task.Run(async () =>
                {
                    var statistics = await _usersService.GetUsers();
                    UserCount = statistics?.Count() ?? 0;
                });
            }
            catch { }
        });
    }

    public string TimeFix()
    {
        var time = DateTime.Now;
        var hour = time.Hour;
        var tag = hour < 9 ? "早上好" : hour <= 11 ? "上午好" : hour <= 13 ? "中午好" : hour < 20 ? "下午好" : "晚上好";
        return $"Hi , {CurrentUser?.RaleName ?? ""} {tag},欢迎访问系统！";
    }


    private void timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
    {
        Dispatcher.UIThread.InvokeAsync(() =>
        {
            try
            {
                TimeCount = DateTime.Now - TimeNow;
                RunTime = string.Format("{0:00}:{1:00}:{2:00}", TimeCount.Hours, TimeCount.Minutes, TimeCount.Seconds);
            }
            catch (Exception) { }
        });
    }

    private async void ConsumeCPU()
    {
        Process cur = Process.GetCurrentProcess();
        SystemInfo sys = new SystemInfo();
        PerformanceCounter curtime = new PerformanceCounter("Process", "% Processor Time", cur.ProcessName);

        //const int KB_DIV = 1024;
        const int MB_DIV = 1024 * 1024;
        //const int GB_DIV = 1024 * 1024 * 1024;

        while (true)
        {
            try
            {
                await Task.Delay(1000);
                // 获取CPU使用率
                await Dispatcher.UIThread.InvokeAsync(() =>
                {
                    var use = (sys.PhysicalMemory - sys.MemoryAvailable) / MB_DIV;
                    //var residue = (sys.PhysicalMemory - sys.MemoryAvailable) / (double)GB_DIV;
                    var residue = curtime.NextValue() / Environment.ProcessorCount;

                    ProcessName = cur.ProcessName;
                    CpuCore = $"{residue.ToString("0.0")}%";
                    CpuRate = $"{sys.CpuLoad.ToString("0.0")}%";
                    MemeryRate = $"{use.ToString("0.0")}MB";
                });
            }
            catch (Exception) { }
        }
    }

    protected override void DisposeManaged() { }
    protected override void DisposeUnmanaged() { }
}

