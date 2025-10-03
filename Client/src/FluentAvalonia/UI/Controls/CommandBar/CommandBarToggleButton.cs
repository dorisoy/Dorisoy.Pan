﻿using Avalonia.Controls;
using Avalonia;
using Avalonia.Controls.Primitives;
using Avalonia.Styling;
using Avalonia.LogicalTree;
using FluentAvalonia.UI.Input;
using FluentAvalonia.Core;
using Avalonia.Controls.Presenters;

namespace FluentAvalonia.UI.Controls;

/// <summary>
/// Represents a button control that can switch states and be displayed in a CommandBar.
/// </summary>
public partial class CommandBarToggleButton : ToggleButton, ICommandBarElement
{
    public CommandBarToggleButton()
    {
        TemplateSettings = new CommandBarButtonTemplateSettings();
    }

    protected override Type StyleKeyOverride => typeof(CommandBarToggleButton);

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);

        if (change.Property == IconSourceProperty)
        {
            PseudoClasses.Set(SharedPseudoclasses.s_pcIcon, change.NewValue != null);
            TemplateSettings.Icon = IconHelpers.CreateFromUnknown(change.GetNewValue<IconSource>());
        }
        else if (change.Property == LabelProperty)
        {
            PseudoClasses.Set(SharedPseudoclasses.s_pcLabel, change.NewValue != null);
        }
        else if (change.Property == HotKeyProperty)
        {
            PseudoClasses.Set(SharedPseudoclasses.s_pcHotkey, change.NewValue != null);
        }
        else if (change.Property == IsCompactProperty)
        {
            PseudoClasses.Set(SharedPseudoclasses.s_pcCompact, change.GetNewValue<bool>());
        }
        else if (change.Property == CommandProperty)
        {
            if (change.OldValue is XamlUICommand xamlComOld)
            {
                if (Label == xamlComOld.Label)
                {
                    Label = null;
                }

                if (HotKey == xamlComOld.HotKey)
                {
                    HotKey = null;
                }

                if (ToolTip.GetTip(this).ToString() == xamlComOld.Description)
                {
                    ToolTip.SetTip(this, null);
                }
            }

            if (change.NewValue is XamlUICommand xamlCom)
            {
                if (string.IsNullOrEmpty(Label))
                {
                    Label = xamlCom.Label;
                }

                IconSource = xamlCom.IconSource;

                if (HotKey == null)
                {
                    HotKey = xamlCom.HotKey;
                }

                if (ToolTip.GetTip(this) == null)
                {
                    ToolTip.SetTip(this, xamlCom.Description);
                }
            }
        }
    }

    protected override void OnClick()
    {
        base.OnClick();
        if (IsInOverflow)
        {
            var cb = this.FindLogicalAncestorOfType<CommandBar>();
            if (cb != null)
            {
                cb.IsOpen = false;
            }
        }
    }

    protected override bool RegisterContentPresenter(ContentPresenter presenter)
    {
        if (presenter.Name == "ContentPresenter")
            return true;

        return base.RegisterContentPresenter(presenter);
    }
}
