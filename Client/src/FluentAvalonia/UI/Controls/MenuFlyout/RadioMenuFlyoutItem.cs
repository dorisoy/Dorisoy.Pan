﻿using Avalonia.Controls;
using Avalonia.Styling;
using Avalonia;
using Avalonia.Data;
using Avalonia.Controls.Metadata;
using FluentAvalonia.Core;

namespace FluentAvalonia.UI.Controls;

/// <summary>
/// Represents a menu item that is mutually exclusive with other radio menu items in its group.
/// </summary>
[PseudoClasses(SharedPseudoclasses.s_pcChecked)]
public class RadioMenuFlyoutItem : MenuFlyoutItem
{
    static RadioMenuFlyoutItem()
    {
        if (SelectionMap == null)
        {
            SelectionMap = new SortedDictionary<string, WeakReference<RadioMenuFlyoutItem>>();
        }
    }

    /// <summary>
    /// Defines the <see cref="GroupName"/> property
    /// </summary>
    public static readonly StyledProperty<string> GroupNameProperty =
        RadioButton.GroupNameProperty.AddOwner<RadioMenuFlyoutItem>(
            new StyledPropertyMetadata<string>(
                coerce: (_, x) => x ?? string.Empty));

    /// <summary>
    /// Defines the <see cref="IsChecked"/> property
    /// </summary>
    public static readonly StyledProperty<bool> IsCheckedProperty =
        AvaloniaProperty.Register<RadioMenuFlyoutItem, bool>(nameof(IsChecked),
            defaultBindingMode: BindingMode.TwoWay);

    /// <summary>
    /// Gets or sets the name that specifies which RadioMenuFlyoutItem controls are mutually exclusive.
    /// </summary>
    public string GroupName
    {
        get => GetValue(GroupNameProperty);
        set => SetValue(GroupNameProperty, value);
    }

    /// <summary>
    /// Gets or sets whether this RadioMenuFlyoutItem is checked
    /// </summary>
    public bool IsChecked
    {
        get => GetValue(IsCheckedProperty);
        set => SetValue(IsCheckedProperty, value);
    }

    protected override Type StyleKeyOverride => typeof(RadioMenuFlyoutItem);

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (change.Property == IsCheckedProperty)
        {
            var newValue = change.GetNewValue<bool>();
            PseudoClasses.Set(SharedPseudoclasses.s_pcChecked, newValue);

            if (_isSafeUncheck) // Unchecked via another item in the group
                return;

            UpdateCheckedItemInGroup();
        }
        else if (change.Property == GroupNameProperty)
        {
            // WinUI doesn't do this here, but this fixes an issue where radio items
            // would misbehave if IsChecked as assigned before group
            UpdateCheckedItemInGroup();
        }
    }

    private void UncheckFromGroupSelection()
    {
        _isSafeUncheck = true;
        IsChecked = false;
        _isSafeUncheck = false;
    }

    private void UpdateCheckedItemInGroup()
    {
        if (IsChecked)
        {
            var groupName = GroupName;
            if (string.IsNullOrEmpty(groupName))
                return;

            if (SelectionMap.TryGetValue(groupName, out var group))
            {
                if (group.TryGetTarget(out var item))
                {
                    if (item == this)
                        return;

                    item.UncheckFromGroupSelection();
                }

                SelectionMap[groupName] = new WeakReference<RadioMenuFlyoutItem>(this);
            }
            else
            {
                SelectionMap.Add(groupName, new WeakReference<RadioMenuFlyoutItem>(this));
            }
        }
    }

    protected override void OnClick()
    {
        base.OnClick();
        IsChecked = !IsChecked;
    }

    private bool _isSafeUncheck = false;

    internal static readonly SortedDictionary<string, WeakReference<RadioMenuFlyoutItem>> SelectionMap;
}

