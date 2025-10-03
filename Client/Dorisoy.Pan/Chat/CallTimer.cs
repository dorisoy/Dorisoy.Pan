namespace Dorisoy.PanClient.Chat;

/// <summary>
/// 计时器
/// </summary>
public class CallTimer
{
    public event PropertyChangedEventHandler PropertyChanged;
    protected void OnPropertyChanged(string PropertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(PropertyName));
    }

    

    public TimeSpan CallTime
    {
        get
        {
            return callTime;
        }
        set
        {
            callTime = value;
            OnPropertyChanged("CallTime");
        }
    }
    private TimeSpan callTime;
    private DispatcherTimer timer;

    /// <summary>
    /// 调用计时器
    /// </summary>
    public CallTimer()
    {
        timer = new DispatcherTimer();
        timer.Interval = TimeSpan.FromSeconds(1);
        timer.Tick += (sender, e) => CallTime += timer.Interval;
    }

    public void Start()
    {
        timer.Start();
    }

    public void Stop()
    {
        timer.Stop();
        CallTime = new TimeSpan(0);
    }
}
