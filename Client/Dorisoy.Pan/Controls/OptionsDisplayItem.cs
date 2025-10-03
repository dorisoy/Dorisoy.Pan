using Avalonia.Controls.Primitives;
using Avalonia.Layout;

namespace Dorisoy.PanClient.Controls;

public class OptionsDisplayItem : TemplatedControl
{
    public static readonly StyledProperty<string> HeaderProperty =
        AvaloniaProperty.Register<OptionsDisplayItem, string>(nameof(Header));

    public static readonly StyledProperty<string> DescriptionProperty =
        AvaloniaProperty.Register<OptionsDisplayItem, string>(nameof(Description));

    public static readonly StyledProperty<HorizontalAlignment> DescriptionHorizontalAlignmentProperty =
      AvaloniaProperty.Register<OptionsDisplayItem, HorizontalAlignment>(nameof(DescriptionHorizontalAlignment), HorizontalAlignment.Left);

    public static readonly StyledProperty<HorizontalAlignment> HeaderHorizontalAlignmentProperty =
       AvaloniaProperty.Register<OptionsDisplayItem, HorizontalAlignment>(nameof(HeaderHorizontalAlignment), HorizontalAlignment.Left);

    public static readonly StyledProperty<IconElement> IconProperty =
        AvaloniaProperty.Register<OptionsDisplayItem, IconElement>(nameof(Icon));

    public static readonly StyledProperty<bool> NavigatesProperty =
        AvaloniaProperty.Register<OptionsDisplayItem, bool>(nameof(Navigates));

    public static readonly StyledProperty<Control> ActionButtonProperty =
        AvaloniaProperty.Register<OptionsDisplayItem, Control>(nameof(ActionButton));

    public static readonly StyledProperty<bool> ExpandsProperty =
        AvaloniaProperty.Register<OptionsDisplayItem, bool>(nameof(Expands));

    public static readonly StyledProperty<object> ContentProperty =
        ContentControl.ContentProperty.AddOwner<OptionsDisplayItem>();

    //public static readonly DirectProperty<OptionsDisplayItem, bool> IsExpandedProperty =
    //    Expander.IsExpandedProperty.AddOwner<OptionsDisplayItem>(x => x.IsExpanded, (x, v) => x.IsExpanded = v);

    public static readonly StyledProperty<ICommand> NavigationCommandProperty =
        AvaloniaProperty.Register<OptionsDisplayItem, ICommand>(nameof(NavigationCommand));


    public static readonly StyledProperty<int> HeaderSizeProperty =
      AvaloniaProperty.Register<OptionsDisplayItem, int>(nameof(HeaderSize), 12);

    public static readonly StyledProperty<int> DescriptionSizeProperty =
      AvaloniaProperty.Register<OptionsDisplayItem, int>(nameof(DescriptionSize), 12);

    public HorizontalAlignment DescriptionHorizontalAlignment
    {
        get => GetValue(DescriptionHorizontalAlignmentProperty);
        set => SetValue(DescriptionHorizontalAlignmentProperty, value);
    }

    public HorizontalAlignment HeaderHorizontalAlignment
    {
        get => GetValue(HeaderHorizontalAlignmentProperty);
        set => SetValue(HeaderHorizontalAlignmentProperty, value);
    }


    public int HeaderSize
    {
        get => GetValue(HeaderSizeProperty);
        set => SetValue(HeaderSizeProperty, value);
    }
    public int DescriptionSize
    {
        get => GetValue(DescriptionSizeProperty);
        set => SetValue(DescriptionSizeProperty, value);
    }

    public string Header
    {
        get => GetValue(HeaderProperty);
        set => SetValue(HeaderProperty, value);
    }

    public string Description
    {
        get => GetValue(DescriptionProperty);
        set => SetValue(DescriptionProperty, value);
    }

    public IconElement Icon
    {
        get => GetValue(IconProperty);
        set => SetValue(IconProperty, value);
    }

    public bool Navigates
    {
        get => GetValue(NavigatesProperty);
        set => SetValue(NavigatesProperty, value);
    }

    public Control ActionButton
    {
        get => GetValue(ActionButtonProperty);
        set => SetValue(ActionButtonProperty, value);
    }

    public bool Expands
    {
        get => GetValue(ExpandsProperty);
        set => SetValue(ExpandsProperty, value);
    }

    public object Content
    {
        get => GetValue(ContentProperty);
        set => SetValue(ContentProperty, value);
    }

    //public bool IsExpanded
    //{
    //    get => _isExpanded;
    //    set => SetAndRaise(IsExpandedProperty, ref _isExpanded, value);
    //}

    public ICommand NavigationCommand
    {
        get => GetValue(NavigationCommandProperty);
        set => SetValue(NavigationCommandProperty, value);
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);

        if (change.Property == NavigatesProperty)
        {
            if (Expands)
                throw new InvalidOperationException("Control cannot both Navigate and Expand");

            PseudoClasses.Set(":navigates", change.NewValue != null);
        }
        else if (change.Property == ExpandsProperty)
        {
            if (Navigates)
                throw new InvalidOperationException("Control cannot both Navigate and Expand");

            PseudoClasses.Set(":expands", change.NewValue != null);
        }
        //else if (change.Property == IsExpandedProperty)
        //{
        //    PseudoClasses.Set(":expanded", change.NewValue != null);
        //}
        else if (change.Property == IconProperty)
        {
            PseudoClasses.Set(":icon", change.NewValue != null);
        }
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);

        _layoutRoot = e.NameScope.Find<Border>("LayoutRoot");
        _layoutRoot.PointerPressed += OnLayoutRootPointerPressed;
        _layoutRoot.PointerReleased += OnLayoutRootPointerReleased;
        _layoutRoot.PointerCaptureLost += OnLayoutRootPointerCaptureLost;
    }

    private void OnLayoutRootPointerPressed(object sender, PointerPressedEventArgs e)
    {
        if (e.GetCurrentPoint(this).Properties.PointerUpdateKind == PointerUpdateKind.LeftButtonPressed)
        {
            _isPressed = true;
            PseudoClasses.Set(":pressed", true);
        }
    }

    private void OnLayoutRootPointerReleased(object sender, PointerReleasedEventArgs e)
    {
        var pt = e.GetCurrentPoint(this);
        if (_isPressed && pt.Properties.PointerUpdateKind == PointerUpdateKind.LeftButtonReleased)
        {
            _isPressed = false;

            PseudoClasses.Set(":pressed", false);

            //if (Expands)
            //    IsExpanded = !IsExpanded;

            if (Navigates)
            {
                //RaiseEvent(new RoutedEventArgs(NavigationRequestedEvent, this));
                NavigationCommand?.Execute(null);
            }
        }
    }

    private void OnLayoutRootPointerCaptureLost(object sender, PointerCaptureLostEventArgs e)
    {
        _isPressed = false;
        PseudoClasses.Set(":pressed", false);
    }

    private bool _isPressed;
    private bool _isExpanded;
    private Border _layoutRoot;
}
