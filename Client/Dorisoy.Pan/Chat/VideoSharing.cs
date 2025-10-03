namespace Dorisoy.PanClient.Chat;

/// <summary>
/// 表示视频共享
/// </summary>
public class VideoSharing : DataSharing
{
    private CancellationTokenSource _cts;
    private Task _previewTask;
    private ServiceHub services;

    /// <summary>
    /// 设备索引
    /// </summary>
    private int _openCvId { get; set; }
    public int OpenCvId
    {
        get => _openCvId;
        set
        {
            _openCvId = value;
            OnPropertyChanged("OpenCvId");
        }
    }

    /// <summary>
    /// 表示远程视频帧
    /// </summary>
    private Bitmap remoteFrame;
    public Bitmap RemoteFrame
    {
        get => remoteFrame;
        set
        {
            if (IsComes)
            {
                remoteFrame = value;
            }
            else
            {
                remoteFrame = null;
            }
            OnPropertyChanged("RemoteFrame");
        }
    }

    /// <summary>
    /// 表示本地视频帧
    /// </summary>
    private Bitmap localFrame;
    public Bitmap LocalFrame
    {
        get => localFrame;
        set
        {
            if (IsSending)
            {
                localFrame = value;
            }
            else
            {
                localFrame = null;
            }

            OnPropertyChanged("LocalFrame");
        }
    }


    public VideoSharing(VoiceChatModel model) : base(model)
    {
        LineIndex = 1;

        services = ServiceHub.Instance;
        
    }

    /// <summary>
    /// 接收标记
    /// </summary>
    /// <param name="buffer"></param>
    public void ReceiveFlags(byte[] buffer)
    {
        //开始视频发送
        if (VoiceChatModel.IsFlag(Flags.BeginVideoSend, buffer))
            IsComes = true;

        //结束视频发送
        if (VoiceChatModel.IsFlag(Flags.EndVideoSend, buffer))
            IsComes = false;
    }

    /// <summary>
    /// 开始发送
    /// </summary>
    public override async void BeginSend()
    {
        base.BeginSend();

        await StartOpenCvCapture();

        BdtpClient.SendReceipt(new byte[] { (byte)Flags.BeginVideoSend });
    }

    /// <summary>
    /// 开始接收
    /// </summary>
    public override void BeginReceive()
    {
        base.BeginReceive();
    }

    /// <summary>
    /// 结束视频发送
    /// </summary>
    public override async void EndSend()
    {
        base.EndSend();

        if (_previewTask == null)
            return;

        await StopOpenCvCapture();

        BdtpClient.SendReceipt(new byte[] { (byte)Flags.EndVideoSend });

        LocalFrame = null;
    }

    /// <summary>
    /// 结束接收
    /// </summary>
    public override void EndReceive()
    {
        base.EndReceive();
        IsComes = false;
    }

    /// <summary>
    /// 清除帧
    /// </summary>
    public void ClearFrames()
    {
        RemoteFrame?.SafeDispose();
        LocalFrame?.SafeDispose();
        RemoteFrame = null;
        LocalFrame = null;
    }

    private UdpSession udpSenders;

    /// <summary>
    /// 重写处理数据接收
    /// </summary>
    protected override void Receive()
    {
        if (udpSenders == null)
            udpSenders = BdtpClient.GetUdpSession(LineIndex);

        if (!udpSenders.DisposedValue)
        {
            udpSenders.Received = (client, e) =>
            {
                try
                {
                    // 接收数据包
                    byte[] data = e.ByteBlock.ToArray();
                    Dispatcher.UIThread.InvokeAsync(() =>
                    {
                        using var ms = new MemoryStream(data);
                        var bitmap = new Avalonia.Media.Imaging.Bitmap(ms);
                        if (bitmap != null)
                        {
                            RemoteFrame = bitmap;
                        }
                    });

                    return Task.FromResult(true);

                }
                catch (Exception ex)
                {
                    return Task.FromResult(false);
                }
            };
        }
    }

    /// <summary>
    /// 开启摄像头
    /// </summary>
    /// <returns></returns>
    public async Task StartOpenCvCapture()
    {
        if (_previewTask != null && !_previewTask.IsCompleted)
            return;

        var initializationSemaphore = new SemaphoreSlim(0, 1);

        _cts = new CancellationTokenSource();
        _previewTask = Task.Run(() =>
        {
            try
            {
                var videoCapture = new VideoCapture(_openCvId, VideoCapture.API.DShow);
                videoCapture.Set(Emgu.CV.CvEnum.CapProp.FrameWidth, 1280);
                videoCapture.Set(Emgu.CV.CvEnum.CapProp.FrameHeight, 720);

                //对每一帧进行处理
                using (var frame = new Mat())
                {
                    while (!_cts.IsCancellationRequested)
                    {
                        try
                        {
                            videoCapture.Read(frame);


                            if (!frame.IsEmpty)
                            {
                                // 释放第一个非空帧上的锁定
                                if (initializationSemaphore != null)
                                    initializationSemaphore.Release();
                            }

                            // 转换格式
                            using Image<Rgb, byte> img_trans = frame.ToImage<Rgb, byte>();

                            // JPEG压缩
                            byte[] bytes = img_trans.ToJpegData(85);

                            using var ms = new MemoryStream(bytes);
                            var bitmap = new Bitmap(ms);

                            if (bitmap != null)
                            {
                                Dispatcher.UIThread.Invoke(() =>
                                {
                                    LocalFrame = bitmap;
                                });
                            }

                            //发送UDP数据
                            BdtpClient.Send(bytes, LineIndex);
                        }
                        catch { }
                    }
                }

                videoCapture?.SafeDispose();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            finally
            {
                if (initializationSemaphore != null)
                    initializationSemaphore.Release();
            }

        }, _cts.Token);

        // 异步初始化可以在不冻结GUI的情况下显示动画加载程序
        // 另一种选择是长轮询。（while！variable）等待任务。延迟
        await initializationSemaphore.WaitAsync();
        initializationSemaphore.Dispose();
        initializationSemaphore = null;

        if (_previewTask.IsFaulted)
        {
            // 异常退出
            await _previewTask;
        }
    }

    /// <summary>
    /// 开启摄像头
    /// </summary>
    public void StartOpenCvCapture2()
    {
        _cts = new CancellationTokenSource();
        // 叫接收者不应该在呼叫请求时激活摄像头，只有当呼叫完成时。
//
    }

    public void StopOpenCvCapture2()
    {
        if (_cts == null || _cts.IsCancellationRequested)
            return;

        _cts.Cancel();

        //services.VideoHandler.CloseCamera();
    }

    /// <summary>
    /// 停止摄像头
    /// </summary>
    /// <returns></returns>
    public async Task StopOpenCvCapture()
    {
        if (_cts == null || _cts.IsCancellationRequested)
            return;

        if (!_previewTask.IsCompleted)
        {
            _cts.Cancel();

            // 等待，以避免与_lastFrame的读/写冲突
            await _previewTask;
        }
    }
}
