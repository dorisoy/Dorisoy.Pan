﻿using Avalonia.Data;
using Avalonia;
using Avalonia.Media;
using Avalonia.Collections;
using Avalonia.Controls;
using FluentAvalonia.Core;
using Avalonia.Controls.Metadata;

namespace FluentAvalonia.UI.Controls;

[TemplatePart(s_tpShowFlyoutButton, typeof(Button))]
public partial class ColorPickerButton
{
    /// <summary>
    /// Defines the <see cref="Color"/> property
    /// </summary>
    public static readonly StyledProperty<Color?> ColorProperty =
        AvaloniaProperty.Register<ColorPickerButton, Color?>(nameof(Color),
            defaultBindingMode: BindingMode.TwoWay);

    /// <summary>
    /// Defines the <see cref="IsMoreButtonVisible"/> property
    /// </summary>
    public static readonly StyledProperty<bool> IsMoreButtonVisibleProperty =
        FAColorPicker.IsMoreButtonVisibleProperty.AddOwner<ColorPickerButton>();

    /// <summary>
    /// Defines the <see cref="IsCompact"/> property
    /// </summary>
    public static readonly StyledProperty<bool> IsCompactProperty =
        FAColorPicker.IsCompactProperty.AddOwner<ColorPickerButton>();

    /// <summary>
    /// Defines the <see cref="IsAlphaEnabled"/> property
    /// </summary>
    public static readonly StyledProperty<bool> IsAlphaEnabledProperty =
        FAColorPicker.IsAlphaEnabledProperty.AddOwner<ColorPickerButton>();

    /// <summary>
    /// Defines the <see cref="UseSpectrum"/> property
    /// </summary>
    public static readonly StyledProperty<bool> UseSpectrumProperty =
        FAColorPicker.UseSpectrumProperty.AddOwner<ColorPickerButton>();

    /// <summary>
    /// Defines the <see cref="UseColorWheel"/> property
    /// </summary>
    public static readonly StyledProperty<bool> UseColorWheelProperty =
        FAColorPicker.UseColorWheelProperty.AddOwner<ColorPickerButton>();

    /// <summary>
    /// Defines the <see cref="UseColorTriangle"/> property
    /// </summary>
    public static readonly StyledProperty<bool> UseColorTriangleProperty =
        FAColorPicker.UseColorTriangleProperty.AddOwner<ColorPickerButton>();

    /// <summary>
    /// Defines the <see cref="UseColorPalette"/> property
    /// </summary>
    public static readonly StyledProperty<bool> UseColorPaletteProperty =
        FAColorPicker.UseColorPaletteProperty.AddOwner<ColorPickerButton>();

    /// <summary>
    /// Defines the <see cref="CustomPaletteColors"/> property
    /// </summary>
    public static readonly DirectProperty<ColorPickerButton, IEnumerable<Color>> CustomPaletteColorsProperty =
        FAColorPicker.CustomPaletteColorsProperty.AddOwner<ColorPickerButton>(x => x.CustomPaletteColors,
            (x, v) => x.CustomPaletteColors = v);

    /// <summary>
    /// Define sthe <see cref="PaletteColumnCount"/> property
    /// </summary>
    public static readonly StyledProperty<int> PaletteColumnCountProperty =
        FAColorPicker.PaletteColumnCountProperty.AddOwner<ColorPickerButton>();

    /// <summary>
    /// Defines the <see cref="ShowAcceptDismissButtons"/> property
    /// </summary>
    public static readonly StyledProperty<bool> ShowAcceptDismissButtonsProperty =
        AvaloniaProperty.Register<ColorPickerButton, bool>(nameof(ShowAcceptDismissButtons), defaultValue: true);

    /// <summary>
    /// Defines the <see cref="FlyoutPlacement"/> property
    /// </summary>
    public static readonly StyledProperty<PlacementMode> FlyoutPlacementProperty =
        AvaloniaProperty.Register<ColorPickerButton, PlacementMode>(nameof(FlyoutPlacement),
            defaultValue: PlacementMode.Bottom);


    /// <summary>
    /// Gets or sets the current <see cref="Avalonia.Media.Color"/> of the ColorPickerButton
    /// </summary>
    public Color? Color
    {
        get => GetValue(ColorProperty);
        set => SetValue(ColorProperty, value);
    }

    /// <summary>
    /// Gets or sets whether the More button is visible in the <see cref="FAColorPicker"/>
    /// </summary>
    public bool IsMoreButtonVisible
    {
        get => GetValue(IsMoreButtonVisibleProperty);
        set => SetValue(IsMoreButtonVisibleProperty, value);
    }

