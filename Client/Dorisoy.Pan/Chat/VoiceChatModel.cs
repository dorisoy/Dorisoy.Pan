using Dorisoy.Pan.ViewModels;

namespace Dorisoy.Pan.Chat;

/// <summary>
/// 确认标记
/// </summary>
public enum Flags
{
    /// <summary>
    /// 接受
    /// </summary>
    Accept,
    /// <summary>
    /// 开始视频发送
    /// </summary>
    BeginVideoSend,
    /// <summary>
    /// 结束视频发送
    /// </summary>
    EndVideoSend
}

/// <summary>
/// Chat状态
/// </summary>
public enum ModelStates
{
    /// <summary>
    /// 等待呼叫
    /// </summary>
    WaitCall,
    /// <summary>
    /// 呼出
    /// </summary>
    OutgoingCall,
    /// <summary>
    /// 来电
    /// </summary>
    IncomingCall,
    /// <summary>
    /// 对话
    /// </summary>
    Talk,
    /// <summary>
    /// 对话
    /// </summary>
    EndTalk,
    /// <summary>
    /// 关闭
    /// </summary>
    Close
}

/// <summary>
/// Chat状态更新
/// </summary>
public class CVChangeEventArgs : EventArgs
{
    public CVChangeEventArgs(ModelStates states)
    {
        States = states;
    }
    public ModelStates States { get; }
}

/// <summary>
/// CallTime更新
/// </summary>
public class CallTimerEventArgs : EventArgs
{
    public CallTimerEventArgs(string s)
    {
        Value = s;
    }
    public string Value { get; }
}

/// <summary>
/// 表示语音聊天模型
/// </summary>
public class VoiceChatModel : ReactiveObject, IDisposable
{
   
    private const int LINES_COUNT = 4;
    public BdtpClient bdtpClient;
    private Task waitCallTask;

    private MediaSounds mediaSounds;
    public AudioSharing audio;
    public VideoSharing video;
    public FileSharing fileShare;

    public IPAddress LocalIP => bdtpClient.LocalIP;
    public bool Connected => bdtpClient.Connected;

    public MainViewViewModel mvvm { get; }
    [Reactive] public IPAddress RemoteIP { get; set; }

    private ModelStates state;
    public ModelStates State
    {
        get
        {
            return state;
        }
        set
        {
            state = value;

            //OnPropertyChanged("State");

            OnStateChange?.Invoke(this, new CVChangeEventArgs(state));

            //播放声音
            if (state == ModelStates.WaitCall || state == ModelStates.IncomingCall)
                mediaSounds.Play();
        }
    }


    public CallTimer callTimer;
    public event EventHandler<CVChangeEventArgs> OnStateChange;
    public event EventHandler<CallTimerEventArgs> OnCallTimerChange;

    public VoiceChatModel(MainViewViewModel main)
    {
        mvvm = main;

        // 创建bdtpClient 实例
        bdtpClient = new BdtpClient(CommonHelper.GetLocalIP(), LINES_COUNT);

        // 创建音频共享实例
        audio = new AudioSharing(this);

        // 创建视频共享实例
        video = new VideoSharing(this);

        // 创建MediaSounds实例
        mediaSounds = new MediaSounds(this);

        // 创建文件共享实例
        fileShare = new FileSharing(this);


        // 计时器
        callTimer = new CallTimer();

        // 初始化事件
        InitializeEvents();

        // 开始等待呼叫
        BeginWaitCall();
    }


    /// <summary>
    /// 初始化事件
    /// </summary>
    private void InitializeEvents()
    {
        bdtpClient.ReceiptReceived += ReceiveAccept;
        bdtpClient.ReceiptReceived += ReceiveDisconnect;
        bdtpClient.ReceiptReceived += video.ReceiveFlags;
        bdtpClient.ReceiptInfoReceived += InfoReceived;
    }

    private void InfoReceived(string obj)
    {
        //mvvm?.WriteMessage(obj);
    }

    public void ScreenLocalFrameReceived()
    {
        mvvm.LocalFrame = mvvm.ScreenFrame.ToAvaloniaBitmap();
    }

    public void ScreenRemoteFrameReceived()
    {
        mvvm.RemoteFrame = mvvm.RemoteScreenFrame.ToAvaloniaBitmap();
    }

    private void ReceiveAccept(byte[] buffer)
    {
        if (IsFlag(Flags.Accept, buffer))
        {
            State = ModelStates.Talk;
        }
    }

    /// <summary>
    /// 接收断开
    /// </summary>
    /// <param name="buffer"></param>
    private void ReceiveDisconnect(byte[] buffer)
    {
        if (buffer.Length == 0)
        {
            //断开时停止播放铃声
            mediaSounds?.Stop();

            //结束呼叫后 -> 重新开始等待传入呼叫
            EndCall();
        }
    }

    /// <summary>
    /// 结束呼叫
    /// </summary>
    public void EndCall()
    {
        if (State == ModelStates.Talk)
            EndTalk();

        bdtpClient.Disconnect();

        // 重新开始等待传入呼叫
        BeginWaitCall();
    }

