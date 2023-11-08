using SkiaSharp;


namespace Dorisoy.PanClient.Controls;


public class Layer
{
    public SKImage Image { get; set; }
    public SKSurface Surface { get; set; }
}

/*
//DrawingContextImpl : IDrawingContextImpl
public class DrawingCanvas : UserControl
{
    //This is where we keep the bitmaps that comprise individual layers
    //we use to composite what we present to the user
    private Layer UILayer;
    private int ActiveLayer;
    private Layer CachedActiveLayer;
    private List<Layer> ImageLayers;

    //Our render target we compile everything to and present to the user
    private RenderTargetBitmap RenderTarget;
    private ISkiaDrawingContextImpl SkiaContext;

    //Reference to the currently active drawing tool
    public IDrawingTool Tool { get; set; }
    //Should have a OnToolChange Event

    public override void EndInit()
    {
        SKPaint SKBrush = new SKPaint();
        SKBrush.IsAntialias = true;
        SKBrush.Color = new SKColor(0, 0, 0);
        SKBrush.Shader = SKShader.CreateColor(SKBrush.Color);
        RenderTarget = new RenderTargetBitmap(new PixelSize((int)Width, (int)Height), new Vector(96, 96));

        var context = RenderTarget.CreateDrawingContext() as IDrawingContextImpl;
        var skia = context.GetFeature<ISkiaSharpApiLeaseFeature>();
        using (var lease = skia.Lease())
        {
            SKCanvas canvas = lease.SkCanvas;
            if (canvas != null)
            {
                //RenderAction?.Invoke(canvas);
            }
        }

        SkiaContext.SkCanvas.Clear(new SKColor(255, 255, 255));

        //Tool = new SquareBrush(5.0f, SKBrush);

        PointerPressed += DrawingCanvas_PointerPressed;
        PointerMoved += DrawingCanvas_PointerMoved;
        PointerExited += DrawingCanvas_PointerLeave;
        PointerReleased += DrawingCanvas_PointerReleased;
        PointerEntered += DrawingCanvas_PointerEnter;

        base.EndInit();
    }



    private DrawingEvent PointerToDrawing(PointerEventArgs e)
    {
        return new DrawingEvent()
        {
            Handled = e.Handled,
            InputModifiers = e.KeyModifiers,
            //This position should be capped at the bounds of the canvas
            Position = e.GetPosition(this).ToSKPoint()
        };
    }

    private void DrawingCanvas_PointerEnter(object sender, PointerEventArgs e)
    {
        if (Tool != null && SkiaContext != null)
        {
            DrawingEvent de = PointerToDrawing(e);
            Tool.OnPointerEnter(SkiaContext.SkCanvas, de);
            e.Handled = de.Handled;

            if (de.Handled)
                InvalidateVisual();
        }
    }

    private void DrawingCanvas_PointerReleased(object sender, PointerReleasedEventArgs e)
    {
        if (Tool != null && SkiaContext != null)
        {
            DrawingEvent de = PointerToDrawing(e);
            Tool.OnPointerRelease(SkiaContext.SkCanvas, de);
            e.Handled = de.Handled;

            if (de.Handled)
                InvalidateVisual();
        }
    }

    private void DrawingCanvas_PointerLeave(object sender, PointerEventArgs e)
    {
        if (Tool != null && SkiaContext != null)
        {
            DrawingEvent de = PointerToDrawing(e);
            Tool.OnPointerLeave(SkiaContext.SkCanvas, de);
            e.Handled = de.Handled;

            if (de.Handled)
                InvalidateVisual();
        }
    }

    private void DrawingCanvas_PointerMoved(object sender, PointerEventArgs e)
    {
        if (Tool != null && SkiaContext != null)
        {
            DrawingEvent de = PointerToDrawing(e);
            Tool.OnPointerMove(SkiaContext.SkCanvas, de);
            e.Handled = de.Handled;

            if (de.Handled)
                InvalidateVisual();
        }
    }

    private void DrawingCanvas_PointerPressed(object sender, PointerPressedEventArgs e)
    {
        if (Tool != null)
        {
            DrawingEvent de = PointerToDrawing(e);
            //Tool.OnPointerPress(CurrentSurface.Canvas, de);
            Tool.OnPointerPress(SkiaContext.SkCanvas, de);
            e.Handled = de.Handled;

            if (de.Handled)
                InvalidateVisual();
        }
    }

    public Task<bool> SaveAsync(string path)
    {
        return Task.Run(() =>
        {
            try
            {
                RenderTarget.Save(path);
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        });
    }

    public override void Render(DrawingContext context)
    {
        context.DrawImage(RenderTarget, 1.0,
            new Rect(0, 0, RenderTarget.PixelSize.Width, RenderTarget.PixelSize.Height),
            new Rect(0, 0, Width, Height)
            );
    }

    //public override void Render(DrawingContext context)
    //{
    //    if (renderingLogic == null || renderingLogic.Bounds != this.Bounds)
    //    {
    //        // (re)create drawing operation matching actual bounds
    //        if (renderingLogic != null)
    //            renderingLogic.Dispose();
    //        renderingLogic = new SkiaDrawOperation();
    //        renderingLogic.RenderAction += (canvas) => OnSkiaRendering(canvas);
    //        renderingLogic.Bounds = new Rect(0, 0, this.Bounds.Width, this.Bounds.Height);
    //    }

    //    renderingLogic.Bounds = new Rect(0, 0, this.Bounds.Width, this.Bounds.Height);

    //    context.Custom(renderingLogic);
    //}
}

public partial class SkiaCanvas : UserControl
{
    class RenderingLogic : ICustomDrawOperation
    {
        public Action<SKCanvas> RenderCall;
        public Rect Bounds { get; set; }

        public void Dispose() { }

        public bool Equals(ICustomDrawOperation? other) => other == this;

        // not sure what goes here....
        public bool HitTest(Point p) { return false; }


        public void Render(IDrawingContextImpl context)
        {
            //var canvas = (context as ISkiaDrawingContextImpl)?.SkCanvas;
            //if (canvas != null)
            //{
            //    Render(canvas);
            //}
            //var context = RenderTarget.CreateDrawingContext() as IDrawingContextImpl;
            var skia = context.GetFeature<ISkiaSharpApiLeaseFeature>();
            using (var lease = skia.Lease())
            {
                SKCanvas canvas = lease.SkCanvas;
                if (canvas != null)
                {
                    //RenderAction?.Invoke(canvas);
                    Render(canvas);
                }
            }

        }

        public void Render(ImmediateDrawingContext context)
        {
            //var context = RenderTarget.CreateDrawingContext() as IDrawingContextImpl;

            //var skia = context.GetFeature<ISkiaSharpApiLeaseFeature>();
            //using (var lease = skia.Lease())
            //{
            //    SKCanvas canvas = lease.SkCanvas;
            //    if (canvas != null)
            //    {
            //        //RenderAction?.Invoke(canvas);
            //        Render(canvas);
            //    }
            //}
        }

        private void Render(SKCanvas canvas)
        {
            RenderCall?.Invoke(canvas);
        }
    }

    RenderingLogic renderingLogic;

    public event Action<SKCanvas> RenderSkia;

    public SkiaCanvas()
    {
        //InitializeComponent();
        renderingLogic = new RenderingLogic();
        renderingLogic.RenderCall += (canvas) => RenderSkia?.Invoke(canvas);
    }

    public override void Render(DrawingContext context)
    {
        renderingLogic.Bounds = new Rect(0, 0, this.Bounds.Width, this.Bounds.Height);

        context.Custom(renderingLogic);

        // If you want continual invalidation (like a game):
        //Dispatcher.UIThread.InvokeAsync(InvalidateVisual, DispatcherPriority.Background);
    }
}

*/


