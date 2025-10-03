namespace Dorisoy.PanClient.Chat;

/// <summary>
/// 表示数据共享
/// </summary>
public abstract class DataSharing
{
    public event PropertyChangedEventHandler PropertyChanged;
    protected void OnPropertyChanged(string name)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }

    /// <summary>
    /// 用于标记数据(通道)的索引（AudioSharing：0，VideoSharing：1，FileSharing：2，ScreenSharing：3）
    /// </summary>
    public int LineIndex { get; set; }

    /// <summary>
    /// 是否发送
    /// </summary>
    public bool IsSending { get; private set; }

    public bool IsComes { get; protected set; }

    private readonly VoiceChatModel vcm;

    protected BdtpClient BdtpClient
    {
        get => vcm.bdtpClient;
    }

    public DataSharing(VoiceChatModel vcm)
    {
        this.vcm = vcm;
    }

    /// <summary>
    /// 开始发送，更新 IsSending 标记 true
    /// </summary>
    public virtual void BeginSend()
    {
        if (IsSending)
            return;

        IsSending = true;
        OnPropertyChanged("IsSending");
    }




    private CancellationTokenSource _cts;
    private Task receiveThread;

    /// <summary>
    /// 开始接收
    /// </summary>
    public virtual void BeginReceive()
    {
        _cts = new CancellationTokenSource();
        receiveThread = Task.Run(ReceiveLoop, _cts.Token);
    }

    /// <summary>
    /// 结束发送，更新 IsSending 标记 false
    /// </summary>
    public virtual void EndSend()
    {
        if (!IsSending)
            return;

        IsSending = false;
        OnPropertyChanged("IsSending");
    }

    /// <summary>
    /// 结束接收
    /// </summary>
    public virtual void EndReceive()
    {
        if (_cts == null || _cts.IsCancellationRequested)
            return;

        _cts?.Cancel();
    }

    protected virtual void Send(object sender, EventArgs e)
    {
        if (vcm.State != ModelStates.Talk)
        {
            return;
        }
    }

    /// <summary>
    /// 处理数据接收
    /// </summary>
    protected abstract void Receive();

    /// <summary>
    /// 循环接收
    /// </summary>
    protected void ReceiveLoop()
    {
        while (!_cts.IsCancellationRequested && BdtpClient.Connected && vcm.State == ModelStates.Talk)
        {
            Receive();
            Thread.Sleep(0);
        }
    }

    /// <summary>
    /// 开关数据发送状态
    /// </summary>
    public void SwitchSendingState()
    {
        if (IsSending)
        {
            EndSend();
        }
        else
        {
            BeginSend();
        }
    }
}
