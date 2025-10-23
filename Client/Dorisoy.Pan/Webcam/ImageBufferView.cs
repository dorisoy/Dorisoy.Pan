using Size = Avalonia.Size;

namespace Dorisoy.Pan;

/// <summary>
/// 用于二进制流图片预览
/// </summary>
public partial class ImageBufferView : Control
{
    public static readonly AvaloniaProperty<Stretch> StretchProperty =
        AvaloniaProperty.Register<ImageBufferView, Stretch>(
            nameof(Stretch));

    public Stretch Stretch
    {
        get => (Stretch)this.GetValue(StretchProperty)!;
        set => this.SetValue(StretchProperty, value);
    }

    public static readonly AvaloniaProperty<StretchDirection> StretchDirectionProperty =
        AvaloniaProperty.Register<ImageBufferView, StretchDirection>(
            nameof(StretchDirection), StretchDirection.Both);

    public StretchDirection StretchDirection
    {
        get => (StretchDirection)this.GetValue(StretchDirectionProperty)!;
        set => this.SetValue(StretchDirectionProperty, value);
    }

    static ImageBufferView()
    {
        AffectsRender<ImageBufferView>(BitmapProperty, StretchProperty, StretchDirectionProperty);
        AffectsMeasure<ImageBufferView>(BitmapProperty, StretchProperty, StretchDirectionProperty);
        AffectsArrange<ImageBufferView>(BitmapProperty, StretchProperty, StretchDirectionProperty);
    }

    public static readonly AvaloniaProperty<ArraySegment<byte>?> ImageBufferProperty =
        AvaloniaProperty.Register<ImageBufferView, ArraySegment<byte>?>(
            nameof(ImageBuffer), coerce: (sender, e) =>
            {
                if (sender is not ImageBufferView { _canUpdataBitmap: true } control)
                {
                    return e;
                }

                if (e.HasValue
                && e.Value.Array != null
                && e.Value.Array.Length > 0)
                {
                    var oldBitmap = control.Bitmap;
                    using MemoryStream stream = new(e.Value.Array);
                    control.Bitmap = new Bitmap(stream);
                    oldBitmap?.Dispose();
                    control._canUpdataBitmap = false;
                }
                else
                {
                    control.Bitmap = null;
                }
                return e;
            });

    /// <summary>
    /// 要渲染的图片的流
    /// </summary>
    public ArraySegment<byte>? ImageBuffer
    {
        get => (ArraySegment<byte>?)this.GetValue(ImageBufferProperty);
        set => this.SetValue(ImageBufferProperty, value);
    }

    public static readonly AvaloniaProperty<Bitmap?> BitmapProperty =
        AvaloniaProperty.Register<ImageBufferView, Bitmap?>(
            nameof(Bitmap), coerce: (sender, e) =>
            {
                if (sender is not ImageBufferView control)
                {
                    return e;
                }

                control.SourceSize = e?.Size ?? control.RenderSize;
                return e;
            });

    /// <summary>
    /// 实际渲染的画面
    /// </summary>
    public Bitmap? Bitmap
    {
        get => (Bitmap?)this.GetValue(BitmapProperty)!;
        set => this.SetValue(BitmapProperty, value);
    }

    /// <summary>
    /// 需要更新渲染画面
    /// </summary>
    private bool _canUpdataBitmap = true;

    public Size RenderSize => this.Bounds.Size;

    public Size SourceSize { get; private set; }

    protected override Size MeasureOverride(Size constraint)
    {
        return Bitmap is not null ? Stretch.CalculateSize(constraint, SourceSize, StretchDirection) : default;
    }

    protected override Size ArrangeOverride(Size arrangeSize)
    {
        return Bitmap is not null ? Stretch.CalculateSize(arrangeSize, SourceSize) : default;
    }

    public override void Render(DrawingContext drawingContext)
    {
        if (Bitmap is not null)
        {
            var sourceSize = SourceSize;

            var viewPort = new Rect(RenderSize);
            var scale = Stretch.CalculateScaling(RenderSize, sourceSize, StretchDirection);
            var scaledSize = sourceSize * scale;
            var destRect = viewPort
                .CenterRect(new Rect(scaledSize))
                .Intersect(viewPort);
            var sourceRect = new Rect(sourceSize)
                .CenterRect(new Rect(destRect.Size / scale));

            if (Bitmap is not null)
            {
                drawingContext.DrawImage(Bitmap, sourceRect, destRect);
            }
            _canUpdataBitmap = true;
        }

        base.Render(drawingContext);
    }
}
