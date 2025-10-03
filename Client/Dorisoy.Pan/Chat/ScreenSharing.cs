using System.Drawing.Imaging;
using Dorisoy.PanClient.Services.ScreenShare;
using Image = System.Drawing.Image;


namespace Dorisoy.PanClient.Chat;

/// <summary>
/// 用于表示屏幕共享
/// </summary>
public class ScreenSharing : DataSharing
{
    private CancellationTokenSource _cts;
    private PrintScreen _printScreen;
    private VoiceChatModel _chatModel;
    //private ScreenShareHandlerH264 _screenShareHandlerH264;

    private int secondaryCanvasBusy = 0;
    private int primaryCanvasBusy = 0;

    public unsafe ScreenSharing(VoiceChatModel model) : base(model)
    {
        _chatModel = model;
        LineIndex = 3;

        _printScreen = new PrintScreen();
       // _screenShareHandlerH264 = new ScreenShareHandlerH264();

        //本地屏幕图像
       
    }

    public override void BeginSend()
    {
        base.BeginSend();
        _cts = new CancellationTokenSource();
        StartCapture(_cts.Token);
        //_screenShareHandlerH264.StartCapture();
    }

    public override void EndSend()
    {
        base.EndSend();
        _cts?.Cancel();
        //_screenShareHandlerH264.StopCapture();
    }

    private UdpSession udpSenders;
    protected override void Receive()
    {
        try
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
                        
                        //_screenShareHandlerH264.HandleNetworkImageBytes(DateTime.Now, e.ByteBlock,0, e.ByteBlock.Len);
                    }
                    catch (Exception)
                    {
                    }
                    return Task.CompletedTask;
                };
            }
        }
        catch (Exception)
        {
        }
    }

    //private Thread m_thread;
    private const int targetFps = 40;
    private const int frameDuration = 1000 / targetFps;// 计算每帧需要的时长（毫秒）

    public void StartCapture(CancellationToken token)
    {
        Task.Run(() =>
        {
            Stopwatch stopwatch = Stopwatch.StartNew();
            long lastFrameTime = stopwatch.ElapsedMilliseconds;

            while (!token.IsCancellationRequested)
            {
                try
                {
                    long currentFrameTime = stopwatch.ElapsedMilliseconds;
                    long elapsedTime = currentFrameTime - lastFrameTime;

                    if (elapsedTime < frameDuration)
                    {
                        // 如果距离上一帧的时间小于目标帧时长，进行等待
                        Thread.Sleep(frameDuration - (int)elapsedTime);
                    }


                    var img = _printScreen.CaptureScreen();
                    if (img != null)
                    {
                        var byteArray = ImageToByte(img);
                        if (byteArray != null)
                        {
                            _chatModel.mvvm.LocalFrame = BitmapExtensions.ToBitmap(byteArray);

                            //using var bb = new ByteBlock(byteArray);
                            //BdtpClient.Send(bb, LineIndex);
                        }

                    }

                    // 更新上一帧时间戳
                    lastFrameTime = stopwatch.ElapsedMilliseconds;
                }
                catch (Exception ex)
                { }
            }
        }, token);
    }


    private byte[] ImageToByte(Image img)
    {

        if (img == null)
            return Array.Empty<byte>();


        // 设置保存的 JPEG 质量
        EncoderParameters encoderParameters = new EncoderParameters(1);
        // 50 是质量因子（0-100）
        encoderParameters.Param[0] = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, 50L);
        // 获取 JPEG 编码器
        var jpgEncoder = GetEncoder(ImageFormat.Jpeg);

        using var ms = new MemoryStream();
        img.Save(ms, jpgEncoder, encoderParameters);
        var picture = new byte[ms.Length];
        picture = ms.GetBuffer();
        return picture;
    
    }


    static ImageCodecInfo GetEncoder(ImageFormat format)
    {
        var codecs = ImageCodecInfo.GetImageDecoders();
        foreach (var codec in codecs)
        {
            if (codec.FormatID == format.Guid)
            {
                return codec;
            }
        }
        return null;
    }

    //private Image ByteToImage(byte[] btImage)
    //{
    //    if (btImage.Length == 0)
    //        return null;
    //    var ms = new System.IO.MemoryStream(btImage);
    //    var image = System.Drawing.Image.FromStream(ms);
    //    return image;
    //}
}




public class PrintScreen
{
    /// <summary>
    /// 创建包含整个桌面的屏幕截图的图像对象
    /// </summary>
    /// <returns></returns>
    public Image CaptureScreen()
    {
        return CaptureWindow(User32.GetDesktopWindow());
    }

