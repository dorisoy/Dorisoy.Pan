using System.Drawing;
using System.Drawing.Imaging;
using OfficeOpenXml.FormulaParsing.LexicalAnalysis;
using SharpDX;
using SharpDX.Direct2D1;
using SharpDX.DXGI;
using Bitmap = System.Drawing.Bitmap;
using D2D = SharpDX.Direct2D1;
using D3D11 = SharpDX.Direct3D11;
using DXGI = SharpDX.DXGI;
using Interop = SharpDX.Mathematics.Interop;
using WIC = SharpDX.WIC;

public class EncodedBmp
{
    public MemoryStream stream;
    public int Width, Height, Stride, startInx;

    public EncodedBmp(MemoryStream stream, int width, int height, int stride, int startInx)
    {
        this.stream = stream;
        Width = width;
        Height = height;
        this.Stride = stride;
        this.startInx = startInx;
    }
}

[SupportedOSPlatform("windows")]
public class DXScreenCapture : IDisposable
{
    private D3D11.Device d3dDevice;
    private OutputDuplication outputDuplication;
    private MemoryStream cachedStream;
    private D2D.Device d2dDevice;
    private D2D.DeviceContext frameDc;
    private D2D.DeviceContext textureDc;

    private Thread captureThread;
    private bool disposedValue;

    private int capture = 0;
    private int acquire = 0;

    internal double scale = 0.5;

    private CancellationTokenSource _cts;

    public void Init(double scale = 0.5, int screen = 0, int device = 0)
    {
        _cts = new CancellationTokenSource();

        this.scale = scale;
        var adapterIndex = device; // adapter index
        var outputIndex = screen; // output index
        using (var dxgiFactory = new DXGI.Factory1())
        using (var dxgiAdapter = dxgiFactory.GetAdapter1(adapterIndex))
        using (var output = dxgiAdapter.GetOutput(outputIndex))
        using (var dxgiOutput = output.QueryInterface<DXGI.Output1>())
        {
            this.d3dDevice = new D3D11.Device(dxgiAdapter,
#if DEBUG
                D3D11.DeviceCreationFlags.Debug |
#endif
                D3D11.DeviceCreationFlags.BgraSupport); // for D2D support

            outputDuplication = dxgiOutput.DuplicateOutput(this.d3dDevice);
        }

        using (var dxgiDevice = this.d3dDevice.QueryInterface<DXGI.Device>())
        using (var d2dFactory = new D2D.Factory1())
            d2dDevice = new D2D.Device(d2dFactory, dxgiDevice);
        frameDc = new D2D.DeviceContext(d2dDevice, D2D.DeviceContextOptions.EnableMultithreadedOptimizations);
        textureDc = new D2D.DeviceContext(d2dDevice, D2D.DeviceContextOptions.EnableMultithreadedOptimizations); // create a D2D device context


    }

