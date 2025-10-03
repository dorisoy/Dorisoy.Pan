using H264Sharp;
using NetworkLibrary;
using NetworkLibrary.Components;

using Dorisoy.PanClient.Services.Video.Camera;
using Dorisoy.PanClient.Services.Video.H264;

namespace Dorisoy.PanClient.Services.Video;

/// <summary>
/// 表示一个视频处理器
/// </summary>
public class VideoHandler2
{
    public Action<byte[], int, bool> OnBytesAvailable;
    public Action<ImageReference> OnLocalImageAvailable;
    public Action<ImageReference> OnRemoteImageAvailable;
    public Action<Action<PooledMemoryStream>, int, bool> OnBytesAvailableAction;
    public Action KeyFrameRequested;
    public Action<byte[], int, int> MarkingFeedback;
    public Action<byte[], int, int> LtrRecoveryRequest;
    public Action<int,int> CamSizeFeedbackAvailable;

    /// <summary>
    /// 捕获间隔Ms
    /// </summary>
    public int CaptureIntervalMs = 43;
    public int TargetBitrate = 1_500_000;
    public bool EnableCongestionAvoidance = true;

    /// <summary>
    /// 视频延迟
    /// </summary>
    public int VideoLatency { get; set; } = 200;
    public int AudioBufferLatency { get; set; } = 0;
    public double AverageLatency { get; set; } = -1;

    private object frameQLocker = new object();
    private ICameraProvider capture;

    /// <summary>
    /// H264转码器
    /// </summary>
    private H264Transcoder transcoder;

    /// <summary>
    /// 视频帧队列
    /// </summary>
    private readonly ConcurrentDictionary<DateTime, ImageReference> frameQueue = new ConcurrentDictionary<DateTime, ImageReference>();

    private readonly ConcurrentDictionary<Guid, DateTime> timeDict = new ConcurrentDictionary<Guid, DateTime>();
    private readonly ManualResetEvent consumerSignal = new ManualResetEvent(false);
    private Thread captureThread = null;
    private DateTime lastProcessedTimestamp = DateTime.Now;
    private DateTime latestAck;
    private ConfigType configType = ConfigType.CameraCaptureAdvanced;
    private int incomingFrameCount;
    private int capturedFrameCnt = 0;
    private int incomingFrameRate = 0;
    private int outgoingFrameRate = 0;
    private int bytesSent = 0;
    private int bytesReceived = 0;
    private int camIdx;
    private int fps = 30;
    private int unAcked = 0;
    private int actualFps = 0;
    private int currentBps = 0;
    private int frameQueueCount = 0;
    private int frameWidth = 640;
    private int frameHeight = 480;
    private int minBps = 300_000; //=> Math.Max(((TargetBitrate * 10) / 100),200_000);
    private int periodicKeyFrameInterval = -1;
    private long avgDivider = 2;
    private bool paused = false;
    private bool keyFrameRequested;
    private bool adjustCamSize = false;


    /// <summary>
    /// 表示摄像头设备状态
    /// </summary>
    private enum State
    {
        /// <summary>
        /// 闲置
        /// </summary>
        Idle,
        /// <summary>
        /// 正在获取摄像头
        /// </summary>
        ObtainingCamera,
        /// <summary>
        /// 已获取摄像头
        /// </summary>
        ObtaininedCamera,
        /// <summary>
        /// 捕获中
        /// </summary>
        Capturing,
        /// <summary>
        /// 执行捕获
        /// </summary>
        CaptureActive,
        /// <summary>
        /// 关闭中
        /// </summary>
        Closing,
        /// <summary>
        /// 释放中
        /// </summary>
        Disposing
    }

    private enum StateAction
    {
        /// <summary>
        /// 获取相机
        /// </summary>
        ObtainCamera,
        /// <summary>
        /// 捕获视频
        /// </summary>
       Capture,
       /// <summary>
       /// 关闭摄像头
       /// </summary>
       Close,
       /// <summary>
       /// 释放
       /// </summary>
       Dispose,
    }

    private State currentState;

