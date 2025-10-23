using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Color = Avalonia.Media.Color;
using Point = Avalonia.Point;
using Size = Avalonia.Size;

namespace Dorisoy.Pan.Controls;

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
            // �����ﴥ���ػ�
            InvalidateVisual();
        }
    }

    public SoundVisualizer()
    {
        InitializeComponent();
    }

    // ��дOnRender�������������������ͼ
    public override void Render(DrawingContext context)
    {
        base.Render(context);

        /*
        if (Data != null)
        {
            // �ٶ�����ͼ�����߶ȵ��ڿؼ��ĸ߶�
            double maxHeight = this.Bounds.Height;
            double maxWidth = this.Bounds.Width;

            // ѭ������V1��V20������ÿ��������
            double barWidth = maxWidth / 20;// �������Ŀ���
            for (int i = 0; i < 20; i++)
            {
                var valueProperty = Data.GetType().GetProperty($"V{i + 1}");
                if (valueProperty != null)
                {
                    int value = (int)valueProperty.GetValue(Data);
                    double barHeight = (value / 100.0) * maxHeight;// ����ֵ����0��100

                    // ������������λ�úʹ�С
                    double x = i * (barWidth + 2);// ����������֮��ļ�࣬������Ϊ2
                    double y = maxHeight - barHeight;// �ӵײ����ϻ���

                    // ���Ƶ���������
                    var rect = new Rect(x, y, barWidth, barHeight);

                    // ���û�ˢ��ɫΪ��ɫ
                    var brush = Brushes.Blue;


                    // ʹ��ָ���Ļ�ˢ������
                    context.FillRectangle(brush, rect);
                }
            }
        }
        */

        if (Data != null)
        {
            double maxHeight = this.Bounds.Height;
            double maxWidth = this.Bounds.Width;

            double spacing = 2;   // ������֮��ļ��
            double barWidth = (maxWidth / 20) - 2;  // �������Ŀ���
            double cornerRadius = 4;// Բ�ǰ뾶


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
                        // ��ʼ�������½�
                        ctx.BeginFigure(new Point(x, maxHeight), true);

                        // ֱ�ߵ����Ͻǣ�Բ�ǵĿ�ʼ��
                        ctx.LineTo(new Point(x, y + cornerRadius));

                        // ����Բ�� - ���Ͻ�
                        ctx.ArcTo(
                            new Point(x + cornerRadius, y),
                            new Size(cornerRadius, cornerRadius),
                            90,
                            false,
                            SweepDirection.Clockwise

                        );

                        // ֱ�ߵ����Ͻǣ�Բ�ǵĽ�����
                        ctx.LineTo(new Point(x + barWidth - cornerRadius, y));

                        // ����Բ�� - ���Ͻ�
                        ctx.ArcTo(
                            new Point(x + barWidth, y + cornerRadius),
                            new Size(cornerRadius, cornerRadius),
                            90,
                            false,
                            SweepDirection.Clockwise
                        );

                        // ֱ�ߵ����½�
                        ctx.LineTo(new Point(x + barWidth, maxHeight));

                        // ��ɸ���״�ıպ�
                        ctx.LineTo(new Point(x, maxHeight));
                    }

                    // ���û�ˢ��ɫΪ��ɫ
                    var brushe = new SolidColorBrush(Color.Parse("#00aaeb"));

                    // ʹ��ָ���Ļ�ˢ���Բ�Ǿ���
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
