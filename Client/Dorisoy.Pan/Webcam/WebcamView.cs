using Image = Avalonia.Controls.Image;

namespace Dorisoy.PanClient.Webcam;

/// <summary>
/// 自定义网络摄像机
/// </summary>
[SupportedOSPlatform("windows")]
public class WebcamView : Image
{
    public static readonly DirectProperty<WebcamView, WebcamStreamingPlayer> WebcamStreamingPlayerProperty =
        AvaloniaProperty.RegisterDirect<WebcamView, WebcamStreamingPlayer>(nameof(WebcamStreamingPlayer), v => v.WebcamStreamingPlayer, (s, v) => s.WebcamStreamingPlayer = v);
    private WebcamStreamingPlayer _WebcamStreamingPlayer;
    public WebcamStreamingPlayer WebcamStreamingPlayer
    {
        get => _WebcamStreamingPlayer;
        set => SetAndRaise(WebcamStreamingPlayerProperty, ref _WebcamStreamingPlayer, value);
    }


    public static readonly DirectProperty<WebcamView, bool> DisplayRenderStatsProperty =
         AvaloniaProperty.RegisterDirect<WebcamView, bool>(nameof(DisplayRenderStats), v => v.DisplayRenderStats, (s, v) => s.DisplayRenderStats = v);
    private bool _displayRenderStats;
    public bool DisplayRenderStats
    {
        get => _displayRenderStats;
        set => SetAndRaise(DisplayRenderStatsProperty, ref _displayRenderStats, value);
    }
}
