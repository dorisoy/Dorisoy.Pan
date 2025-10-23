namespace Dorisoy.Pan.Player;

public enum LibVLCAvaloniaRenderingOptions
{
    /// <summary>
    /// 使用传递给vlc的本机句柄，尽可能获得最佳性能
    /// </summary>
    VlcNative,
    /// <summary>
    /// 对图像使用默认的avalonia渲染
    /// </summary>
    Avalonia,
    /// <summary>
    /// 使用默认的avalonia渲染和自定义绘图操作，期望使用avalonia获得更好的性能
    /// </summary>
    AvaloniaCustomDrawingOperation
}

public class LibVLCAvaloniaOptions
{
    public static LibVLCAvaloniaRenderingOptions RenderingOptions { get; set; } = LibVLCAvaloniaRenderingOptions.VlcNative;
}