    SingleThreadDispatcher marshaller = new SingleThreadDispatcher();


    public VideoHandler2()
    {
        currentState = State.Idle;
        transcoder=SetupTranscoder(fps, TargetBitrate);
        // 音频抖动同步
        StartFrameBuffer();
    }

    private readonly object locker = new object();
    private bool Execute(StateAction action)
    {
        if (action == StateAction.Dispose)
        {
            currentState = State.Disposing;
            marshaller.Enqueue(()=> Dispose_());
        }
        lock (locker)
            switch (currentState)
            {
                case State.Idle:
                    {
                        if (action == StateAction.ObtainCamera)
                        {
                            currentState = State.ObtainingCamera;
                            marshaller.EnqueueBlocking(() =>
                            {
                                bool isObtained = StartObtainCamera();
                                currentState = isObtained ? State.ObtaininedCamera : State.Idle;
                            });

                            return true;
                        }
                        return false;
                    }
                case State.ObtaininedCamera:
                    {
                        if (action == StateAction.Capture)
                        {
                            currentState = State.Capturing;

                            marshaller.Enqueue(() =>
                            {
                                StartCapturing_();
                                currentState = State.CaptureActive;
                            });

                            return true;
                        }
                        return false;
                    }
                case State.CaptureActive:
                    {
                        if (action == StateAction.Close)
                        {
                            currentState = State.Closing;
                            marshaller.Enqueue(() =>
                            {
                                CloseCamera_();
                                currentState = State.Idle;
                            });

                            return true;
                        }
                        return false;
                    }
                case State.Disposing:
                default:
                    return false;
            }
    }

    /// <summary>
    /// 开始帧缓冲
    /// </summary>
    private void StartFrameBuffer()
    {
        Thread t = new Thread(() =>
        {
            while (true)
            {
                if (Interlocked.CompareExchange(ref frameQueueCount, 0, 0) == 0)
                    consumerSignal.WaitOne();

                Thread.Sleep(1);//slow dispatch
                var videoJitterLatency = transcoder.Duration;

                if (frameQueue.Count > (AudioBufferLatency - Math.Min(AudioBufferLatency, videoJitterLatency)) / (1000 / Math.Max(1, incomingFrameRate)))
                {
                    KeyValuePair<DateTime, ImageReference> lastFrame;// oldest
                    lock (frameQLocker)
                    {
                        var samplesOrdered = frameQueue.OrderByDescending(x => x.Key);
                        lastFrame = samplesOrdered.Last();
                    }

                    bool dispose = true;
                    if (lastProcessedTimestamp < lastFrame.Key)
                    {
                        //远程映像可用时
                        OnRemoteImageAvailable?.Invoke(lastFrame.Value);
                        lastProcessedTimestamp = lastFrame.Key;
                        dispose = false;
                    }

                    if (frameQueue.TryRemove(lastFrame.Key, out var ff))
                    {
                        if (dispose)
                            ff?.Release();
                        Interlocked.Decrement(ref frameQueueCount);
                    }

                }
            }

        });
        t.Name = "FrameBufferThread";
        t.Start();
    }

    private H264Transcoder SetupTranscoder(int fps, int bps)
    {
        var transcoder = new H264Transcoder( fps, bps);

        transcoder.EncodedFrameAvailable = HandleEncodedFrame;
        //transcoder.EncodedFrameAvailable2 = HandleEncodedFrame2;
        transcoder.DecodedFrameAvailable = HandleDecodedFrame;
        transcoder.KeyFrameRequested = () => 
        { 
            //ServiceHub.Instance.Log("Info", "Requesting Key Frame"); KeyFrameRequested?.Invoke(); 
        } ;
        transcoder.MarkingFeedback = (b,o,c) => 
        { 
            MarkingFeedback?.Invoke(b,o,c); 
        } ;
        transcoder.LtrRecoveryRequest = (b,o,c) => 
        {
            //ServiceHub.Instance.Log("Info", "Requesting Ltr Recovery"); LtrRecoveryRequest?.Invoke(b,o,c); 
        } ;
        transcoder.SetKeyFrameInterval(periodicKeyFrameInterval);

        return transcoder;
    }