    public EncodedBmp Draw(MemoryStream stream = null)
    {
        if (stream == null)
        {
            if (cachedStream == null)
                cachedStream = new MemoryStream();

            stream = cachedStream;
        }
        var ratio = scale; // resize ratio
        try
        {
            outputDuplication.TryAcquireNextFrame(500, out var _, out DXGI.Resource frame);
            if (frame != null)
            {
                using (frame)
                {

                    // get DXGI surface/bitmap from resource
                    //System.NullReferenceException:“Object reference not set to an instance of an object.”

                    using (var frameSurface = frame.QueryInterface<DXGI.Surface>())
                    using (var frameBitmap = new D2D.Bitmap1(frameDc, frameSurface))
                    {
                        int width = (int)(frameSurface.Description.Width * ratio);
                        int height = (int)(frameSurface.Description.Height * ratio);
                        if (width % 2 != 0)
                            width++;
                        if (height % 2 != 0)
                            height++;
                        // create a GPU resized texture/surface/bitmap
                        var desc = new D3D11.Texture2DDescription
                        {
                            CpuAccessFlags = D3D11.CpuAccessFlags.None, // only GPU
                            BindFlags = D3D11.BindFlags.RenderTarget, // to use D2D
                            Format = DXGI.Format.B8G8R8A8_UNorm,
                            //Format = DXGI.Format.b,
                            Width = width,
                            Height = height,
                            OptionFlags = D3D11.ResourceOptionFlags.None,
                            MipLevels = 1,
                            ArraySize = 1,
                            SampleDescription = { Count = 1, Quality = 0 },
                            Usage = D3D11.ResourceUsage.Default
                        };

                        using (var texture = new D3D11.Texture2D(d3dDevice, desc))
                        using (var textureSurface = texture.QueryInterface<DXGI.Surface>()) // this texture is a DXGI surface
                        using (var textureBitmap = new D2D.Bitmap1(textureDc, textureSurface)) // we can create a GPU bitmap on a DXGI surface
                        {

                            // associate the DC with the GPU texture/surface/bitmap
                            textureDc.Target = textureBitmap;

                            // this is were we draw on the GPU texture/surface
                            textureDc.BeginDraw();

                            // this will automatically resize
                            textureDc.DrawBitmap(
                                frameBitmap,
                                new Interop.RawRectangleF(0, 0, desc.Width, desc.Height),
                                1,
                                D2D.InterpolationMode.HighQualityCubic, // change this for quality vs speed
                                null,
                                null);

                            // commit draw
                            DrawCursor((float)scale, textureDc);

                            textureDc.EndDraw();

                            try
                            {
                                outputDuplication.ReleaseFrame();
                            }
                            catch { }
                            using (var wic = new WIC.ImagingFactory2())
                            using (var bmpEncoder = new WIC.BitmapEncoder(wic, WIC.ContainerFormatGuids.Bmp))
                            {
                                stream.Position = 0;
                                bmpEncoder.Initialize(stream);
                                using (var bmpFrame = new WIC.BitmapFrameEncode(bmpEncoder))
                                {
                                    bmpFrame.Initialize();

                                    // here we use the ImageEncoder (IWICImageEncoder)
                                    // that can write any D2D bitmap directly
                                    using (var imageEncoder = new WIC.ImageEncoder(wic, d2dDevice))
                                    {
                                        imageEncoder.WriteFrame(textureBitmap, bmpFrame, new WIC.ImageParameters(
                                            new D2D.PixelFormat(Format.R8G8B8A8_UNorm, D2D.AlphaMode.Ignore),
                                            textureDc.DotsPerInch.Width,
                                            textureDc.DotsPerInch.Height,
                                            0,
                                            0,
                                            desc.Width,
                                            desc.Height));
                                    }


                                    bmpFrame.Commit();
                                    bmpEncoder.Commit();
                                    var rect = new System.Drawing.Rectangle(0, 0, desc.Width, desc.Height);


                                    int stride = ((((width * 24) + 31) & ~31) >> 3);
                                    int lineLenght = stride;
                                    int data_ptr = lineLenght * (height - 1); // ptr to last line

                                    var buff = stream.GetBuffer();
                                    // 54 is info header of encoded bmp.
                                    var rgb = new EncodedBmp(stream, desc.Width, desc.Height, -stride, data_ptr + 54);

                                    return rgb;


                                    // var b = new Bitmap(stream);
                                    //return b;
                                }
                            }
                        }
                    }
                }
            }
        }
        catch { }
        return null;

    }

    Bitmap bitmap = null;
    void DrawCursor(float scale, D2D.DeviceContext dc)
    {
        // Drawing Cursor

        CursorInfo pci;
        pci.cbSize = System.Runtime.InteropServices.Marshal.SizeOf(typeof(CursorInfo));

        if (GetCursorInfo(out pci))
        {
            if (pci.flags == cursorVisible)
            {
                System.Drawing.Size iconSize = new System.Drawing.Size(16,16);
                if (bitmap == null || bitmap.Width != iconSize.Width || bitmap.Height != iconSize.Height)
                    bitmap = new Bitmap(iconSize.Width, iconSize.Height);

                using (Graphics gg = Graphics.FromImage(bitmap))
                {
                    gg.Clear(System.Drawing.Color.Transparent);
                    gg.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                    DrawIcon(gg.GetHdc(), 0, 0, pci.hCursor);
                    gg.ReleaseHdc();
                }

                int posX = (int)(pci.ptScreenPos.x * scale);
                int posY = (int)(pci.ptScreenPos.y * scale);
                using (var dxbmp = GDIBitmapToDXBitmap(dc, bitmap))
                    dc.DrawBitmap(
                               dxbmp,
                               new Interop.RawRectangleF(posX, posY, bitmap.Width * scale + posX, bitmap.Height * scale + posY),
                               1,
                               D2D.InterpolationMode.HighQualityCubic, // change this for quality vs speed
                               null,
                               null);
            }
        }
    }

