////using Avalonia.Visuals.Media.Imaging;
//using Avalonia.Rendering.SceneGraph;
//using Image = Avalonia.Controls.Image;
//using Point = Avalonia.Point;
//using Size = Avalonia.Size;
//using Disposable = System.Reactive.Disposables.Disposable;

//namespace Dorisoy.PanClient.Webcam;

///// <summary>
///// 自定义图像渲染器
///// </summary>
//[SupportedOSPlatform("windows")]
//public class WebcamImageRenderer : Image
//{
//    public WebcamImageRenderer()
//    {
//        this.GetObservable(SourceProperty).Subscribe(v =>
//        {
//            _subscriptions?.Dispose();
//            _subscriptions = null;

//            if (v is WebcamVlcSharpWriteableBitmap vb)
//            {
//                var m = vb.Updated.Select(_ => vb.PixelSize)
//                                .DistinctUntilChanged()
//                                .ObserveOn(RxApp.MainThreadScheduler)
//                                .Subscribe(_ => InvalidateMeasure());

//                var f = vb.Updated.Subscribe(_ => _stats.DeliveredFrame());

//                _subscriptions = new CompositeDisposable(m, f);
//            }
//        });
//    }

//    public static readonly AvaloniaProperty<WebcamSourceProvider> SourceProviderProperty =
//        AvaloniaProperty.RegisterDirect<WebcamImageRenderer, WebcamSourceProvider>(nameof(SourceProvider), v => v.SourceProvider, (x, v) => x.SourceProvider = v);

//    private WebcamSourceProvider _sourceProvider;

//    public WebcamSourceProvider SourceProvider
//    {
//        get => _sourceProvider;
//        set
//        {
//            if (SetAndRaise((DirectPropertyBase<WebcamSourceProvider>)SourceProviderProperty, ref _sourceProvider, value))
//            {
//                Source = _sourceProvider?.VideoSource;
//            }
//        }
//    }

//    public static readonly DirectProperty<WebcamImageRenderer, bool> DisplayRenderStatsProperty =
//        WebcamView.DisplayRenderStatsProperty.AddOwner<WebcamImageRenderer>(v => v.DisplayRenderStats, (s, v) => s.DisplayRenderStats = v);

//    private bool _displayRenderStats;

//    public bool DisplayRenderStats
//    {
//        get => _displayRenderStats;
//        set
//        {
//            if (SetAndRaise(DisplayRenderStatsProperty, ref _displayRenderStats, value))
//            {
//                _stats.Enabled = DisplayRenderStats;
//                _stats.Reset();
//            }
//        }
//    }

//    public static readonly DirectProperty<WebcamImageRenderer, bool> UseCustomDrawingOperationProperty =
//WebcamView.DisplayRenderStatsProperty.AddOwner<WebcamImageRenderer>(v => v.UseCustomDrawingOperation, (s, v) => s.UseCustomDrawingOperation = v);

//    private bool _useCustomDrawingOperation = true;

//    public bool UseCustomDrawingOperation
//    {
//        get => _useCustomDrawingOperation;
//        set => SetAndRaise(UseCustomDrawingOperationProperty, ref _useCustomDrawingOperation, value);
//    }

//    /// <summary>
//    /// 重置渲染状态
//    /// </summary>
//    public void ResetStats()
//    {
//        _stats.Reset();
//    }

//    //private IDisposable _subscriptions;
//    //private struct CustomOp : ICustomDrawOperation
//    //{
//    //    private Bitmap _source;
//    //    private Rect _sourceRect;
//    //    private Rect _destRect;
//    //    private BitmapInterpolationMode _interpolationMode;
//    //    private RenderingStats _stats;

//    //    public Rect Bounds => _destRect;

//    //    Rect ICustomDrawOperation.Bounds => throw new NotImplementedException();

//    //    public CustomOp(Bitmap source, Rect sourceRect, Rect destRect, BitmapInterpolationMode interpolationMode, RenderingStats stats)
//    //    {
//    //        _source = source;
//    //        _sourceRect = sourceRect;
//    //        _destRect = destRect;
//    //        _interpolationMode = interpolationMode;
//    //        _stats = stats;
//    //    }

//    //    public void Dispose()
//    //    {
//    //        if (!(_source is WebcamVlcSharpWriteableBitmap))
//    //            _source.Dispose();
//    //    }

//    //    public bool Equals(ICustomDrawOperation other) => false;

//    //    public bool HitTest(Point p) => false;

//    //    //public void Render(IDrawingContextImpl context)
//    //    //{
//    //    //    try
//    //    //    {
//    //    //        using (_stats.RenderFrame())
//    //    //        {
//    //    //            //if (_source is WebcamVlcSharpWriteableBitmap vb)
//    //    //            //    vb.Render(context, 1, _sourceRect, _destRect, _interpolationMode);
//    //    //            //else
//    //    //            //    context.DrawBitmap(context, 1, _sourceRect, _destRect, _interpolationMode);
//    //    //        }
//    //    //    }
//    //    //    catch (Exception e)
//    //    //    {
//    //    //        Console.WriteLine($"error render:{e.ToString()}");
//    //    //    }