    /// <summary>
    /// 获取相机
    /// </summary>
    /// <returns></returns>
    public bool ObtainCamera()
    {
        return Execute(StateAction.ObtainCamera);
    }

    /// <summary>
    /// 开始捕获
    /// </summary>
    public void StartCapturing()
    {
        Execute(StateAction.Capture);
    }

    /// <summary>
    /// 关闭相机
    /// </summary>
    public void CloseCamera()
    {
        Execute(StateAction.Close);
    }

    /// <summary>
    /// 开始获取相机
    /// </summary>
    /// <returns></returns>
    private bool StartObtainCamera()
    {
        
        OnLocalImageAvailable?.Invoke(null);
        try
        {
            // new VideoCapture(camIdx, VideoCaptureAPIs.WINRT);
            capture = new WindowsCameraProvider(camIdx);

            capture.Open(camIdx);
            capture.FrameWidth = frameWidth;
            capture.FrameHeight = frameHeight;

            // DebugLogWindow.AppendLog("[Info] Camera Backend: ", capture.GetBackendName());
            //
            transcoder.SetupTranscoder(capture.FrameWidth, capture.FrameHeight, configType);
            transcoder.SetKeyFrameInterval(periodicKeyFrameInterval);
            transcoder.SetTargetBps(TargetBitrate);

            return capture.IsOpened();
        }
        catch(Exception e)
        {
           // ServiceHub.Instance.Log("Error","Unable to obtain camera;" + e.Message);
            return false;
        }
        
    }


    /// <summary>
    /// 关闭摄像头
    /// </summary>
    private void CloseCamera_()
    {
        //capStopped.WaitOne(500);
        OnLocalImageAvailable?.Invoke(null);
        try
        {
            Interlocked.Exchange(ref capture,null)?.Release();
        }
        catch 
        { 
           // ServiceHub.Instance.Log("Error", "Capture Release Failed");
        }

        Interlocked.Exchange(ref frameQueueCount, 0);

        transcoder?.FlushPool();
       

        OnLocalImageAvailable?.Invoke(null);
    }

   


    Stopwatch sw = new Stopwatch();


    /// <summary>
    /// 剩余时间
    /// </summary>
    private int remainderTime = 0;

    private void StartCapturing_()
    {
        adjustCamSize = false;

        sw.Reset();
        remainderTime = 0;

        marshaller.Enqueue(() =>
        {
            Cap();
        });
    }

    /// <summary>
    /// 捕获
    /// </summary>
    private void Cap()
    {
        if (currentState == State.CaptureActive)
        {
            try
            {
                if (paused)
                {
                    Thread.Sleep(10);
                    marshaller.Enqueue(Cap) ;
                    return;
                }

                sw.Restart();

                if (adjustCamSize)
                {
                    adjustCamSize = false;
                    if (capture.FrameWidth != frameWidth || capture.FrameHeight != frameHeight)
                    {
                        capture.Release();
                        capture.Dispose();
                        capture = new WindowsCameraProvider(camIdx);
                        //new VideoCapture(camIdx, VideoCaptureAPIs.MSMF);
                        capture.Open(camIdx);
                        capture.FrameWidth = frameWidth;
                        capture.FrameHeight = frameHeight;
                        frameWidth = capture.FrameWidth;
                        frameHeight = capture.FrameHeight;
                        CamSizeFeedbackAvailable?.Invoke(frameWidth, frameHeight);
                    }

                    transcoder.ApplyChanges(fps, TargetBitrate, frameWidth, frameHeight, configType);
                    keyFrameRequested = true;
                }

               
                if (!capture.Grab())
                {
                    CaptureFailed("Grab Operation Failed");
                    return;
                }

                //解码并返回抓取的视频帧 ImageReference
                capture.Retrieve(out var imref);
                if (imref == null || imref.Width == 0 || imref.Height == 0)
                {
                    CaptureFailed("Frame width is 0");
                    return;
                }

                try
                {
                    //H.264编码帧
                    EncodeFrame(imref);

                    //处理事件引用，更新本地帧图像
                    OnLocalImageAvailable?.Invoke(imref);

                    //帧计数
                    capturedFrameCnt++;
                }
                catch (Exception ex)
                { 
                    //ServiceHub.Instance.Log(" Capture encoding failed: ", ex.Message); 
                };

                //捕获间隔Ms
                int dt = (int)sw.ElapsedMilliseconds + remainderTime;
                while (dt < CaptureIntervalMs)
                {
                    Thread.Sleep(1);
                    if (currentState != State.CaptureActive)
                        return;

                    dt = (int)sw.ElapsedMilliseconds + remainderTime;
                }


                remainderTime = dt - CaptureIntervalMs;

                marshaller.Enqueue(Cap);;

            }
            catch (Exception e) { CaptureFailed(e.Message); return; }
        }
        else
        {
            return;
        }


        void CaptureFailed(string message)
        {
            if (currentState != State.Capturing)
                return;
            //ServiceHub.Instance.Log("Camera ERROR", message);
            CloseCamera();
            OnLocalImageAvailable?.Invoke(null);
        }
    }

