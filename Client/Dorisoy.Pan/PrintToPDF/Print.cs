using Avalonia.Skia;
using AvaloniaEdit.Utils;

namespace Dorisoy.Pan.PrintToPDF;

public static class Print
{
    public static void ToFile(string fileName, params Visual[] visuals) => ToFile(fileName, visuals.AsEnumerable());
    public static bool ToFile(string fileName, IEnumerable<Visual> visuals)
    {
        try
        {
            if (visuals != null && visuals.Any())
            {
                foreach (var visual in visuals)
                {
                    var bounds = visual.Bounds;
                    var pixelSize = new PixelSize((int)bounds.Width, (int)bounds.Height);

                    //�ı�ģʽ Antialias
                    RenderOptions.SetTextRenderingMode(visual, TextRenderingMode.Antialias);

                    using var renderBitmap = new RenderTargetBitmap(pixelSize, SkiaPlatform.DefaultDpi);
                    renderBitmap.Render(visual);
                    renderBitmap.Save(fileName, 100);
                }
            }
            return true;
        }
        catch (Exception ex)
        {
            return false;
        }
    }
}

