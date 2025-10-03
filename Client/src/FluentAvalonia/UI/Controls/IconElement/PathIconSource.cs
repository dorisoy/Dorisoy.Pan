﻿using Avalonia;
using Avalonia.Media;

namespace FluentAvalonia.UI.Controls;

/// <summary>
/// Represents an icon source that uses a vector path as its content.
/// </summary>
public class PathIconSource : IconSource
{
    static PathIconSource()
    {
        StretchProperty.OverrideDefaultValue<PathIconSource>(Stretch.Uniform);
        StretchDirectionProperty.OverrideDefaultValue<PathIconSource>(StretchDirection.Both);
    }

    /// <summary>
    /// Defines the <see cref="Data"/> property
    /// </summary>
    public static readonly StyledProperty<Geometry> DataProperty =
        FAPathIcon.DataProperty.AddOwner<PathIconSource>();

    /// <summary>
    /// Gets or sets a Geometry that specifies the shape to be drawn. 
    /// In XAML. this can also be set using a string that describes Move and draw commands syntax.
    /// </summary>
    public Geometry Data
    {
        get => GetValue(DataProperty);
        set => SetValue(DataProperty, value);
    }

    /// <summary>
    /// Defines the <see cref="Stretch"/> property.
    /// </summary>
    public static readonly StyledProperty<Stretch> StretchProperty =
        FAPathIcon.StretchProperty.AddOwner<FAPathIcon>();

    /// <summary>
    /// Gets or sets a <see cref="Stretch"/> enumeration value that describes how the shape fills its allocated space.
    /// </summary>
    public Stretch Stretch
    {
        get => GetValue(StretchProperty);
        set => SetValue(StretchProperty, value);
    }

    /// <summary>
    /// Defines the <see cref="StretchDirection"/> property.
    /// </summary>
    public static readonly StyledProperty<StretchDirection> StretchDirectionProperty =
        FAPathIcon.StretchDirectionProperty.AddOwner<Avalonia.Controls.PathIcon>();

    /// <summary>
    /// Gets or sets a value controlling in what direction contents will be stretched.
    /// </summary>
    public StretchDirection StretchDirection
    {
        get => GetValue(StretchDirectionProperty);
        set => SetValue(StretchDirectionProperty, value);
    }
}