    /// <summary>
    /// 编码帧
    /// </summary>
    /// <param name="frame"></param>
    private void EncodeFrame(ImageReference frame)
    {
        try
        {
            //是否关键帧，如果是则强制在下一次编码时使用即时解码器刷新帧内帧
            if (keyFrameRequested)
            {
                keyFrameRequested = false;
                if (transcoder.IsSetUp)
                    transcoder.ForceIntraFrame();
            }
            else
            {
                 ImageData data = new ImageData(ImageType.Bgr,frame.Width,frame.Height,frame.Stride,frame.DataStart);
                 if(transcoder.IsSetUp)
                    transcoder.Encode(data);
            }

        }
        catch (Exception ex) { 
            //ServiceHub.Instance.Log(" Capture encoding failed: ", ex.Message); 
        };
    }

    /// <summary>
    /// 解码帧
    /// </summary>
    /// <param name="img"></param>
    private void HandleDecodedFrame(ImageReference img)
    {
        lock (frameQLocker)
        {
            if (frameQueue.TryAdd(DateTime.Now, img))
                Interlocked.Increment(ref frameQueueCount);
        }
        consumerSignal.Set();
    }

    Random rng = new Random();

    /// <summary>
    /// 编码帧
    /// </summary>
    /// <param name="data"></param>
    /// <param name="length"></param>
    /// <param name="isKeyFrame"></param>
    private void HandleEncodedFrame(byte[] data, int length, bool isKeyFrame)
    {
        int howManyUnackedAllowed = (int)(AverageLatency / (1000 / Math.Max(1, actualFps))) + 2;
        int pending = Interlocked.CompareExchange(ref unAcked, 0, 0);
        if (pending > howManyUnackedAllowed)
        {
            RedcuceQuality(20 * (howManyUnackedAllowed - pending));
        }

        bytesSent += length;
        //if (rng.Next(0, 100) % 15 == 0)
        //    return;

        //var dat = ByteCopy.ToArray(data, 0, length);
        //Task.Delay(rng.Next(0, 500)).ContinueWith((x) =>
        //{
        //    OnBytesAvailable?.Invoke(dat, length);
        //    bytesSent += length;

        //});

        OnBytesAvailable?.Invoke(data, length, isKeyFrame);
    }

    /// <summary>
    /// 处理解码帧
    /// </summary>
    /// <param name="action"></param>
    /// <param name="byteLenght"></param>
    /// <param name="isKeyFrame"></param>
    private void HandleEncodedFrame2(Action<PooledMemoryStream> action, int byteLenght, bool isKeyFrame)
    {
       // Console.WriteLine($"[{DateTime.Now.Millisecond}]ENcoded: " + byteLenght);
        int howManyUnackedAllowed = (int)(AverageLatency / (1000 / Math.Max(1,actualFps))) + 2;
        int pending = Interlocked.CompareExchange(ref unAcked,0,0);
        if (pending > howManyUnackedAllowed)
        {
            RedcuceQuality(20 * (howManyUnackedAllowed - pending));
        }
        bytesSent += byteLenght;
        //Random rng = new Random(DateTime.Now.Millisecond);
        //if (rng.Next(0, 100) % 10 == 0)
        //    return;
        OnBytesAvailableAction?.Invoke(action,byteLenght,isKeyFrame);

    }

