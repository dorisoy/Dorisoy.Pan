﻿using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Presenters;
using Avalonia.Styling;
using FluentAvalonia.Core;

namespace FluentAvalonia.UI.Controls;

/// <summary>
/// Represents a container that allows an element that doesn't implement ICommandBarElement 
/// to be displayed in a command bar.
/// </summary>
[PseudoClasses(SharedPseudoclasses.s_pcOverflow)]
public class CommandBarElementContainer : ContentControl, ICommandBarElement
{
    protected override Type StyleKeyOverride => typeof(CommandBarElementContainer);

    /// <summary>
    /// Defines the <see cref="IsInOverflow"/> property
    /// </summary>
    public static readonly DirectProperty<CommandBarElementContainer, bool> IsInOverflowProperty =
        AvaloniaProperty.RegisterDirect<CommandBarElementContainer, bool>(nameof(IsInOverflow),
                x => x.IsInOverflow);

    /// <summary>
    /// Defines the <see cref="DynamicOverflowOrder"/> property
    /// </summary>
    public static readonly DirectProperty<CommandBarElementContainer, int> DynamicOverflowOrderProperty =
        AvaloniaProperty.RegisterDirect<CommandBarElementContainer, int>(nameof(DynamicOverflowOrder),
            x => x.DynamicOverflowOrder, (x, v) => x.DynamicOverflowOrder = v);

    /// <summary>
    /// Defines the <see cref="IsCompact"/> property
    /// </summary>
    public static readonly StyledProperty<bool> IsCompactProperty =
        AvaloniaProperty.Register<CommandBarElementContainer, bool>(nameof(IsCompact));

    public bool IsCompact
    {
        get => GetValue(IsCompactProperty);
        set => SetValue(IsCompactProperty, value);
    }

    public bool IsInOverflow
    {
        get => _isInOverflow;
        internal set
        {
            if (SetAndRaise(IsInOverflowProperty, ref _isInOverflow, value))
            {
                PseudoClasses.Set(SharedPseudoclasses.s_pcOverflow, value);
            }
        }
    }

    public int DynamicOverflowOrder
    {
        get => _dynamicOverflowOrder;
        set => SetAndRaise(DynamicOverflowOrderProperty, ref _dynamicOverflowOrder, value);
    }

    protected override bool RegisterContentPresenter(ContentPresenter presenter)
    {
        if (presenter.Name == "ContentPresenter")
            return true;

        return base.RegisterContentPresenter(presenter);
    }

    private bool _isInOverflow;
    private int _dynamicOverflowOrder;
}
