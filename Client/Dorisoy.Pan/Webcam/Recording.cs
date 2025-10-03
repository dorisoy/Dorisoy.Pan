namespace Dorisoy.PanClient.Webcam;

using System;
//using System.Diagnostics;
using System.Drawing;
//using System.Linq;
//using System.Net;
//using System.Text;
using System.Threading;
using System.Timers;
//using DirectShowLib;
//using DirectShowLib;
using Emgu.CV;
//using Emgu.CV.Structure;
using Serilog;
using Dorisoy.PanClient.Models;
using TorchSocket.Core;
//using TorchSocket.Sockets;
using static Emgu.CV.VideoCapture;

public class RecorderEventArgs : EventArgs
{
    public RecorderEventArgs(TimeSpan timeSpan, Tuple<string, string> path)
    {
        TimeCounter = timeSpan;
        Path = path;
    }

    public TimeSpan TimeCounter { get; }
    public Tuple<string, string> Path { get; }
    public string FormatString
    {
        get
        {
            return string.Format("{0:00}:{1:00}:{2:00}:{3:000}", TimeCounter.Hours, TimeCounter.Minutes, TimeCounter.Seconds, TimeCounter.Milliseconds);
        }
    }
}

/// <summary>
/// 用于视屏录制
/// </summary>
public class Recorder : IDisposable
{
    private readonly API _videoCaptureApi = API.DShow;
    private VideoCapture _videoCapture;

    /// <summary>
    /// 视频写入器
    /// </summary>
    private VideoWriter _videoWriter;
    private Mat _capturedFrame = new();


    private CancellationTokenSource _ctsReader;
    private CancellationTokenSource _ctsWriter;

    /// <summary>
    /// 录制时间统计
    /// </summary>
    private System.Timers.Timer _timer;
    private System.DateTime _timeNow = new DateTime();
    private Tuple<string, string> _savePath;
    private TimeSpan _timeCount;

    /// <summary>
    /// 分段记录时长(分钟)_frameRate
    /// </summary>
    private int _segmentDuration { get; set; }

    private Bitmap _lastFrame;

    /// <summary>
    /// 镜像
    /// </summary>
    public bool FlipHorizontally { get; set; }

    /// <summary>
    /// 播放事件
    /// </summary>
    public event EventHandler<WebcamPlayReadEventArgs> Playing;

    /// <summary>
    /// 录制计时
    /// </summary>
    public event EventHandler<RecorderEventArgs> Timing;

    /// <summary>
    /// 录制完成通知
    /// </summary>
    public event EventHandler<RecorderEventArgs> Recorded;

    /// <summary>
    /// 是否有效
    /// </summary>
    public bool IsVideoCaptureValid => _videoCapture is not null && _videoCapture.IsOpened;


    public Recorder(CameraDevice device, int frameWidth, int frameHeight)
    {
        if (_timer == null)
        {
            _timer = new() { Interval = 1 };
            _timer.Elapsed += timer_Elapsed;
        }

        //1900 x 1080
        _videoCapture = new VideoCapture(device.OpenCvId, _videoCaptureApi);
        _videoCapture.Set(Emgu.CV.CvEnum.CapProp.CodecPixelFormat, VideoWriter.Fourcc('M', 'J', 'P', 'G'));

        //_videoCapture.Set(Emgu.CV.CvEnum.CapProp.CodecPixelFormat, VideoWriter.Fourcc('H', '2', '6', '4'));
        //_videoCapture.Set(Emgu.CV.CvEnum.CapProp.FrameWidth, 1280);
        //_videoCapture.Set(Emgu.CV.CvEnum.CapProp.FrameHeight, 720);

        _videoCapture.Set(Emgu.CV.CvEnum.CapProp.FrameWidth, 1280);
        _videoCapture.Set(Emgu.CV.CvEnum.CapProp.FrameHeight, 720);

        _videoCapture.Set(Emgu.CV.CvEnum.CapProp.Fps, 30);
        _videoCapture.Set(Emgu.CV.CvEnum.CapProp.Buffersize, 1);
    }


    public void Dispose()
    {
        GC.SuppressFinalize(this);
        Dispose(true);
    }

