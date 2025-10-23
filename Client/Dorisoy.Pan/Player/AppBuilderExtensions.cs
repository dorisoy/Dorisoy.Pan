using Path = System.IO.Path;
using LibVLCSharp.Shared;

namespace Dorisoy.Pan.Player;

public static class AppBuilderExtensions
{
    /// <summary>
    ///  使用LibVLCSharp
    /// </summary>
    public static AppBuilder UseVLCSharp(this AppBuilder b, LibVLCAvaloniaRenderingOptions? renderingOptions = null, string libvlcDirectoryPath = null)
    {
        if (renderingOptions != null)
            LibVLCAvaloniaOptions.RenderingOptions = renderingOptions.Value;

        var libVlcDirectoryPath = Path.Combine(Environment.CurrentDirectory, "libvlc", Utilities.IsWin64() ? "win-x64" : "win-x86");

        return b.AfterSetup(_ => LibVLCSharp.Shared.Core.Initialize(libVlcDirectoryPath));
    }
}