    /// <summary>
    /// 处理传入图像
    /// </summary>
    /// <param name="timeStamp"></param>
    /// <param name="payload"></param>
    /// <param name="payloadOffset"></param>
    /// <param name="payloadCount"></param>
    public unsafe void HandleIncomingImage(DateTime timeStamp, byte[] payload, int payloadOffset, int payloadCount)
    {
        bytesReceived += payloadCount;
        consumerSignal.Set();
        Interlocked.Increment(ref incomingFrameCount);
        transcoder.HandleIncomingFrame(timeStamp ,payload, payloadOffset, payloadCount);
    }
  
    /// <summary>
    /// 刷新缓冲
    /// </summary>
    public void FlushBuffers()
    {
        frameQueue?.Clear();

        transcoder?.FlushPool();

        AverageLatency = 0;
        avgDivider = 2;
        timeDict?.Clear();
        OnRemoteImageAvailable?.Invoke(null);
    }

    /// <summary>
    /// 硬重置
    /// </summary>
    public void HardReset()
    {
        var tra = transcoder;
        var newTra= SetupTranscoder(fps, TargetBitrate);
        Interlocked.Exchange(ref transcoder, newTra);
        tra.Dispose();
    }


    public void Pause() => paused = true;
    public void Resume() => paused = false;

    /// <summary>
    /// 处理确认
    /// </summary>
    /// <param name="message"></param>
    public void HandleAck(MessageEnvelope message)
    {
        double currentLatency = 0;
        if (timeDict.TryRemove(message.MessageId, out var timeStamp))
        {
            currentLatency = (DateTime.Now - timeStamp).TotalMilliseconds;
            if(timeStamp>latestAck)
                latestAck = timeStamp;
            Interlocked.Decrement(ref unAcked);
        }
        else
            return;

        if (AverageLatency == 0)
        {
            AverageLatency = currentLatency;
        }

        if (currentLatency <= AverageLatency)
            BumpQuality(2);
        else
        {
           //RedcuceQuality(100);
           // RedcuceQuality((int)Math.Abs(currentLatency - AverageLatency));
            //Console.WriteLine("recuced by latency");
        }
        AverageLatency = (avgDivider * AverageLatency + currentLatency) / (avgDivider + 1);
        avgDivider = Math.Min(600,avgDivider+1);

        // check for lost packets/jitter
        if (timeDict.Count > 0)
        {
            foreach (var item in timeDict)
            {
                List<Guid> toRemove = new List<Guid>();
                if ((DateTime.Now - item.Value).TotalMilliseconds > AverageLatency*1.3)
                {
                    // lost package , we need to remove it and reduce quality after
                    toRemove.Add(item.Key);
                }
                foreach (var key in toRemove)
                {
                    if(timeDict.TryRemove(key, out _))
                        Interlocked.Decrement(ref unAcked);
                  
                    RedcuceQuality(5);
                }
            }

        }

    }


    public void ImageDispatched(Guid messageId, DateTime timeStamp)
    {
        if(timeDict.TryAdd(messageId, timeStamp))
            Interlocked.Increment(ref unAcked);
    }
   
    private void RedcuceQuality(int factor=1)
    {
        if (factor < 0) return;
        if (!EnableCongestionAvoidance) return;
        if(currentBps == 0)
        {
            currentBps = TargetBitrate;
        }
        var currentBps_ =currentBps - ( currentBps * factor / 100);
        currentBps = Math.Max(currentBps_, minBps);
        transcoder?.SetTargetBps(currentBps);
       
    }

