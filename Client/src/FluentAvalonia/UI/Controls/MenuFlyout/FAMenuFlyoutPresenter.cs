﻿using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Threading;
using Avalonia.VisualTree;

namespace FluentAvalonia.UI.Controls;

/// <summary>
/// Displays the content of a <see cref="FAMenuFlyout"/> control.
/// </summary>
[PseudoClasses(s_pcIcons, s_pcToggle)]
public class FAMenuFlyoutPresenter : ItemsControl
{
    public FAMenuFlyoutPresenter()
    {
        KeyboardNavigation.SetTabNavigation(this, KeyboardNavigationMode.Cycle);

        //AddHandler(AccessKeyHandler.AccessKeyPressedEvent, AccessKeyPressed);
    }

    internal AvaloniaObject InternalParent { get; set; }

    protected override bool NeedsContainerOverride(object item, int index, out object recycleKey)
    {
        recycleKey = typeof(MenuFlyoutItem);
        return !(item is MenuFlyoutItemBase);
    }

    protected override Control CreateContainerForItemOverride(object item, int index, object recycleKey)
    {
        var cont = this.FindDataTemplate(item, ItemTemplate)?.Build(item);

        if (cont is MenuFlyoutItemBase mfib)
        {
            mfib.IsContainerFromTemplate = true;
            return mfib;
        }

        return new MenuFlyoutItem()
        {
            Text = item.ToString()
        };
    }

    protected override void PrepareContainerForItemOverride(Control element, object item, int index)
    {
        var mfib = element as MenuFlyoutItemBase;

        if (!mfib.IsContainerFromTemplate)
            base.PrepareContainerForItemOverride(element, item, index);

        mfib.InternalParent = this;
        var iconCount = _iconCount;
        var toggleCount = _toggleCount;
        if (element is ToggleMenuFlyoutItem tmfi)
        {
            if (tmfi.IconSource != null)
            {
                iconCount++;
            }

            toggleCount++;
        }
        else if (element is RadioMenuFlyoutItem rmfi)
        {
            if (rmfi.IconSource != null)
            {
                iconCount++;
            }

            toggleCount++;
        }
        else if (element is MenuFlyoutItem mfi)
        {
            if (mfi.IconSource != null)
            {
                iconCount++;
            }
        }
        else if (element is MenuFlyoutSubItem mfsi)
        {
            if (mfsi.IconSource != null)
            {
                iconCount++;
            }
        }

        if (iconCount != _iconCount || _toggleCount != toggleCount)
        {
            _iconCount = iconCount;
            _toggleCount = toggleCount;
            // Update all other items already realized based on changes to this one
            UpdateVisualState();            
        }

        // This container isn't realized yet, so we need to apply the classes here
        ((IPseudoClasses)element.Classes).Set(s_pcIcons, iconCount != 0);
        ((IPseudoClasses)element.Classes).Set(s_pcToggle, toggleCount != 0);
    }

    protected override void ClearContainerForItemOverride(Control element)
    {
        base.ClearContainerForItemOverride(element);
        var iconCount = _iconCount;
        var toggleCount = _toggleCount;

        if (element is ToggleMenuFlyoutItem tmfi)
        {
            if (tmfi.IconSource != null)
            {
                iconCount--;
            }

            toggleCount--;
        }
        else if (element is RadioMenuFlyoutItem rmfi)
        {
            if (rmfi.IconSource != null)
            {
                iconCount--;
            }

            toggleCount--;
        }
        else if (element is MenuFlyoutItem mfi)
        {
            if (mfi.IconSource != null)
            {
                iconCount--;
            }
        }
        else if (element is MenuFlyoutSubItem mfsi)
        {
            if (mfsi.IconSource != null)
            {
                iconCount--;
            }
        }

        if (iconCount != _iconCount || _toggleCount != toggleCount)
        {
            _iconCount = iconCount;
            _toggleCount = toggleCount;
            UpdateVisualState();
        }
    }