    private SharpDX.Direct2D1.Bitmap GDIBitmapToDXBitmap(SharpDX.Direct2D1.DeviceContext dc, System.Drawing.Bitmap bmp)
    {
        BitmapProperties props = new BitmapProperties(new D2D.PixelFormat(Format.B8G8R8A8_UNorm, SharpDX.Direct2D1.AlphaMode.Premultiplied));
        var rect = new System.Drawing.Rectangle(0, 0, bmp.Width, bmp.Height);
        var data = bmp.LockBits(rect, ImageLockMode.ReadOnly, bmp.PixelFormat);
        int stride = data.Stride;
        SharpDX.DataStream ds = new DataStream(data.Scan0, data.Width * data.Height * 3, true, true);

        var dxbmp = new SharpDX.Direct2D1.Bitmap(dc, new Size2(bmp.Width, bmp.Height), ds, stride, props);
        bmp.UnlockBits(data);
        return dxbmp;
    }


    public int TargetFps = 1;
    public void StopCapture()
    {
        //Interlocked.Exchange(ref capture, 0);

        //if (captureThread != null && captureThread.IsAlive)
        //    captureThread.Join();

        _cts?.Cancel();

        while (streamBag.TryTake(out var s))
        {
            s.Dispose();
        }
    }


    ConcurrentBag<MemoryStream> streamBag = new ConcurrentBag<MemoryStream>();
    public void CaptureAuto(int targetFrameRate, Action<EncodedBmp> onCaptured)
    {
 

        TargetFps = targetFrameRate;

        //if (Interlocked.CompareExchange(ref capture, 0, 0) == 1)
        //    return;

        if (Interlocked.CompareExchange(ref acquire, 1, 0) == 1)
            return;

        //if (captureThread != null && captureThread.IsAlive)
        //{
        //    Interlocked.Exchange(ref capture, 0);
        //    captureThread.Join();
        //}

        if (streamBag.Count < 1)
        {
            streamBag.Add(new MemoryStream());
        }

        //Interlocked.Exchange(ref capture, 1);

#if DEBUG
        Stopwatch sw = Stopwatch.StartNew();
        int fNum = 0;
#endif

        Stopwatch measure = Stopwatch.StartNew();

        Task.Run(() =>
        {
            int remainder = 0;
            while (!_cts.IsCancellationRequested)
            {
                int requiredInterval = 1000 / TargetFps;

                measure.Restart();

                EncodedBmp img;

                if (!streamBag.TryTake(out var stream))
                    stream = new MemoryStream();

                img = Draw(stream);
                if (img != null)
                {

                    onCaptured?.Invoke(img);
                    int dt = (int)measure.ElapsedMilliseconds + remainder;
                    while (dt < requiredInterval)
                    {
                        Thread.Sleep(1);
                        dt = (int)measure.ElapsedMilliseconds + remainder;
                    }
                    remainder = dt - requiredInterval;
                }
#if DEBUG
                // print fps
                fNum++;
                if (sw.ElapsedMilliseconds > 1000)
                {
                    Console.WriteLine("Fps:" + fNum);
                    fNum = 0;
                    sw.Restart();
                }
#endif
            }

            var ss = capture;

        }, _cts.Token);

        Interlocked.Exchange(ref acquire, 0);
    }

    public void ReturnImage(EncodedBmp im)
    {
        streamBag.Add(im.stream);
    }


    protected virtual void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
            {
                StopCapture();
            }

            d3dDevice?.Dispose();
            d3dDevice = null;
            outputDuplication?.Dispose();
            outputDuplication = null;
            d2dDevice?.Dispose();
            d2dDevice = null;
            frameDc?.Dispose();
            frameDc = null;
            textureDc?.Dispose();
            textureDc = null;
            disposedValue = true;
            while (streamBag.TryTake(out var s))
                s.Dispose();
        }
    }

    ~DXScreenCapture()
    {
        Dispose(disposing: false);
    }

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    #region Cursor
    [StructLayout(LayoutKind.Sequential)]
    struct CursorInfo
    {
        public Int32 cbSize;
        public Int32 flags;
        public IntPtr hCursor;
        public PointApi ptScreenPos;
    }

    [StructLayout(LayoutKind.Sequential)]
    struct PointApi
    {
        public int x;
        public int y;
    }

    [DllImport("user32.dll")]
    static extern bool GetCursorInfo(out CursorInfo pci);

    [DllImport("user32.dll")]
    static extern bool DrawIcon(IntPtr hDC, int X, int Y, IntPtr hIcon);

    const Int32 cursorVisible = 0x00000001;
    #endregion

}
