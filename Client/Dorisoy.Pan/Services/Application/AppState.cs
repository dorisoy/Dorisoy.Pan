namespace Dorisoy.PanClient.Services;

public interface IAppState
{
    string Nickname { get; set; }
    string Room { get; set; }
    BrushSettings BrushSettings { get; }
    bool IsWindowMaximized { get; set; }
    bool IsInitialized { get; set; }
}

public class AppState : IAppState
{
    public AppState()
    {
        BrushSettings = new("avares://Dorisoy.PanClient/Assets/Images/Cursors");
    }

    public string Nickname { get; set; }

    public string Room { get; set; }

    /// <summary>
    /// 画笔设置
    /// </summary>
    public BrushSettings BrushSettings { get; }

    public bool IsWindowMaximized { get; set; }

    public bool IsInitialized { get; set; }
}