    /// <summary>
    /// 创建包含特定窗口的屏幕截图的图像对象
    /// </summary>
    /// <param name="handle">句柄（在windows窗体中，这是通过Handle属性获得的）</param>
    /// <returns></returns>
    public Image CaptureWindow(IntPtr handle)
    {
        // get te hDC of the target window
        IntPtr hdcSrc = User32.GetWindowDC(handle);
        // get the size
        User32.RECT windowRect = new User32.RECT();
        User32.GetWindowRect(handle, ref windowRect);
        int width = windowRect.right - windowRect.left;
        int height = windowRect.bottom - windowRect.top;
        // create a device context we can copy to
        IntPtr hdcDest = GDI32.CreateCompatibleDC(hdcSrc);
        // create a bitmap we can copy it to,
        // using GetDeviceCaps to get the width/height
        IntPtr hBitmap = GDI32.CreateCompatibleBitmap(hdcSrc, width, height);
        // select the bitmap object
        IntPtr hOld = GDI32.SelectObject(hdcDest, hBitmap);
        // bitblt over
        GDI32.BitBlt(hdcDest, 0, 0, width, height, hdcSrc, 0, 0, GDI32.SRCCOPY);
        // restore selection
        GDI32.SelectObject(hdcDest, hOld);
        // clean up
        GDI32.DeleteDC(hdcDest);
        User32.ReleaseDC(handle, hdcSrc);

        // get a .NET image object for it
        Image img = Image.FromHbitmap(hBitmap);

        // free up the Bitmap object
        GDI32.DeleteObject(hBitmap);

        return img;
    }


    [DllImport("user32.dll")]
    private static extern bool GetCursorInfo(out CURSORINFO pci);

    private const Int32 CURSOR_SHOWING = 0x00000001;

    [StructLayout(LayoutKind.Sequential)]
    private struct POINT
    {
        public Int32 x;
        public Int32 y;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct CURSORINFO
    {
        public Int32 cbSize;
        public Int32 flags;
        public IntPtr hCursor;
        public POINT ptScreenPos;
    }

    /// <summary>
    /// 捕获特定窗口的屏幕截图，并将其保存到文件中
    /// </summary>
    /// <param name="handle"></param>
    /// <param name="filename"></param>
    /// <param name="format"></param>
    public void CaptureWindowToFile(IntPtr handle, string filename, ImageFormat format)
    {
        Image img = CaptureWindow(handle);
        img.Save(filename, format);
    }

    /// <summary>
    /// 捕获整个桌面的屏幕截图，并将其保存到文件中
    /// </summary>
    /// <param name="filename"></param>
    /// <param name="format"></param>
    public void CaptureScreenToFile(string filename, ImageFormat format)
    {
        CaptureScreen().Save(filename, format);
    }

    /// <summary>
    /// 包含Gdi32 API函数的帮助程序类
    /// </summary>
    private class GDI32
    {
        //BitBlt-dwRop参数
        public const int SRCCOPY = 0x00CC0020; 

        [DllImport("gdi32.dll")]
        public static extern bool BitBlt(IntPtr hObject, int nXDest, int nYDest,
            int nWidth, int nHeight, IntPtr hObjectSource,
            int nXSrc, int nYSrc, int dwRop);

        [DllImport("gdi32.dll")]
        public static extern IntPtr CreateCompatibleBitmap(IntPtr hDC, int nWidth,
            int nHeight);

        [DllImport("gdi32.dll")]
        public static extern IntPtr CreateCompatibleDC(IntPtr hDC);

        [DllImport("gdi32.dll")]
        public static extern bool DeleteDC(IntPtr hDC);

        [DllImport("gdi32.dll")]
        public static extern bool DeleteObject(IntPtr hObject);

        [DllImport("gdi32.dll")]
        public static extern IntPtr SelectObject(IntPtr hDC, IntPtr hObject);
    }

    /// <summary>
    /// 包含User32 API函数的帮助程序类
    /// </summary>
    private class User32
    {
        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int left;
            public int top;
            public int right;
            public int bottom;
        }

        [DllImport("user32.dll")]
        public static extern IntPtr GetDesktopWindow();

        [DllImport("user32.dll")]
        public static extern IntPtr GetWindowDC(IntPtr hWnd);

        [DllImport("user32.dll")]
        public static extern IntPtr ReleaseDC(IntPtr hWnd, IntPtr hDC);

        [DllImport("user32.dll")]
        public static extern IntPtr GetWindowRect(IntPtr hWnd, ref RECT rect);

    }
}

