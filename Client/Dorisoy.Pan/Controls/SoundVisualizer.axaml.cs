using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Color = Avalonia.Media.Color;
using Point = Avalonia.Point;
using Size = Avalonia.Size;

namespace Dorisoy.PanClient.Controls;

public partial class SoundVisualizer : UserControl
{
    public static readonly DirectProperty<SoundVisualizer, SoundSliceData> DataProperty =
       AvaloniaProperty.RegisterDirect<SoundVisualizer, SoundSliceData>(
           nameof(Data),
           o => o.Data,
           (o, v) => o.Data = v);

    private SoundSliceData _data;

    public SoundSliceData Data
    {
        get => _data;
        set
        {
            _data = value;
            // 在这里触发重绘
            InvalidateVisual();
        }
    }

    public SoundVisualizer()
    {
        InitializeComponent();
    }

    // 重写OnRender函数，在这里绘制柱形图
    public override void Render(DrawingContext context)
    {
        base.Render(context);

        /*
        if (Data != null)
        {
            // 假定柱形图的最大高度等于控件的高度
            double maxHeight = this.Bounds.Height;
            double maxWidth = this.Bounds.Width;

            // 循环遍历V1到V20，绘制每个柱形条
            double barWidth = maxWidth / 20;// 柱形条的宽度
            for (int i = 0; i < 20; i++)
            {
                var valueProperty = Data.GetType().GetProperty($"V{i + 1}");
                if (valueProperty != null)
                {
                    int value = (int)valueProperty.GetValue(Data);
                    double barHeight = (value / 100.0) * maxHeight;// 假设值域是0到100

                    // 计算柱形条的位置和大小
                    double x = i * (barWidth + 2);// 包括柱形条之间的间距，这里设为2
                    double y = maxHeight - barHeight;// 从底部向上绘制

                    // 绘制单个柱形条
                    var rect = new Rect(x, y, barWidth, barHeight);

                    // 设置画刷颜色为蓝色
                    var brush = Brushes.Blue;


                    // 使用指定的画刷填充矩形
                    context.FillRectangle(brush, rect);
                }
            }
        }
        */

        if (Data != null)
        {
            double maxHeight = this.Bounds.Height;
            double maxWidth = this.Bounds.Width;

            double spacing = 2;   // 柱形条之间的间距
            double barWidth = (maxWidth / 20) - 2;  // 柱形条的宽度
            double cornerRadius = 4;// 圆角半径


            for (int i = 0; i < 20; i++)
            {
                var valueProperty = Data.GetType().GetProperty($"V{i + 1}");
                if (valueProperty != null)
                {
                    int value = (int)valueProperty.GetValue(Data);
                    double barHeight = (value / 100.0) * maxHeight;
                    double x = i * (barWidth + spacing);
                    double y = maxHeight - barHeight;

                    var geometry = new StreamGeometry();

                    using (var ctx = geometry.Open())
                    {
                        // 开始点在左下角
                        ctx.BeginFigure(new Point(x, maxHeight), true);

                        // 直线到左上角（圆角的开始）
                        ctx.LineTo(new Point(x, y + cornerRadius));

                        // 创建圆角 - 左上角
                        ctx.ArcTo(
                            new Point(x + cornerRadius, y),
                            new Size(cornerRadius, cornerRadius),
                            90,
                            false,
                            SweepDirection.Clockwise

                        );

                        // 直线到右上角（圆角的结束）
                        ctx.LineTo(new Point(x + barWidth - cornerRadius, y));

                        // 创建圆角 - 右上角
                        ctx.ArcTo(
                            new Point(x + barWidth, y + cornerRadius),
                            new Size(cornerRadius, cornerRadius),
                            90,
                            false,
                            SweepDirection.Clockwise
                        );

                        // 直线到右下角
                        ctx.LineTo(new Point(x + barWidth, maxHeight));

                        // 完成该形状的闭合
                        ctx.LineTo(new Point(x, maxHeight));
                    }

                    // 设置画刷颜色为蓝色
                    var brushe = new SolidColorBrush(Color.Parse("#00aaeb"));

                    // 使用指定的画刷填充圆角矩形
                    context.DrawGeometry(brushe, null, geometry);
                }
            }
        }
    }
}

public class SoundSliceData
{

    public SoundSliceData() { }
    public SoundSliceData(float[] sums)
    {
        if (sums == null || sums.Length < 20)
            throw new ArgumentException("Array must contain at least 20 elements.");

        V1 = (int)sums[0];
        V2 = (int)sums[1];
        V3 = (int)sums[2];
        V4 = (int)sums[3];
        V5 = (int)sums[4];
        V6 = (int)sums[5];
        V7 = (int)sums[6];
        V8 = (int)sums[7];
        V9 = (int)sums[8];
        V10 = (int)sums[9];
        V11 = (int)sums[10];
        V12 = (int)sums[11];
        V13 = (int)sums[12];
        V14 = (int)sums[13];
        V15 = (int)sums[14];
        V16 = (int)sums[15];
        V17 = (int)sums[16];
        V18 = (int)sums[17];
        V19 = (int)sums[18];
        V20 = (int)sums[19];
    }

    public int V1 { get; }
    public int V2 { get; }
    public int V3 { get; }
    public int V4 { get; }
    public int V5 { get; }
    public int V6 { get; }
    public int V7 { get; }
    public int V8 { get; }
    public int V9 { get; }
    public int V10 { get; }
    public int V11 { get; }
    public int V12 { get; }
    public int V13 { get; }
    public int V14 { get; }
    public int V15 { get; }
    public int V16 { get; }
    public int V17 { get; }
    public int V18 { get; }
    public int V19 { get; }
    public int V20 { get; }
}