//    //    //    _stats.Render(context, $"{_sourceRect.Width}x{_sourceRect.Height} -> {_destRect.Width.ToString("0.00")}x{_destRect.Height.ToString("0.00")}");
//    //    //}

//    //    //bool ICustomDrawOperation.HitTest(Point p)
//    //    //{
//    //    //    throw new NotImplementedException();
//    //    //}

//    //    //void ICustomDrawOperation.Render(ImmediateDrawingContext context)
//    //    //{
//    //    //    throw new NotImplementedException();
//    //    //}

//    //    //bool IEquatable<ICustomDrawOperation>.Equals(ICustomDrawOperation other)
//    //    //{
//    //    //    throw new NotImplementedException();
//    //    //}

//    //    void IDisposable.Dispose()
//    //    {
//    //        throw new NotImplementedException();
//    //    }
//    //}

//    //public new void Render(DrawingContext context)
//    //{
//    //    if (UseCustomDrawingOperation)
//    //    {
//    //        var source = Source as Bitmap;
//    //        if (source != null)
//    //        {
//    //            Rect viewPort = new Rect(Bounds.Size);
//    //            Size sourceSize = source.PixelSize.ToSize(1);
//    //            Vector scale = Stretch.CalculateScaling(Bounds.Size, sourceSize);
//    //            Size scaledSize = sourceSize * scale;
//    //            Rect destRect = viewPort
//    //                .CenterRect(new Rect(scaledSize))
//    //                .Intersect(viewPort);
//    //            Rect sourceRect = new Rect(sourceSize)
//    //                .CenterRect(new Rect(destRect.Size / scale));

//    //            var interpolationMode = RenderOptions.GetBitmapInterpolationMode(this);

//    //            context.Custom(new CustomOp(source, sourceRect, destRect, interpolationMode, _stats));
//    //        }
//    //    }
//    //    else
//    //    {
//    //        using (_stats.RenderFrame())
//    //        {
//    //            base.Render(context);
//    //        }

//    //        var size = (Source as Bitmap)?.PixelSize ?? default(PixelSize);
//    //        _stats.Render(context, $"{size.Width}x{size.Height}");
//    //    }
//    //}

//    private readonly RenderingStats _stats = new RenderingStats();


//    /// <summary>
//    /// 渲染状态
//    /// </summary>
//    private class RenderingStats
//    {
//        public int StatsFrameCount = 60;
//        public int UpdateStatsPerFrames = 10;
//        public bool Enabled = false;

//        private Queue<double> _frameTimes = new Queue<double>();
//        private Queue<double> _frameDurations = new Queue<double>();
//        private Stopwatch _start;
//        private int _rendered = 0;
//        private int _delivered = 0;
//        private double _last;

//        public RenderingStats()
//        {
//            Reset();
//        }

//        public void Reset()
//        {
//            _frameTimes.Clear();
//            _frameDurations.Clear();
//            _frameTimes.Enqueue(0);
//            _frameDurations.Enqueue(0);
//            _start = Stopwatch.StartNew();
//            _rendered = 0;
//            _delivered = 0;
//            _last = 0;
//            _text = null;
//        }

//        public void DeliveredFrame() => _delivered++;

//        public IDisposable RenderFrame()
//        {
//            if (_delivered <= _rendered)
//            {
//                //假设调整大小，例如一些与帧无关的强制无效
//                return Disposable.Empty;
//            }

//            var w = Stopwatch.StartNew();
//            _last = _start.ElapsedMilliseconds;
//            _frameTimes.Enqueue(_last);
//            _rendered++;

//            return Disposable.Create(() =>
//            {
//                w.Stop();
//                _frameDurations.Enqueue(w.ElapsedMilliseconds);
//                if (_frameDurations.Count > StatsFrameCount)
//                {
//                    _frameTimes.Dequeue();
//                    _frameDurations.Dequeue();
//                }
//            });
//        }

//        public override string ToString()
//        {
//            var dur = (_last - _frameTimes.Peek()) / _frameTimes.Count;
//            string f(double d) => d.ToString("0.00");
//            return $"total rendered:{_rendered}, delivered: {_delivered}, dropped: {_delivered - _rendered}\n" +
//                   $"stats for last {_frameTimes.Count} frames, rendered fps: {f(1000.0 / dur)}, time for render: {f(_frameDurations.Average())} ms";
//        }

//        public void Render(DrawingContext context, string info, double x = 10, double y = 10)
//        {
//            Render(context, info, x, y);
//        }

//        private FormattedText _text;

//        public void Render(IDrawingContextImpl context, string info, double x = 10, double y = 10)
//        {
//            if (Enabled)
//            {
//                //_text = _text != null && _rendered % UpdateStatsPerFrames != 0 ?
//                //                _text :
//                //                new FormattedText() { Text = $"{info}\n{ToString()}", FontSize = 12, Typeface = Typeface.Default };
//                //context.DrawRectangle(Brushes.Black, null, _text.Bounds.Translate(new Vector(x, y)));
//                //context.DrawText(Brushes.White, new Point(x, y), _text.PlatformImpl);
//            }
//        }
//    }
//}
