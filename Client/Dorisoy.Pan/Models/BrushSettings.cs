using Color = Avalonia.Media.Color;

namespace Dorisoy.Pan.Models;

/// <summary>
/// 画笔设置
/// </summary>
public class BrushSettings
{
    private static readonly Dictionary<ColorsEnum, SolidColorBrush> ColorLookup = new()
    {
        { ColorsEnum.Default, new SolidColorBrush(Color.FromRgb(255, 127, 39))  },
        { ColorsEnum.Eraser, new SolidColorBrush(Color.FromRgb(255, 255, 255)) },
        { ColorsEnum.Red, new SolidColorBrush(Color.FromRgb(235, 51, 36)) },
        { ColorsEnum.Blue, new SolidColorBrush(Color.FromRgb(0, 162, 232)) },
        { ColorsEnum.Green, new SolidColorBrush(Color.FromRgb(34, 177, 76)) },
        { ColorsEnum.Yellow, new SolidColorBrush(Color.FromRgb(255, 242, 0)) },
        { ColorsEnum.Orange, new SolidColorBrush(Color.FromRgb(255, 127, 39)) },
        { ColorsEnum.Purple, new SolidColorBrush(Color.FromRgb(163, 73, 164)) },
        { ColorsEnum.Pink, new SolidColorBrush(Color.FromRgb(255, 174, 201)) },
        { ColorsEnum.Gray, new SolidColorBrush(Color.FromRgb(195, 195, 195)) }
    };
    private static readonly Dictionary<ThicknessEnum, double> ThicknessLookup = new()
    {
        { ThicknessEnum.Thin, 2 },
        { ThicknessEnum.SemiThin, 4 },
        { ThicknessEnum.Medium, 6 },
        { ThicknessEnum.SemiThick, 8 },
        { ThicknessEnum.Thick, 10 },
        { ThicknessEnum.Eraser, 50 }
    };
    private readonly string _cursorsPath;
    private readonly Dictionary<ColorsEnum, string> _cursorBrushPathLookup;

    public BrushSettings(string cursorsPath)
    {
        _cursorsPath = cursorsPath ?? throw new ArgumentException("Argument required.", nameof(cursorsPath));

        if (_cursorsPath != string.Empty)
        {
            _cursorBrushPathLookup = new()
            {
                { ColorsEnum.Default, $"{_cursorsPath}/brush-black.png" },
                { ColorsEnum.Red, $"{_cursorsPath}/brush-red.png" },
                { ColorsEnum.Blue, $"{_cursorsPath}/brush-blue.png" },
                { ColorsEnum.Green, $"{_cursorsPath}/brush-green.png" },
                { ColorsEnum.Yellow, $"{_cursorsPath}/brush-yellow.png" },
                { ColorsEnum.Orange, $"{_cursorsPath}/brush-orange.png" },
                { ColorsEnum.Purple, $"{_cursorsPath}/brush-purple.png" },
                { ColorsEnum.Pink, $"{_cursorsPath}/brush-pink.png" },
                { ColorsEnum.Gray, $"{_cursorsPath}/brush-gray.png" }
            };
        }

        BrushColor = ColorsEnum.Default;
        BrushThickness = ThicknessEnum.SemiThin;
    }

    public event EventHandler<BrushChangedEventArgs> BrushChanged;

    public Cursor Cursor { get; private set; }
    public bool IsTeam { get; set; } = false;

    private ColorsEnum brushColor;
    public ColorsEnum BrushColor
    {
        get => brushColor;
        set
        {
            brushColor = value;
            ColorBrush = ColorLookup[value];

            if (string.IsNullOrEmpty(_cursorsPath))
            {
                return;
            }

            if (value == ColorsEnum.Eraser)
            {
                var path = $"{_cursorsPath}/eraser-{Thickness}.png";
                if (!string.IsNullOrEmpty(path))
                {
                    using (var st = AssetLoader.Open(new Uri(path)))
                    {
                        Cursor = new(new Bitmap(st),
                            new PixelPoint((int)HalfThickness,
                            (int)HalfThickness));
                    };
                }

            }
            else
            {
                var path = _cursorBrushPathLookup[value];
                if (!string.IsNullOrEmpty(path))
                {
                    //Unable to locate 'Avalonia.Platform.IAssetLoader'.”
                    using (var st = AssetLoader.Open(new Uri(path)))
                    {
                        Cursor = new(new Bitmap(st),
                            new PixelPoint(0, 0));
                    };
                }
            }

            BrushChanged?.Invoke(null, new BrushChangedEventArgs(Cursor));
        }
    }

    public SolidColorBrush ColorBrush { get; private set; }

    private ThicknessEnum brushThickness;
    public ThicknessEnum BrushThickness
    {
        get => brushThickness;
        set
        {
            brushThickness = value;
            Thickness = ThicknessLookup[value];
            HalfThickness = Thickness / 2;

            MaxBrushPointX = IsTeam ? Globals.ChatCanvasWidth : Globals.CanvasWidth - HalfThickness;
            MaxBrushPointY = IsTeam ? Globals.ChatCanvasHeight : Globals.CanvasHeight - HalfThickness;

            MinBrushPoint = HalfThickness;

            if (brushColor == ColorsEnum.Eraser)
            {
                var path = $"{_cursorsPath}/eraser-{Thickness}.png";

                if (!string.IsNullOrEmpty(path))
                {
                    using (var st = AssetLoader.Open(new Uri(path)))
                    {
                        Cursor = new(new Bitmap(st),
                            new PixelPoint((int)HalfThickness,
                            (int)HalfThickness));
                    };
                }
            }

            BrushChanged?.Invoke(null, new BrushChangedEventArgs(Cursor));
        }
    }

    public double Thickness { get; private set; }
    public double HalfThickness { get; private set; }

    /// <summary>
    /// 最大笔刷X坐标
    /// </summary>
    public double MaxBrushPointX { get; private set; }

    /// <summary>
    /// 最大笔刷Y坐标
    /// </summary>
    public double MaxBrushPointY { get; private set; }

    /// <summary>
    /// 最小笔刷点
    /// </summary>
    public double MinBrushPoint { get; private set; }


    public void UpdateBrushPoint()
    {
        MaxBrushPointX = IsTeam ? Globals.ChatCanvasWidth : Globals.CanvasWidth - HalfThickness;
        MaxBrushPointY = IsTeam ? Globals.ChatCanvasHeight : Globals.CanvasHeight - HalfThickness;
    }


    public static SolidColorBrush FindColorBrush(byte color)
    {
        return ColorLookup[(ColorsEnum)color];
    }

    public static double FindThickness(byte thickness)
    {
        return ThicknessLookup[(ThicknessEnum)thickness];
    }


}

public class BrushChangedEventArgs : EventArgs
{
    public BrushChangedEventArgs(Cursor cursor)
    {
        Cursor = cursor;
    }
    public Cursor Cursor { get; private set; }
}