    protected override void OnKeyDown(KeyEventArgs args)
    {
        base.OnKeyDown(args);
        if (args.Handled)
            return;

        var item = GetMenuItem(args.Source);
        switch (args.Key)
        {
            case Key.Down:
                {
                    var current = TopLevel.GetTopLevel(this).FocusManager.GetFocusedElement();
                    if (current is MenuFlyoutItemBase mfib)
                    {
                        var index = IndexFromContainer(mfib);
                        if (index == -1)
                            return; // Somethings wrong

                        while (true)
                        {
                            index++;
                            if (index >= ItemCount)
                                index = 0;

                            var cont = ContainerFromIndex(index);
                            if (cont != null && !(cont is MenuFlyoutSeparator) &&
                                cont.Focusable && cont.IsEffectivelyEnabled)
                            {
                                cont.Focus(NavigationMethod.Directional);
                                args.Handled = true;
                                break;
                            }
                            else if (cont == item)
                            {
                                // If we loop back to the original item, stop 
                                break; 
                            }
                        }
                    }
                }
                break;

            case Key.Up:
                {
                    var current = TopLevel.GetTopLevel(this).FocusManager.GetFocusedElement();
                    if (current is MenuFlyoutItemBase mfib)
                    {
                        var index = IndexFromContainer(mfib);
                        if (index == -1)
                            return; // Somethings wrong

                        while (true)
                        {
                            index--;
                            if (index < 0)
                                index = ItemCount - 1;

                            var cont = ContainerFromIndex(index);
                            if (cont != null && !(cont is MenuFlyoutSeparator) &&
                                cont.Focusable && cont.IsEffectivelyEnabled)
                            {
                                cont.Focus(NavigationMethod.Directional);
                                args.Handled = true;
                                break;
                            }
                            else if (cont == item)
                            {
                                // If we loop back to the original item, stop 
                                break;
                            }
                        }
                    }
                }
                break;

            case Key.Right:
                {
                    if (item is MenuFlyoutSubItem mfsi)
                    {
                        mfsi.Open(true);
                        args.Handled = true;
                    }
                }
                break;

            case Key.Left:
                {
                    if (InternalParent is MenuFlyoutSubItem mfsi)
                    {
                        // NOTE: Order matters here for some reason, focus the MFSI FIRST,
                        // then close it. Otherwise the focus adorner isn't shown
                        mfsi.Focus(NavigationMethod.Directional);
                        mfsi.Close();
                        args.Handled = true;
                    }
                }
                break;

            case Key.Enter:
                {
                    var current = TopLevel.GetTopLevel(this).FocusManager.GetFocusedElement();
                    if (current is MenuFlyoutItemBase mfib && mfib.Focusable && mfib.IsEffectivelyEnabled)
                    {
                        if (mfib is MenuFlyoutSubItem mfsi)
                        {
                            mfsi.Open(true);
                        }
                        else
                        {
                            (mfib as MenuFlyoutItem)?.RaiseClick();
                            CloseMenu();
                        }
                        args.Handled = true;
                    }
                }
                break;

            case Key.Escape:
                {
                    if (InternalParent is MenuFlyoutSubItem mfsi)
                    {
                        // NOTE: Order matters here for some reason, focus the MFSI FIRST,
                        // then close it. Otherwise the focus adorner isn't shown
                        mfsi.Focus(NavigationMethod.Directional);
                        mfsi.Close();                        
                        args.Handled = true;
                    }
                    else
                    {
                        CloseMenu();
                    }
                }
                break;
        }
    }

    protected override void OnPointerReleased(PointerReleasedEventArgs args)
    {
        base.OnPointerReleased(args);
        var item = GetMenuItem(args.Source);

        // MenuFlyoutSubItem doesn't support click, so we don't raise it there
        // Toggle and Radio MFIs derive from MenuFlyoutItem so this handles everything
        if (item is MenuFlyoutItem mfi)
        {
            mfi.RaiseClick();
            CloseMenu();
            args.Handled = true;
        }
    }

    internal void PointerEnteredItem(MenuFlyoutItemBase item)
    {
        if (item is MenuFlyoutSubItem mfsi)
        {
            if (mfsi == _openedItem)
            {
                _closingCancelDisp?.Dispose();
                return;
            }

            if (_openedItem != null)
            {
                _openedItem.Close();
                _openedItem = null;
            }

            _openingItem = mfsi;
            DispatcherTimer.RunOnce(() =>
            {
                if (_openingItem == mfsi)
                {
                    _openingItem = null;
                    mfsi.Open();
                    _openedItem = mfsi;
                }
            }, TimeSpan.FromMilliseconds(400));
        }
        else
        {
            if (_openedItem != null)
            {
                _closingCancelDisp = DispatcherTimer.RunOnce(() =>
                {
                    _openedItem?.Close();
                    _openedItem = null;
                }, TimeSpan.FromMilliseconds(400));
            }
        }
    }

    internal void PointerExitedItem(MenuFlyoutItemBase item)
    {
        if (_openingItem == item)
        {
            _openingItem = null;
        }
    }

    private void AccessKeyPressed(object sender, RoutedEventArgs args)
    {
        var src = GetMenuItem(args.Source);

        if (src is MenuFlyoutSubItem mfsi)
        {
            mfsi.Open(true);
        }
        else
        {
            (src as MenuFlyoutItem)?.RaiseClick();
        }

        args.Handled = true;
    }

    internal void MenuOpened(bool fromKeyboard = false)
    {
        // Overlay popups continue to be a pain, since they get added very late 
        // after opening the popup - so post this to the dispatcher to run
        // after the next layout pass
        Dispatcher.UIThread.Post(() =>
        {
            var item = GetRealizedContainers()
            .Where(x => x.Focusable && x.IsEffectivelyEnabled)
            .FirstOrDefault();

            if (item != null)
            {
                item.Focus(fromKeyboard ? NavigationMethod.Directional : NavigationMethod.Unspecified);
            }
        }, DispatcherPriority.Render);        
    }

    private MenuFlyoutItemBase GetMenuItem(object src)
    {
        return ((Visual)src).FindAncestorOfType<MenuFlyoutItemBase>(true);
    }

    internal void CloseMenu()
    {
        if (InternalParent is MenuFlyoutSubItem mfsi)
        {
            mfsi.Close(true);
        }
        else if (InternalParent is FAMenuFlyout fmf)
        {
            fmf.Close();
        }
    }
        
    private void UpdateVisualState()
    {
        // v2 Change: ControlThemes means we can't use styling on the MFP to apply the 
        // Icon/Toggle adjustments and we have to put them directly on the items

        bool icon = _iconCount > 0;
        bool toggle = _toggleCount > 0;
        foreach (var item in GetRealizedContainers())
        {
            ((IPseudoClasses)item.Classes).Set(s_pcIcons, icon);
            ((IPseudoClasses)item.Classes).Set(s_pcToggle, toggle);
        }
    }

    private MenuFlyoutItemBase _openingItem;
    private MenuFlyoutSubItem _openedItem;
    private IDisposable _closingCancelDisp;

    private int _iconCount = 0;
    private int _toggleCount = 0;

    private const string s_pcIcons = ":icons";
    private const string s_pcToggle = ":toggle";
}