    ~Recorder()
    {
        Dispose(false);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            StopRecording();

            if (_timer != null)
                _timer.Elapsed -= timer_Elapsed;

            _videoWriter?.SafeDispose();
            _capturedFrame?.SafeDispose();
            _videoCapture?.Stop();
            _videoCapture?.SafeDispose();
            _videoCapture = null;
        }
    }

    #region 录制

    /// <summary>
    /// 开始录制
    /// </summary>
    /// <param name="path">保存路径</param>
    /// <param name="segmentDuration">分段时长</param>
    public void StartRecording(Tuple<string, string> path, int segmentDuration = 10)
    {
        //添加摄像头换面帧到记录线程 
        _ctsWriter = new CancellationTokenSource();

        _timeNow = DateTime.Now;
        _segmentDuration = segmentDuration;
        _savePath = path;

        if (IsVideoCaptureValid)
        {
            //声明视频大小
            var size = new Size(_videoCapture.Width, _videoCapture.Height);
            var fps = _videoCapture.Get(Emgu.CV.CvEnum.CapProp.Fps);

            // 创建视频编码器 FourCC.XVID,
            _videoWriter = new VideoWriter(path.Item1, VideoWriter.Fourcc('X', 'V', 'I', 'D'), fps, size, true);
            //添加录制任务
            Task.Run(() => AddCameraFrameToRecordingThread(fps), _ctsWriter.Token);
            //开始计时
            _timer?.Start();
        }
    }

    /// <summary>
    /// 添加摄像头换面帧到记录线程
    /// </summary>
    private void AddCameraFrameToRecordingThread(double? fps)
    {
        var waitTimeBetweenFrames = 8;
        var lastWrite = DateTime.Now;

        while (!_ctsWriter.IsCancellationRequested)
        {
            try
            {
                if (DateTime.Now.Subtract(lastWrite).TotalMilliseconds < waitTimeBetweenFrames)
                    continue;

                lastWrite = DateTime.Now;

                //把_capturedFrame 帧写入到AVI视频文件中
                if (_capturedFrame != null && !_capturedFrame.IsEmpty)
                {
                    if (_videoWriter.IsOpened)
                    {
                        _videoWriter.Write(_capturedFrame);
                    }
                }

                //System.AccessViolationException:“Attempted to read or write protected memory. This is often an indication that other memory is corrupt.”

                //System.AccessViolationException:“Attempted to read or write protected memory. This is often an indication that other memory is corrupt.”
            }
            catch (Exception ex) 
            {
                Log.Error(ex.Message);
            }
        }
    }

    /// <summary>
    /// 停止录制
    /// </summary>
    public void StopRecording()
    {
        try
        {
            //停止计时
            _timer?.Stop();
            _ctsWriter?.Cancel();
            _ctsReader?.Cancel();

            //_videoWriter?.SafeDispose();
            //_videoWriter = null;

            //_videoWriter 释放后触发Recorded 事件
            if (_savePath != null)
                Recorded?.Invoke(this, new RecorderEventArgs(_timeCount, _savePath));

        }
        catch { }
    }


    /// <summary>
    /// 视频录制计时器
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void timer_Elapsed(object sender, ElapsedEventArgs e)
    {
        _timeCount = DateTime.Now - _timeNow;
        if (_savePath != null)
            Timing?.Invoke(this, new RecorderEventArgs(_timeCount, _savePath));
    }
    #endregion


    //private Task _senderThread;
    //private CancellationTokenSource _cancellationTokenSource;

    ///// <summary>
    ///// 开启UDP发送任务
    ///// </summary>
    //public async Task StartUDPSend(UdpSession udpSession, IPEndPoint iPEndPoint)
    //{
    //    if (_senderThread != null && !_senderThread.IsCompleted)
    //        return;

    //    if (IsVideoCaptureValid)
    //    {
    //        ////视频写入线程 
    //        _cancellationTokenSource = new CancellationTokenSource();
    //        _senderThread = Task.Run(() => SendUDPThread(udpSession, 30, iPEndPoint), _cancellationTokenSource.Token);
    //        if (_senderThread.IsFaulted)
    //        {
    //            // 异常退出
    //            await _senderThread;
    //        }
    //    }
    //}

    ///// <summary>
    ///// 停止UDP发送任务
    ///// </summary>
    ///// <returns></returns>
    //public async Task StopUDPSend()
    //{
    //    // 如果在停止之前调用“Dispose”
    //    if (_cancellationTokenSource == null || _cancellationTokenSource.IsCancellationRequested)
    //        return;

    //    if (!_senderThread.IsCompleted)
    //    {
    //        _cancellationTokenSource.Cancel();

    //        // 等待
    //        await _senderThread;
    //    }
    //}

    ///// <summary>
    ///// 发送任务作业
    ///// </summary>
    ///// <param name="udpSession"></param>
    ///// <param name="fps"></param>
    //private void SendUDPThread(UdpSession udpSession, double? fps, IPEndPoint remotePoint)
    //{
    //    var waitTimeBetweenFrames = 8;
    //    var lastWrite = DateTime.Now;

    //    Stopwatch stopWatch = new Stopwatch();

    //    while (!_cancellationTokenSource.IsCancellationRequested)
    //    {
    //        try
    //        {
    //            if (DateTime.Now.Subtract(lastWrite).TotalMilliseconds < waitTimeBetweenFrames)
    //                continue;

    //            lastWrite = DateTime.Now;

    //            //System.AccessViolationException:“Attempted to read or write protected memory. This is often an indication that other memory is corrupt.”

    //            if (_capturedFrame != null && !_capturedFrame.IsEmpty)
    //            {
    //                // 计算当前帧的长度
    //                long totalLngth = _capturedFrame.Total * _capturedFrame.ElementSize;

    //                Debug.WriteLine($"压缩前：{totalLngth}");


    //                // 转换格式
    //                using Image<Rgb, byte> img_trans = _capturedFrame.ToImage<Rgb, byte>();

    //                stopWatch.Stop();
    //                Debug.WriteLine($" 转换格式执行时间：{stopWatch.ElapsedMilliseconds} 毫秒");
    //                stopWatch.Reset();
    //                //-----------------------------

    //                stopWatch.Start();

    //                // JPEG压缩
    //                byte[] bytes = img_trans.ToJpegData(85);

    //                stopWatch.Stop();
    //                Debug.WriteLine($" JPEG压缩执行时间：{stopWatch.ElapsedMilliseconds} 毫秒");
    //                stopWatch.Reset();
    //                //-----------------------------

    //                stopWatch.Start();

    //                if (udpSession != null)
    //                {
    //                    udpSession.Send(remotePoint, bytes);
    //                }

    //                stopWatch.Stop();
    //                Debug.WriteLine($" 发送UDP执行时间：{stopWatch.ElapsedMilliseconds} 毫秒");
    //                stopWatch.Reset();

    //            }
    //        }
    //        catch { }
    //    }
    //}



    /// <summary>
    /// 启动视频帧环出线程
    /// </summary>
    public void StartStreaming()
    {
        //启动视频帧环出线程
        _ctsReader = new CancellationTokenSource();
        if (IsVideoCaptureValid)
        {
            Task.Run(CaptureFrameLoop, _ctsReader.Token);
        }
    }

    public void StopStreaming()
    {
        _ctsReader?.Cancel();
        _ctsWriter?.Cancel();
    }

    public bool IsLooping { get; set; } = false;

    /// <summary>
    /// 循环捕获帧
    /// </summary>
    private void CaptureFrameLoop()
    {
        //阻止当前线程，直到设置了当前
        while (!_ctsReader.IsCancellationRequested)
        {
            try
            {
                if (_capturedFrame == null)
                    break;

                if (_videoCapture != null && _videoCapture.Ptr != IntPtr.Zero)
                {
                    //读取视频帧
                    if (_videoCapture.Read(_capturedFrame))
                    {
                        if (!_capturedFrame.IsEmpty)
                        {
                            Playing?.Invoke(this, new WebcamPlayReadEventArgs(_capturedFrame));
                        }
                    }
                }
            }
            catch (System.AccessViolationException) { }
        }

        IsLooping = false;
    }
}
