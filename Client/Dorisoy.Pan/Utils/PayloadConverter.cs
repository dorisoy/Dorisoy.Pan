using Ellipse = Avalonia.Controls.Shapes.Ellipse;
using Point = Avalonia.Point;

namespace Dorisoy.Pan.Utils;


/// <summary>
/// Payload转换器
/// </summary>
public static class PayloadConverter
{
    public static byte[] ToBytes(double x, double y, ThicknessEnum size, ColorsEnum color)
    {
        var bytes = new byte[6];

        bytes[0] = (byte)x;
        bytes[1] = (byte)((short)x >> 8);

        bytes[2] = (byte)y;
        bytes[3] = (byte)((short)y >> 8);

        bytes[4] = (byte)size;
        bytes[5] = (byte)color;

        return bytes;
    }

    public static Ellipse ToPoint(byte[] bytes)
    {
        var x = (bytes[1] << 8) + bytes[0];
        var y = (bytes[3] << 8) + bytes[2];
        var size = BrushSettings.FindThickness(bytes[4]);
        var colorBrush = BrushSettings.FindColorBrush(bytes[5]);

        var ellipse = new Ellipse
        {
            Margin = new Thickness(x - (size / 2), y - (size / 2), 0, 0),
            Fill = colorBrush,
            Width = size,
            Height = size
        };

        return ellipse;
    }

    public static byte[] ToBytes(List<Point> points, ThicknessEnum thickness, ColorsEnum color)
    {
        var bytes = new byte[1 + points.Count * 4 + 2];
        int currentIndex = 0;

        bytes[currentIndex++] = (byte)points.Count;

        foreach (var point in points)
        {
            bytes[currentIndex++] = (byte)point.X;
            bytes[currentIndex++] = (byte)((short)point.X >> 8);

            bytes[currentIndex++] = (byte)point.Y;
            bytes[currentIndex++] = (byte)((short)point.Y >> 8);
        }

        bytes[currentIndex++] = (byte)thickness;
        bytes[currentIndex] = (byte)color;

        return bytes;
    }

    public static (Queue<Point> points, double thickness, SolidColorBrush colorBrush) ToLine(byte[] bytes)
    {
        int currentIndex = 0;
        var count = (int)bytes[currentIndex++];

        var result = new Queue<Point>(count);

        for (var i = 0; i < count; i++)
        {
            var buffer = i * 4;

            var x = (bytes[buffer + 2] << 8) + bytes[buffer + 1];
            var y = (bytes[buffer + 4] << 8) + bytes[buffer + 3];
            result.Enqueue(new Point(x, y));
        }

        var size = BrushSettings.FindThickness(bytes[^2]);
        var colorBrush = BrushSettings.FindColorBrush(bytes[^1]);

        return (result, size, colorBrush);
    }
}