    /// <summary>
    /// Gets or sets whether the <see cref="FAColorPicker"/> is in a compact state
    /// </summary>
    public bool IsCompact
    {
        get => GetValue(IsCompactProperty);
        set => SetValue(IsCompactProperty, value);
    }

    /// <summary>
    /// Gets or sets whether the user can edit the alpha in the <see cref="FAColorPicker"/>
    /// </summary>
    public bool IsAlphaEnabled
    {
        get => GetValue(IsAlphaEnabledProperty);
        set => SetValue(IsAlphaEnabledProperty, value);
    }

    /// <summary>
    /// Gets or sets whether the <see cref="FAColorPicker"/> should allow using the 
    /// Color Spectrum display for selecting a color
    /// </summary>
    public bool UseSpectrum
    {
        get => GetValue(UseSpectrumProperty);
        set => SetValue(UseSpectrumProperty, value);
    }

    /// <summary>
    /// Gets or sets whether the <see cref="FAColorPicker"/> should allow using the
    /// Color Wheel display for selecting a color
    /// </summary>
    public bool UseColorWheel
    {
        get => GetValue(UseColorWheelProperty);
        set => SetValue(UseColorWheelProperty, value);
    }

    /// <summary>
    /// Gets or sets whether the <see cref="FAColorPicker"/> should allow using the 
    /// HSV Color Triangle display for selecting a color
    /// </summary>
    public bool UseColorTriangle
    {
        get => GetValue(UseColorTriangleProperty);
        set => SetValue(UseColorTriangleProperty, value);
    }

    /// <summary>
    /// Gets or sets whether the <see cref="FAColorPicker"/> should allow showing the
    /// color palette of pre-defined colors
    /// </summary>
    public bool UseColorPalette
    {
        get => GetValue(UseColorPaletteProperty);
        set => SetValue(UseColorPaletteProperty, value);
    }

    /// <summary>
    /// Gets or sets a collection of colors to be used in the Custom Color Palette display
    /// of the <see cref="FAColorPicker"/>
    /// </summary>
    public IEnumerable<Color> CustomPaletteColors
    {
        get => _customPaletteColors ?? (CustomPaletteColors = new AvaloniaList<Color>());
        set => SetAndRaise(CustomPaletteColorsProperty, ref _customPaletteColors, value);
    }

    /// <summary>
    /// Gets or sets the number of columns to use in the Custom Color Palette display 
    /// of the <see cref="FAColorPicker"/>
    /// </summary>
    public int PaletteColumnCount
    {
        get => GetValue(PaletteColumnCountProperty);
        set => SetValue(PaletteColumnCountProperty, value);
    }

    /// <summary>
    /// Gets or sets whether the Flyout should show the Accept and Dismiss buttons. If true,
    /// changes to the color are only applied if accept is pressed. If false, changes to the 
    /// color apply immediately.
    /// </summary>
    public bool ShowAcceptDismissButtons
    {
        get => GetValue(ShowAcceptDismissButtonsProperty);
        set => SetValue(ShowAcceptDismissButtonsProperty, value);
    }

    /// <summary>
    /// Gets or sets the placement for the color picker flyout
    /// </summary>
    public PlacementMode FlyoutPlacement
    {
        get => GetValue(FlyoutPlacementProperty);
        set => SetValue(FlyoutPlacementProperty, value);
    }

    /// <summary>
    /// Raised when the color change was confirmed and the flyout closes.
    /// </summary>
    public event TypedEventHandler<ColorPickerButton, ColorButtonColorChangedEventArgs> FlyoutConfirmed;

    /// <summary>
    /// Raised when the color change was dismissed and the flyout closes.
    /// </summary>
    public event TypedEventHandler<ColorPickerButton, EventArgs> FlyoutDismissed;

    /// <summary> Raised when the flyout opens.
    /// </summary>
    public event TypedEventHandler<ColorPickerButton, EventArgs> FlyoutOpened;

    /// <summary>
    /// Raised when the flyout closes regardless of confirmation or dismissal.
    /// </summary>
    public event TypedEventHandler<ColorPickerButton, EventArgs> FlyoutClosed;

    /// <summary>
    /// Fired when the current <see cref="Color"/> property changes
    /// </summary>
    public event TypedEventHandler<ColorPickerButton, ColorButtonColorChangedEventArgs> ColorChanged;

    private IEnumerable<Color> _customPaletteColors;
    private const string s_tpShowFlyoutButton = "ShowFlyoutButton";
}
