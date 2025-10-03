﻿using Avalonia;
using Avalonia.Controls;
using FluentAvalonia.Core;

namespace FluentAvalonia.UI.Controls.Primitives;

/// <summary>
/// Represents a button in a TaskDialog
/// </summary>
/// <remarks>
/// This type should not be used directly and is generated automatically
/// by a TaskDialog
/// </remarks>
public class TaskDialogButtonHost : Button
{
    public static readonly StyledProperty<IconSource> IconSourceProperty =
        SettingsExpander.IconSourceProperty.AddOwner<TaskDialogButtonHost>();

    public IconSource IconSource
    {
        get => GetValue(IconSourceProperty);
        set => SetValue(IconSourceProperty, value);
    }

    protected override void OnClick()
    {
        base.OnClick();

        if (DataContext is TaskDialogButton tdb)
        {
            tdb.RaiseClick();
        }
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);

        if (change.Property == IconSourceProperty)
        {
            PseudoClasses.Set(SharedPseudoclasses.s_pcIcon, change.NewValue != null);
        }
    }
}
