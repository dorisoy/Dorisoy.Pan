namespace Dorisoy.PanClient.Player;

/// <summary>
/// 自定义弹出式面板
/// </summary>
public class FlyoutPanel : Panel
{
    public static readonly StyledProperty<bool> IsOpenProperty = AvaloniaProperty.Register<FlyoutPanel, bool>(name: "IsOpen");
    public bool IsOpen
    { get => GetValue(IsOpenProperty); set => SetValue(IsOpenProperty, value); }

    public FlyoutPanel()
    {

    }
}
