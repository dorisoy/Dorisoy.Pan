using static Emgu.CV.VideoCapture;
namespace Dorisoy.PanClient.Webcam;

[SupportedOSPlatform("windows")]
public sealed class WebcamStreamingPlayer : IDisposable
{
    private Task _previewTask;
    private CancellationTokenSource _cancellationTokenSource;
    private int _frameWidth = 1920;
    private int _frameHeight = 1080;

    public int CameraDeviceId { get; private set; }
    public byte[] LastPngFrame { get; private set; }
    public bool FlipHorizontally { get; set; }
    public bool IsPlaying { get; private set; } = false;

    /// <summary>
    /// 播放事件
    /// </summary>
    public event EventHandler<WebcamPlayReadEventArgs> Playing;

    public WebcamStreamingPlayer(int frameWidth, int frameHeight)
    {
        _frameWidth = frameWidth;
        _frameHeight = frameHeight;
    }

    /// <summary>
    /// 启动摄像头
    /// </summary>
    /// <param name="openCvId"></param>
    /// <param name="frameWidth"></param>
    /// <param name="frameHeight"></param>
    /// <returns></returns>
    /// <exception cref="ApplicationException"></exception>
    public async Task Start(int openCvId, int frameWidth, int frameHeight)
    {
        _frameWidth = frameWidth;
        _frameHeight = frameWidth;

        if (_previewTask != null && !_previewTask.IsCompleted)
            return;

        //var initializationSemaphore = new SemaphoreSlim(0, 1);

        _cancellationTokenSource = new CancellationTokenSource();
        _previewTask = Task.Run(() =>
        {
            try
            {
                //该对象的创建和处理应该在同一个线程中完成，因为如果不这样做，它将抛出disconnectedContext异常
                using var videoCapture = new VideoCapture(openCvId, API.DShow);
                if (!videoCapture.IsOpened)
                {
                    throw new ApplicationException("无法连接到摄像头");
                }

                //修改分辨率
                videoCapture.Set(Emgu.CV.CvEnum.CapProp.FrameWidth, _frameWidth);
                videoCapture.Set(Emgu.CV.CvEnum.CapProp.FrameHeight, _frameHeight);
                videoCapture.Set(Emgu.CV.CvEnum.CapProp.Fps, 30);

                //对每一帧进行处理
                using var frame = new Mat();
                while (!_cancellationTokenSource.IsCancellationRequested)
                {
                    videoCapture.Read(frame);
                    if (!frame.IsEmpty)
                    {
                        //// 释放第一个非空帧上的锁定
                        //if (initializationSemaphore != null)
                        //    initializationSemaphore.Release();

                        Playing?.Invoke(this, new WebcamPlayReadEventArgs(frame));

                        SetPlaying(true);
                    }
                }
            }
            finally
            {
                //initializationSemaphore?.Release();
            }

        }, _cancellationTokenSource.Token);

        // 异步初始化可以在不冻结GUI的情况下显示动画加载程序
        // 另一种选择是长轮询。（while！variable）等待任务。延迟
        //await initializationSemaphore.WaitAsync();
        // initializationSemaphore?.Dispose();
        //initializationSemaphore = null;

        if (_previewTask.IsFaulted)
        {
            // 异常退出
            await _previewTask;
        }
    }

    /// <summary>
    /// 停止摄像头
    /// </summary>
    /// <returns></returns>
    public async Task Stop()
    {
        // 如果在停止之前调用“Dispose”
        if (_cancellationTokenSource == null || _cancellationTokenSource.IsCancellationRequested)
            return;

        if (!_previewTask.IsCompleted)
        {
            _cancellationTokenSource.Cancel();
            await _previewTask;
        }

        SetPlaying(false);
    }

    /// <summary>
    /// 更新播放状态
    /// </summary>
    /// <param name="playing"></param>
    public void SetPlaying(bool playing)
    {
        this.IsPlaying = playing;
    }

    public void Dispose()
    {
        _cancellationTokenSource?.Cancel();
        SetPlaying(false);
    }
}