//DrawingCanvas : UserControl
//https://github.com/AvaloniaUI/Avalonia/blob/master/samples/RenderDemo/Pages/CustomSkiaPage.cs

/*
public class DrawingCanvas : UserControl
{
    private readonly GlyphRun _noSkia;

    //private Layer UILayer;
    //private int ActiveLayer;
    //private Layer CachedActiveLayer;
    //private List<Layer> ImageLayers;


    public static List<Ellipse> Ellipses { get; set; }
    public static List<Path> Paths { get; set; }

    private RenderTargetBitmap RenderTarget;

    //实验
    private readonly IAppState _appState;
    private Point currentPoint = new();
    private bool pressed;
    private bool _disposedValue;
    private readonly BrushSettings _brushSettings;
    private readonly Queue<Point> _linePointsQueue = new();
    private readonly DispatcherTimer _lineRenderingTimer = new();


    public DrawingCanvas()
    {
        ClipToBounds = true;

        _appState = Locator.Current.GetRequiredService<IAppState>();
        _brushSettings = _appState.BrushSettings;

        var text = "Current rendering API is not Skia";
        var glyphs = text.Select(ch => Typeface.Default.GlyphTypeface.GetGlyph(ch)).ToArray();
        _noSkia = new GlyphRun(Typeface.Default.GlyphTypeface, 12, text.AsMemory(), glyphs);


        //定义Timer用于监视渲染
        _lineRenderingTimer.Tick += LineRenderingTimer_Tick;
        _lineRenderingTimer.Interval = TimeSpan.FromMilliseconds(Globals.RenderingIntervalMs);
        _lineRenderingTimer.Start();
    }

    class CustomDrawOp : ICustomDrawOperation
    {
        private readonly IImmutableGlyphRunReference _noSkia;

        public CustomDrawOp(Rect bounds, GlyphRun noSkia)
        {
            _noSkia = noSkia.TryCreateImmutableGlyphRunReference();
            Bounds = bounds;
        }

        public void Dispose()
        {
            // No-op
        }

        public Rect Bounds { get; }
        public bool HitTest(Point p) => false;
        public bool Equals(ICustomDrawOperation other) => false;

        static Stopwatch St = Stopwatch.StartNew();



        public void Render(ImmediateDrawingContext context)
        {
            var leaseFeature = context.TryGetFeature<ISkiaSharpApiLeaseFeature>();
            if (leaseFeature == null)
                context.DrawGlyphRun(Brushes.Black, _noSkia);
            else
            {
                using var lease = leaseFeature.Lease();
                var canvas = lease.SkCanvas;
                canvas.Save();

                //// create the first shader
                //var colors = new SKColor[] {
                //        new SKColor(0, 255, 255),
                //        new SKColor(255, 0, 255),
                //        new SKColor(255, 255, 0),
                //        new SKColor(0, 255, 255)
                //    };

                //var sx = Animate(100, 2, 10);
                //var sy = Animate(1000, 5, 15);
                //var lightPosition = new SKPoint(
                //    (float)(Bounds.Width / 2 + Math.Cos(St.Elapsed.TotalSeconds) * Bounds.Width / 4),
                //    (float)(Bounds.Height / 2 + Math.Sin(St.Elapsed.TotalSeconds) * Bounds.Height / 4));
                //using (var sweep =
                //    SKShader.CreateSweepGradient(new SKPoint((int)Bounds.Width / 2, (int)Bounds.Height / 2), colors,
                //        null))
                //using (var turbulence = SKShader.CreatePerlinNoiseFractalNoise(0.05f, 0.05f, 4, 0))
                //using (var shader = SKShader.CreateCompose(sweep, turbulence, SKBlendMode.SrcATop))
                //using (var blur = SKImageFilter.CreateBlur(Animate(100, 2, 10), Animate(100, 5, 15)))
                //using (var paint = new SKPaint
                //{
                //    Shader = shader,
                //    ImageFilter = blur
                //})
                //    canvas.DrawPaint(paint);

                //using (var pseudoLight = SKShader.CreateRadialGradient(
                //    lightPosition,
                //    (float)(Bounds.Width / 3),
                //    new[] {
                //            new SKColor(255, 200, 200, 100),
                //            SKColors.Transparent,
                //            new SKColor(40,40,40, 220),
                //            new SKColor(20,20,20, (byte)Animate(100, 200,220)) },
                //    new float[] { 0.3f, 0.3f, 0.8f, 1 },
                //    SKShaderTileMode.Clamp))
                //using (var paint = new SKPaint
                //{
                //    Shader = pseudoLight
                //})

                //canvas.DrawPaint(paint);

                //foreach (var p in Paths)
                //{
                //    canvas.DrawPath(p);
                //}



                canvas.Restore();
            }
        }

        static int Animate(int d, int from, int to)
        {
            var ms = (int)(St.ElapsedMilliseconds / d);
            var diff = to - from;
            var range = diff * 2;
            var v = ms % range;
            if (v > diff)
                v = range - v;
            var rv = v + from;
            if (rv < from || rv > to)
                throw new Exception("WTF");
            return rv;
        }
    }


    public override void EndInit()
    {
        //PointerPressed += DrawingCanvas_PointerPressed;
        //PointerMoved += DrawingCanvas_PointerMoved;
        //PointerExited += DrawingCanvas_PointerExited;
        //PointerReleased += DrawingCanvas_PointerReleased;
        //PointerEntered += DrawingCanvas_PointerEntered;
        base.EndInit();
    }


    private void LineRenderingTimer_Tick(object sender, EventArgs e)
    {
        Dispatcher.UIThread.InvokeAsync(async () =>
        {
            var points = RenderLine();
            if (!points.Any())
            {
                return;
            }
            //绘制
            await DrawLineAsync(points);
        });
    }


    private void DrawingCanvas_PointerPressed(object sender, PointerPressedEventArgs e)
    {
        var p = e.GetPosition(this).ToSKPoint();
        currentPoint = e.GetPosition(this);
        pressed = true;

        this.InvalidateVisual();
    }

    private void DrawingCanvas_PointerEntered(object sender, PointerEventArgs e)
    {

    }

    private async void DrawingCanvas_PointerReleased(object sender, PointerReleasedEventArgs e)
    {
        pressed = false;
        var newPoint = RestrictPointToCanvas(currentPoint.X, currentPoint.Y);
        DrawPoint(newPoint.X, newPoint.Y);
        await DrawPointAsync(newPoint.X, newPoint.Y);

        this.InvalidateVisual();
    }

    private void DrawingCanvas_PointerExited(object sender, PointerEventArgs e)
    {

    }

    //private void DrawingCanvas_PointerMoved(object sender, PointerEventArgs e)
    //{
    //    Point mousePos = e.GetPosition(canvas);

    //    var x = Math.Min(mousePos.X, mouseDownPos.X);
    //    var y = Math.Min(mousePos.Y, mouseDownPos.Y);
    //    var x1 = Math.Max(mouseDownPos.X, mousePos.X);
    //    var y1 = Math.Max(mouseDownPos.Y, mousePos.Y);

    //    var w = Math.Max(mousePos.X, mouseDownPos.X) - x;
    //    var h = Math.Max(mousePos.Y, mouseDownPos.Y) - y;

    //    rect.Width = w;
    //    rect.Height = h;

    //    Canvas.SetLeft(rect, x);
    //    Canvas.SetRight(rect, x1 + w);
    //    Canvas.SetTop(rect, y);
    //    Canvas.SetBottom(rect, y1 + h);
    //}


    

    /// <summary>
    /// 绘制点
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    public async Task DrawPointAsync(double x, double y)
    {
        var data = PayloadConverter.ToBytes(x, y, _appState.BrushSettings.BrushThickness, _appState.BrushSettings.BrushColor);
        var point = PayloadConverter.ToPoint(data);
        await Dispatcher.UIThread.InvokeAsync(() =>
        {
            //this.Children.Add(point);
            Ellipses.Add(point);
        });
    }

    /// <summary>
    /// 绘制画线
    /// </summary>
    /// <param name="cpoints"></param>
    /// <returns></returns>
    public async Task DrawLineAsync(List<Point> cpoints)
    {
        var data = PayloadConverter.ToBytes(cpoints, _appState.BrushSettings.BrushThickness, _appState.BrushSettings.BrushColor);
        var (points, thickness, colorBrush) = PayloadConverter.ToLine(data);
        await Dispatcher.UIThread.InvokeAsync(() =>
        {
            RenderLine(points, thickness, colorBrush);
        });
    }


    public List<Point> RenderLine()
    {
        if (_linePointsQueue.Count == 0)
        {
            return new List<Point>();
        }

        var myPointCollection = new Points();

        var result = _linePointsQueue.ToList();
        var firstPoint = _linePointsQueue.Dequeue();

        while (_linePointsQueue.Count > 0)
        {
            var point = _linePointsQueue.Dequeue();
            myPointCollection.Add(point);
        }

        var pathGeometry = new PathGeometry();
        var pathFigure = new PathFigure
        {
            Segments = new PathSegments
            {
                new PolyLineSegment
                {
                    Points = myPointCollection
                }
            },
            StartPoint = firstPoint,
            IsClosed = false
        };
        pathGeometry.Figures.Add(pathFigure);

        var path = new Path
        {
            Stroke = _brushSettings.ColorBrush,
            StrokeThickness = _brushSettings.Thickness,
            Data = pathGeometry
        };

        //this.Children.Add(path);
        Paths.Add(path);



        var ellipse = new Ellipse
        {
            Margin = new Thickness(firstPoint.X - _brushSettings.HalfThickness, firstPoint.Y - _brushSettings.HalfThickness, 0, 0),
            Fill = _brushSettings.ColorBrush,
            Width = _brushSettings.Thickness,
            Height = _brushSettings.Thickness
        };
        //this.Children.Add(ellipse);
        Ellipses.Add(ellipse);
        return result;
    }


    public void RenderLine(Queue<Point> linePointsQueue, double thickness, SolidColorBrush colorBrush)
    {
        if (linePointsQueue.Count == 0)
        {
            return;
        }

        var myPointCollection = new Points();

        var firstPoint = linePointsQueue.Dequeue();

        while (linePointsQueue.Count > 0)
        {
            var point = linePointsQueue.Dequeue();
            myPointCollection.Add(point);
        }

        var pathGeometry = new PathGeometry();
        var pathFigure = new PathFigure
        {
            Segments = new PathSegments
            {
                new PolyLineSegment
                {
                    Points = myPointCollection
                }
            },
            StartPoint = firstPoint,
            IsClosed = false
        };
        pathGeometry.Figures.Add(pathFigure);

        var path = new Path
        {
            Stroke = colorBrush,
            StrokeThickness = thickness,
            Data = pathGeometry
        };

        //this.Children.Add(path);
        Paths.Add(path);

        var ellipse = new Ellipse
        {
            Margin = new Thickness(firstPoint.X - thickness / 2, firstPoint.Y - thickness / 2, 0, 0),
            Fill = colorBrush,
            Width = thickness,
            Height = thickness
        };
        //this.Children.Add(ellipse);
        Ellipses.Add(ellipse);
    }


    public Point RestrictPointToCanvas(double x, double y)
    {
        if (x > _brushSettings.MaxBrushPointX)
        {
            x = _brushSettings.MaxBrushPointX;
        }
        else if (x < _brushSettings.MinBrushPoint)
        {
            x = _brushSettings.MinBrushPoint;
        }

        if (y > _brushSettings.MaxBrushPointY)
        {
            y = _brushSettings.MaxBrushPointY;
        }
        else if (y < _brushSettings.MinBrushPoint)
        {
            y = _brushSettings.MinBrushPoint;
        }

        return new Point(x, y);
    }

    public void DrawPoint(double x, double y)
    {
        var ellipse = new Ellipse
        {
            Margin = new Thickness(x - _brushSettings.HalfThickness, y - _brushSettings.HalfThickness, 0, 0),
            Fill = _brushSettings.ColorBrush,
            Width = _brushSettings.Thickness,
            Height = _brushSettings.Thickness
        };

        //this.Children.Add(ellipse);
        Ellipses.Add(ellipse);
    }


    public Task<bool> SaveAsync(string path)
    {
        return Task.Run(() =>
        {
            try
            {
                RenderTarget.Save(path);
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        });
    }


    //void RenderToFile(Geometry geometry, Brush brush, string path)
    //{
    //    var control = new DrawingPresenter()
    //    {
    //        Drawing = new GeometryDrawing
    //        {
    //            Geometry = geometry,
    //            Brush = brush,
    //        },
    //        Width = geometry.Bounds.Right,
    //        Height = geometry.Bounds.Bottom
    //    };

    //    RenderToFile(control, path);
    //}

    void RenderToFile(Control target, string path)
    {
        var pixelSize = new PixelSize((int)target.Width, (int)target.Height);
        var size = new Size(target.Width, target.Height);
        using (RenderTargetBitmap bitmap = new RenderTargetBitmap(pixelSize, new Vector(96, 96)))
        {
            target.Measure(size);
            target.Arrange(new Rect(size));
            bitmap.Render(target);
            bitmap.Save(path);
        }
    }

    //public override void Render(DrawingContext context)
    //{
    //    context.DrawImage(RenderTarget, new Rect(0, 0, Bounds.Width, Bounds.Height));
    //}

    public override void Render(DrawingContext context)
    {
        context.Custom(new CustomDrawOp(new Rect(0, 0, Bounds.Width, Bounds.Height), _noSkia));
        Dispatcher.UIThread.InvokeAsync(InvalidateVisual, DispatcherPriority.Background);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposedValue)
        {
            if (disposing)
            {
                // TODO: 释放托管状态(托管对象)
            }

            // TODO: 释放未托管的资源(未托管的对象)并重写终结器
            // TODO: 将大型字段设置为 null
            _disposedValue = true;
        }
    }

    // // TODO: 仅当“Dispose(bool disposing)”拥有用于释放未托管资源的代码时才替代终结器
    // ~DrawingCanvas()
    // {
    //     // 不要更改此代码。请将清理代码放入“Dispose(bool disposing)”方法中
    //     Dispose(disposing: false);
    // }

    public void Dispose()
    {
        // 不要更改此代码。请将清理代码放入“Dispose(bool disposing)”方法中
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}

*/
/*
public class LineBoundsDemoControl : Control
{
    static LineBoundsDemoControl()
    {
        AffectsRender<LineBoundsDemoControl>(AngleProperty);
    }

    public LineBoundsDemoControl()
    {
        var timer = new DispatcherTimer();
        timer.Interval = TimeSpan.FromSeconds(1 / 60.0);
        timer.Tick += (sender, e) => Angle += Math.PI / 360;
        timer.Start();
    }

    public static readonly StyledProperty<double> AngleProperty =
        AvaloniaProperty.Register<LineBoundsDemoControl, double>(nameof(Angle));

    public double Angle
    {
        get => GetValue(AngleProperty);
        set => SetValue(AngleProperty, value);
    }

    public override void Render(DrawingContext drawingContext)
    {
        var lineLength = Math.Sqrt((100 * 100) + (100 * 100));

        var diffX = LineBoundsHelper.CalculateAdjSide(Angle, lineLength);
        var diffY = LineBoundsHelper.CalculateOppSide(Angle, lineLength);


        var p1 = new Point(200, 200);
        var p2 = new Point(p1.X + diffX, p1.Y + diffY);

        var pen = new Pen(Brushes.Green, 20, lineCap: PenLineCap.Square);
        var boundPen = new Pen(Brushes.Black);

        drawingContext.DrawLine(pen, p1, p2);

        drawingContext.DrawRectangle(boundPen, LineBoundsHelper.CalculateBounds(p1, p2, pen));
    }



    public Bitmap GetGraphic(int pixelsPerModule = 8, Bitmap bitmap = null, bool drawQuietZones = false)
    {
        var size = (this.QrCodeData.ModuleMatrix.Count - (drawQuietZones ? 0 : 8)) * pixelsPerModule;
        var offset = drawQuietZones ? 0 : 4 * pixelsPerModule;

        SKPaint lightPaint = new SKPaint();
        lightPaint.Color = new SKColor(255, 255, 255);
        SKPaint darkPaint = new SKPaint();
        darkPaint.Color = new SKColor(0, 0, 0);
        WriteableBitmap renderTarget = new WriteableBitmap(new PixelSize(size, size), new Vector(96, 96), PixelFormat.Rgba8888);
        Stream stream;
        using (var lockedBitmap = renderTarget.Lock())
        {
            SKImageInfo info = new SKImageInfo(lockedBitmap.Size.Width, lockedBitmap.Size.Height, lockedBitmap.Format.ToSkColorType());
            SKSurface currentSurface = SKSurface.Create(info, lockedBitmap.Address, lockedBitmap.RowBytes);
            SKCanvas canvas = currentSurface.Canvas;
            canvas.Clear(new SKColor(255, 255, 255));
            for (var x = 0; x < size + offset; x = x + pixelsPerModule)
            {
                for (var y = 0; y < size + offset; y = y + pixelsPerModule)
                {
                    var module = this.QrCodeData.ModuleMatrix[(y + pixelsPerModule) / pixelsPerModule - 1][(x + pixelsPerModule) / pixelsPerModule - 1];

                    if (module)
                    {
                        canvas.DrawRect(x - offset, y - offset, pixelsPerModule, pixelsPerModule, darkPaint);
                    }
                    else
                    {
                        canvas.DrawRect(x - offset, y - offset, pixelsPerModule, pixelsPerModule, lightPaint);
                    }
                }
            }
            if (bitmap != null)
            {
                using (Stream icon = new MemoryStream())
                {
                    bitmap.Save(icon);
                    icon.Seek(0, SeekOrigin.Begin);
                    SKData data = SKData.Create(icon);
                    SKImage img = SKImage.FromEncodedData(data);
                    int position = (int)(size - bitmap.Size.Width) / 2;
                    canvas.DrawImage(img, new SKPoint(position, position));
                }
            }
            stream = SKImage.FromPixels(info, lockedBitmap.Address, lockedBitmap.RowBytes)
                    .Encode(SKEncodedImageFormat.Png, 100)
                    .AsStream();
        }
        renderTarget.Dispose();

        Bitmap qrCode = new Bitmap(stream);
        stream.Close();
        stream.Dispose();
        return qrCode;
    }


}

*/

