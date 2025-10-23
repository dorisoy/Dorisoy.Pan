using Avalonia.Controls.Primitives;
using FluentAvalonia.Core;

namespace Dorisoy.Pan.Controls;

public class ControlBoxSubstitution : AvaloniaObject
{
    public static readonly DirectProperty<ControlBoxSubstitution, bool> IsEnabledProperty =
        AvaloniaProperty.RegisterDirect<ControlBoxSubstitution, bool>(nameof(IsEnabled),
             x => x.IsEnabled, (x, v) => x.IsEnabled = v);

    public static readonly DirectProperty<ControlBoxSubstitution, object> ValueProperty =
        AvaloniaProperty.RegisterDirect<ControlBoxSubstitution, object>(nameof(Value),
            x => x.Value, (x, v) => x.Value = v);


    public string Key { get; set; }

    private object _value = null;
    public object Value
    {
        get => _value;
        set
        {
            SetAndRaise(ValueProperty, ref _value, value);
            ValueChanged?.Invoke(this, null);
        }
    }

    private bool _enabled = true;
    public bool IsEnabled
    {
        get => _enabled;
        set
        {
            SetAndRaise(IsEnabledProperty, ref _enabled, value);
            ValueChanged?.Invoke(this, null);
        }
    }

    public event TypedEventHandler<ControlBoxSubstitution, object> ValueChanged;

    public string ValueAsString()
    {
        if (!IsEnabled)
        {
            return string.Empty;
        }

        object value = Value;

        if (value is SolidColorBrush brush)
        {
            value = brush.Color;
        }

        if (value == null)
        {
            return string.Empty;
        }

        return value.ToString();
    }
}

public class ControlBox : HeaderedContentControl
{

    private Button _expandedButtonButton;

    public ControlBox()
    {
        PseudoClasses.Add(":optionsfull");
    }

    public static readonly StyledProperty<Type> TargetTypeProperty =
        AvaloniaProperty.Register<ControlBox, Type>(nameof(TargetType));

    public bool EnableShowExpandedButton { get; set; }


    /// <summary>
    /// 工具栏子控件
    /// </summary>
    public Control ToolBars
    {
        get => GetValue(ToolBarsProperty);
        set => SetValue(ToolBarsProperty, value);
    }


    public static readonly StyledProperty<Control> ToolBarsProperty =
      AvaloniaProperty.Register<ControlBox, Control>(nameof(ToolBars));


    public bool IsToolBar
    {
        get => GetValue(IsToolBarProperty);
        set => SetValue(IsToolBarProperty, value);
    }
    public static readonly StyledProperty<bool> IsToolBarProperty = AvaloniaProperty.Register<ControlBox, bool>(nameof(IsToolBar), true);


    public Type TargetType
    {
        get => GetValue(TargetTypeProperty);
        set => SetValue(TargetTypeProperty, value);
    }


    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {

        base.OnApplyTemplate(e);

        _expandedButtonButton = e.NameScope.Find<Button>("ExpandedButton");

        if (!EnableShowExpandedButton)
        {
            _expandedButtonButton.IsVisible = false;
        }
    }


    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);

    }
}
