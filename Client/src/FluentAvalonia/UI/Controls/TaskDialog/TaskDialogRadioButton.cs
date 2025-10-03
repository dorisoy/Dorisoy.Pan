﻿using Avalonia;
using Avalonia.Controls.Primitives;

namespace FluentAvalonia.UI.Controls;

/// <summary>
/// Represents a RadioButton in a <see cref="TaskDialog"/>
/// </summary>
public class TaskDialogRadioButton : TaskDialogCommand
{
    /// <summary>
    /// Defines the <see cref="IsChecked"/> property
    /// </summary>
    public static readonly StyledProperty<bool?> IsCheckedProperty =
        ToggleButton.IsCheckedProperty.AddOwner<TaskDialogRadioButton>();

    /// <summary>
    /// Gets or sets whether this RadioButton is checked
    /// </summary>
    public bool? IsChecked
    {
        get => GetValue(IsCheckedProperty);
        set => SetValue(IsCheckedProperty, value);
    }
}
