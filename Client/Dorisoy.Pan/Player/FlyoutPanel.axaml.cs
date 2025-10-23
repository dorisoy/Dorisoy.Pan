namespace Dorisoy.Pan.Player;

/// <summary>
/// �Զ��嵯��ʽ���
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
