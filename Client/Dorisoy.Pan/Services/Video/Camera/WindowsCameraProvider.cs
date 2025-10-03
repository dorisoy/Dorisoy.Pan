using OpenCvSharp;
using Mat = OpenCvSharp.Mat;
using VideoCapture = OpenCvSharp.VideoCapture;

namespace Dorisoy.PanClient.Services.Video.Camera;

/// <summary>
/// 用于表示一个摄像头提供器实现
/// </summary>
public class WindowsCameraProvider : ICameraProvider
{
    /// <summary>
    /// 提供了一个线程安全的(Mat)集合
    /// Mat 表示了一个多维的矩阵，用于存储图像或矩阵的数据。它是OpenCV库中Mat类的C#封装版本，提供了对图像像素及像素操作的支持
    /// </summary>
    private ConcurrentBag<Mat> mats = [];

    /// <summary>
    /// 提供了一个线程安全的ImageReference集合
    /// </summary>
    private ConcurrentBag<ImageReference> imrefs = [];

    /// <summary>
    /// 视频捕获
    /// </summary>
    private readonly VideoCapture capture;

    private int frameWidth => capture.FrameWidth;
    private int frameHeight => capture.FrameHeight;

    public WindowsCameraProvider(int camIdx)
    {
        //VideoCapture.API.DShow  OpenCvSharp
        capture = new VideoCapture(camIdx, VideoCaptureAPIs.MSMF);
    }

    public int FrameWidth { get => frameWidth; set { capture.FrameWidth = value; } }
    public int FrameHeight { get => frameHeight; set => capture.FrameHeight = value; }

    /// <summary>
    /// 打开摄像头
    /// </summary>
    /// <param name="camIdx"></param>
    public void Open(int camIdx)
    {
        capture.Open(camIdx);
    }

    /// <summary>
    /// 是否已经打开
    /// </summary>
    /// <returns></returns>
    public bool IsOpened()
    {
        return capture.IsOpened();
    }

    /// <summary>
    /// 关闭视频文件、捕获设备
    /// </summary>
    public void Release()
    {
        capture?.Release();
    }

    /// <summary>
    /// 释放非托管
    /// </summary>
    public void Dispose()
    {
        capture.Dispose();
    }

    /// <summary>
    /// 从视频文件或捕获设备抓取下一帧
    /// </summary>
    /// <returns></returns>
    public bool Grab()
    {
        return capture.Grab();
    }

    /// <summary>
    /// 解码并返回抓取的视频帧
    /// </summary>
    /// <param name="im"></param>
    /// <returns></returns>
    public bool Retrieve(out ImageReference im)
    {
        mats.TryTake(out var frame);
        imrefs.TryTake(out im);

        frame ??= new Mat();

        if (frame.Width != capture.FrameWidth || frame.Height != capture.FrameHeight)
        {
            frame.Dispose();
            frame = new Mat();
        }

        var res = capture.Retrieve(frame);
        if (res == false)
        {
            return false;
        }

        if (im == null)
        {
            im = ImageReference.FromMat(frame, ReturnMat);
        }
        else
        {
            im.Update(frame);
        }
        return res;

    }

    private void ReturnMat(ImageReference reference)
    {
        imrefs.Add(reference);
        mats.Add((Mat)reference.underlyingData);
    }
}