    private void BumpQuality(int factor=1)
    {
        if (currentBps == 0)
        {
            currentBps = TargetBitrate;
        }
        var currentBps_ = currentBps+ ((currentBps * factor) / 100);
        currentBps = Math.Min(TargetBitrate, currentBps_);
        transcoder?.SetTargetBps(currentBps);
    }

    public void ApplySettings(int camFrameWidth, int camFrameHeight,int targetBps, int idrInterval, int camIndex, int minBPS)
    {
        if (camFrameHeight == 0 || camFrameWidth == 0)
            return;

        frameHeight = camFrameHeight;
        frameWidth = camFrameWidth;
        TargetBitrate = targetBps*1000;
        camIdx = camIndex;
        adjustCamSize = true;
        periodicKeyFrameInterval = idrInterval;
        transcoder?.SetKeyFrameInterval(periodicKeyFrameInterval);
        transcoder?.SetTargetBps(TargetBitrate);
        this.minBps = minBPS * 1000;

        AverageLatency = 0; avgDivider = 0;
    }

    public void ForceKeyFrame()
    {
        keyFrameRequested = true;
        //ServiceHub.Instance.Log("Info","Forcing Key Frame");                    
        RedcuceQuality(20);
    }
    public VCStatistics GetStatistics()
    {
        incomingFrameRate = Interlocked.Exchange(ref incomingFrameCount, 0);

        var sendRate = (float)bytesSent / 1000;
        bytesSent = 0;
        var receiveRate = (float)bytesReceived / 1000;
        bytesReceived = 0;

        outgoingFrameRate = capturedFrameCnt;
        if (outgoingFrameRate != actualFps)
            transcoder?.SetTargetFps(outgoingFrameRate + 5);
        actualFps = outgoingFrameRate;

        capturedFrameCnt = 0;

        var st = new VCStatistics
        {
            OutgoingFrameRate = outgoingFrameRate,
            IncomingFrameRate = incomingFrameRate,
            TransferRate = sendRate,
            AverageLatency = AverageLatency,
            ReceiveRate = receiveRate,
            CurrentMaxBitRate = currentBps == 0 ? TargetBitrate : currentBps,
           
        };

        return st;
    }

    public void HandleLtrRecovery(byte[] payload, int payloadOffset, int payloadCount)
    {
       transcoder.SetLTRRecoverRequest(payload, payloadOffset, payloadCount);
       RedcuceQuality(10);

    }

    public void HandleMarkingFeedback(byte[] payload, int payloadOffset, int payloadCount)
    {
        transcoder.SetMarkingFeedback(payload, payloadOffset, payloadCount);
    }

   
    public void Dispose()
    {
        Execute(StateAction.Dispose);
    }

    int isDisposed = 0;
    private void Dispose_()
    {
        if(Interlocked.CompareExchange(ref isDisposed, 1, 0) == 0)
        {
            try
            {
                capture?.Release();

            }
            catch
            {

            }
            try
            {
                capture?.Dispose();

            }
            catch
            {

            }

        }
    }
}


public struct VCStatistics
{
    public float IncomingFrameRate;
    public float OutgoingFrameRate;
    public float TransferRate;
    public float ReceiveRate;
    public double AverageLatency;
    public int CurrentMaxBitRate;
  
    public override bool Equals(object obj) => obj is VCStatistics other && this.Equals(other);

    public bool Equals(VCStatistics p) => IncomingFrameRate == p.IncomingFrameRate
        && OutgoingFrameRate == p.OutgoingFrameRate
        && TransferRate == p.TransferRate
        && ReceiveRate == p.ReceiveRate
        && AverageLatency == p.AverageLatency
        && CurrentMaxBitRate == p.CurrentMaxBitRate;

    public override int GetHashCode() => (IncomingFrameRate, OutgoingFrameRate, TransferRate, AverageLatency,CurrentMaxBitRate).GetHashCode();

    public static bool operator ==(VCStatistics lhs, VCStatistics rhs) => lhs.Equals(rhs);

    public static bool operator !=(VCStatistics lhs, VCStatistics rhs) => !(lhs == rhs);
}
