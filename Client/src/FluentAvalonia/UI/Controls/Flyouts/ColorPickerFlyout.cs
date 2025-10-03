﻿using Avalonia.Controls;
using FluentAvalonia.Core;
using FluentAvalonia.UI.Controls.Primitives;
using System.ComponentModel;

namespace FluentAvalonia.UI.Controls;

/// <summary>
/// Defines a flyout that hosts a <see cref="ColorPicker"/>
/// </summary>
public sealed class ColorPickerFlyout : PickerFlyoutBase
{
    /// <summary>
    /// Gets the <see cref="ColorPicker"/> that this flyout hosts
    /// </summary>
    public FAColorPicker ColorPicker => _picker ??= new FAColorPicker();

    /// <summary>
    /// Raised when the Confirmed button is tapped indicating the new Color should be applied
    /// </summary>
    public event TypedEventHandler<ColorPickerFlyout, EventArgs> Confirmed;

    /// <summary>
    /// Raised when the Dismiss button is tapped, indicating the new color should not be applied
    /// </summary>
    public event TypedEventHandler<ColorPickerFlyout, EventArgs> Dismissed;

    protected override Control CreatePresenter()
    {
        if (_picker == null)
            _picker = new FAColorPicker();

        var pfp = new PickerFlyoutPresenter()
        {
            Content = _picker
        };
        pfp.Confirmed += OnFlyoutConfirmed;
        pfp.Dismissed += OnFlyoutDismissed;

        return pfp;
    }

    protected override void OnConfirmed()
    {
        Confirmed?.Invoke(this, EventArgs.Empty);
        Hide();
    }

    protected override void OnOpening(CancelEventArgs args)
    {
        base.OnOpening(args);
    }

    protected override bool ShouldShowConfirmationButtons() => _showButtons;

    private void OnFlyoutDismissed(PickerFlyoutPresenter sender, object args)
    {
        Dismissed?.Invoke(this, EventArgs.Empty);
        Hide();
    }

    private void OnFlyoutConfirmed(PickerFlyoutPresenter sender, object args)
    {
        OnConfirmed();
    }

    internal void ShowHideButtons(bool show)
    {
        _showButtons = show;
    }

    private bool _showButtons = true;
    private FAColorPicker _picker;
}