    private CancellationTokenSource _cts;

    /// <summary>
    /// 开始等待传入呼叫
    /// </summary>
    private void BeginWaitCall()
    {
        try
        {
            if (State == ModelStates.Close)
                return;

            //if (waitCallTask != null && !waitCallTask.IsCompleted)
            //    return;

            _cts = new CancellationTokenSource();

            Thread.Sleep(100);

            waitCallTask = Task.Run(WaitCall, _cts.Token);
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.Message);
        }
    }

    /// <summary>
    /// 等待呼叫
    /// </summary>
    private void WaitCall()
    {
        // 标记等待呼叫
        State = ModelStates.WaitCall;

        if (bdtpClient.Accept())
        {
            RemoteIP = bdtpClient.RemoteIP;
            State = ModelStates.IncomingCall;
        }

        EndWaitCall();
    }

    /// <summary>
    /// 结束等待呼叫
    /// </summary>
    private void EndWaitCall()
    {
        //TCP 监听停止（停止等待传入的连接请求）
        bdtpClient.StopAccept();

        //// 如果在停止之前调用“Dispose”
        //if (_cts == null || _cts.IsCancellationRequested)
        //    return;

        //if (!waitCallTask.IsCompleted)
        //{
        //    // 取消等待呼叫
        //    _cts?.Cancel();

        //    // 等待完成
        //    await waitCallTask;
        //}

        // 取消等待呼叫
        _cts?.Cancel();
    }

    /// <summary>
    /// 接受呼叫
    /// </summary>
    public void AcceptCall()
    {
        //接收时发送确认标记,向连接到的主机发送TCP确认. 发送Accept标记
        if (bdtpClient.SendReceipt(new byte[] { (byte)Flags.Accept }))
        {
            //断开时停止播放铃声
            mediaSounds?.Stop();
            //开始对话
            BeginTalk();
            //连接到呼叫者，作为呼叫者客户端
            bdtpClient.AcceptCallConnect();
        }
    }

    /// <summary>
    /// 拒绝呼叫
    /// </summary>
    public void DeclineCall()
    {
        //断开时停止播放铃声
        mediaSounds?.Stop();
        bdtpClient.Disconnect();
        BeginWaitCall();
    }

    /// <summary>
    /// 选择远程用户呼叫
    /// </summary>
    /// <param name="user"></param>
    public void BeginCall(OnlinUserUserModel user)
    {
        //当前会话用户
        mvvm.RemoteUser = user;

        //更新呼出状态
        State = ModelStates.OutgoingCall;

        // 如果有等待则先，结束等待呼叫
        EndWaitCall();

        //远程会话IP
        RemoteIP = IPAddress.Parse(user.IP);

        // 连接并等待响应，使用指定的IP地址将客户端连接到远程BDTP主机
        if (bdtpClient.Connect(RemoteIP) && WaitAccept())
        {
            BeginTalk();
            user.Connected = true;
        }
        else
        {
            EndCall();
            user.Connected = false;
        }
    }

    /// <summary>
    /// 开始对话
    /// </summary>
    private void BeginTalk()
    {
        try
        {
            // 标记对话状态
            State = ModelStates.Talk;

            callTimer.Start();

            audio.BeginSend();
            audio.BeginReceive();

            video.BeginReceive();

            fileShare.BeginReceive();

        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.Message);
        }
    }

    /// <summary>
    /// 结束开始对话
    /// </summary>
    private void EndTalk()
    {
        State = ModelStates.EndTalk;

        audio.EndSend();
        audio.EndReceive();

        video.EndSend();
        video.EndReceive();
        video.ClearFrames();

        callTimer.Stop();
    }



    /// <summary>
    /// 关闭
    /// </summary>
    public void Closing()
    {
        try
        {
            //结束对话
            if (State == ModelStates.Talk)
                EndTalk();

            //关闭状态
            State = ModelStates.Close;

            //结束呼叫
            EndCall();

            mediaSounds?.Stop();
            bdtpClient?.Dispose();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine(ex.Message);
        }
    }

    /// <summary>
    /// 等待接收
    /// </summary>
    /// <returns></returns>
    private bool WaitAccept()
    {
        //已经连接且状态不在会话时,继续等待
        while (bdtpClient.Connected && State != ModelStates.Talk);

        //如果已经会话，返回true
        if (State == ModelStates.Talk)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    // 确认处理程序
    public static bool IsFlag(Flags flag, byte[] buffer)
    {
        return buffer.Length == 1 && buffer[0] == (byte)flag;
    }


    //文件传输 fileShare



    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!m_disposed)
        {
            if (disposing)
            {
                //释放托管资源
                bdtpClient.SafeDispose();
            }

            // 释放非托管资源

            m_disposed = true;
        }
    }

    ~VoiceChatModel()
    {
        Dispose(false);
    }

    private bool m_disposed;
}
