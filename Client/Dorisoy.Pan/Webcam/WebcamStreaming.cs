using Dorisoy.PanClient.Models;
using static Emgu.CV.VideoCapture;
using Image = Avalonia.Controls.Image;

namespace Dorisoy.PanClient.Webcam;

[SupportedOSPlatform("windows")]
public class WebcamPlayReadEventArgs : EventArgs
{
    public WebcamPlayReadEventArgs(Mat mat) { Mat = mat; }
    public Mat Mat { get; }
}


[SupportedOSPlatform("windows")]
public sealed class WebcamStreaming : IDisposable
{
    private Task _previewTask;

    private CancellationTokenSource _cancellationTokenSource;
    private readonly Image _imageControlForRendering;
    private readonly int _frameWidth = 0;
    private readonly int _frameHeight = 0;

    public int CameraDeviceId { get; private set; }
    public byte[] LastPngFrame { get; private set; }
    public bool FlipHorizontally { get; set; }
    public bool IsPlaying { get; private set; }

    public event EventHandler<WebcamPlayReadEventArgs> Playing;

    public WebcamStreaming(
        Image imageControlForRendering,
        int frameWidth,
        int frameHeight)
    {
        _imageControlForRendering = imageControlForRendering;
        _frameWidth = frameWidth;
        _frameHeight = frameHeight;
    }

    public async Task Start()
    {
        if (_previewTask != null && !_previewTask.IsCompleted)
            return;

        var initializationSemaphore = new SemaphoreSlim(0, 1);

        _cancellationTokenSource = new CancellationTokenSource();
        _previewTask = Task.Run(async () =>
        {
            try
            {
                //该对象的创建和处理应该在同一个线程中完成，因为如果不这样做，它将抛出disconnectedContext异常
                var videoCapture = new VideoCapture(CameraDeviceId, API.DShow);

                if (!videoCapture.IsOpened)
                {
                    throw new ApplicationException("Cannot connect to camera");
                }

                //capture.set(CV_CAP_PROP_FRAME_WIDTH, 1080);      //设置宽度
                //capture.set(CV_CAP_PROP_FRAME_HEIGHT, 720); 		//设置高度

                //capture.set(CAP_PROP_FRAME_WIDTH, 640);
                //capture.set(CAP_PROP_FRAME_HEIGHT, 480);

                //修改分辨率
                videoCapture.Set(Emgu.CV.CvEnum.CapProp.FrameWidth, 1920);
                videoCapture.Set(Emgu.CV.CvEnum.CapProp.FrameHeight, 1080);


                /*设置摄像头参数 不要随意修改
               capture.set(CV_CAP_PROP_FRAME_WIDTH, 1080);//宽度
               capture.set(CV_CAP_PROP_FRAME_HEIGHT, 960);//高度
               capture.set(CV_CAP_PROP_FPS, 30);//帧数
               capture.set(CV_CAP_PROP_BRIGHTNESS, 1);//亮度 1
               capture.set(CV_CAP_PROP_CONTRAST,40);//对比度 40
               capture.set(CV_CAP_PROP_SATURATION, 50);//饱和度 50
               capture.set(CV_CAP_PROP_HUE, 50);//色调 50
               capture.set(CV_CAP_PROP_EXPOSURE, 50);//曝光 50
               */

                //videoCapture.Set(VideoCaptureProperties.Fps, 10);
                //videoCapture.Set(VideoCaptureProperties.Brightness, 50);

                //对每一帧进行处理
                using (var frame = new Mat())
                {
                    while (!_cancellationTokenSource.IsCancellationRequested)
                    {
                        videoCapture.Read(frame);

                        if (!frame.IsEmpty)
                        {

                            // 释放第一个非空帧上的锁定
                            if (initializationSemaphore != null)
                                initializationSemaphore.Release();

                            //FlipMode.X：沿x - 轴翻转
                            //FlipMode.Y：沿y - 轴翻转
                            //FlipMode.X：沿xy - 轴翻转

                            //_lastFrame = FlipHorizontally 
                            //    ? BitmapConverter.ToBitmap(frame.Flip(FlipMode.XY))
                            //    : BitmapConverter.ToBitmap(frame);

                            //var lastFrameBitmapImage = _lastFrame.ConvertToAvaloniaBitmap();
                            //lastFrameBitmapImage.Freeze();
                            //_imageControlForRendering.Dispatcher.Invoke(() => _imageControlForRendering.Source = lastFrameBitmapImage);

                            // 转换格式
                            using Image<Rgb, byte> img_trans = frame.ToImage<Rgb, byte>();
                            // JPEG压缩
                            byte[] bytes = img_trans.ToJpegData(50);

                            using var ms = new MemoryStream(bytes);
                            var bitmap = new Avalonia.Media.Imaging.Bitmap(ms);
                            if (bitmap != null)
                            {
                                if (Playing != null)
                                    Playing.Invoke(this, new WebcamPlayReadEventArgs(frame));

                                SetPlaying(true);

                                await Dispatcher.UIThread.InvokeAsync(() =>
                                {
                                    _imageControlForRendering.Source = bitmap;

                                }, DispatcherPriority.Background);
                            }
                        }

                        // 30 FPS
                        await Task.Delay(8);
                    }
                }

                videoCapture?.Dispose();
            }
            finally
            {
                if (initializationSemaphore != null)
                    initializationSemaphore.Release();
            }

        }, _cancellationTokenSource.Token);

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
    public void SetPlaying(bool playing)
    {
        this.IsPlaying = playing;
    }
    public void SetDevice(CameraDevice device)
    {
        if (device != null)
            this.CameraDeviceId = device.OpenCvId;
    }
    public void Dispose()
    {
        _cancellationTokenSource?.Cancel();
        SetPlaying(false);
    }
}
