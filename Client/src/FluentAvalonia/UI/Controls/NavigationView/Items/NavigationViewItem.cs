﻿using Avalonia;
using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.Threading;
using Avalonia.VisualTree;
using FluentAvalonia.UI.Controls.Primitives;
using System.Collections.Specialized;
using System.ComponentModel;

namespace FluentAvalonia.UI.Controls;
using FluentAvalonia.Core;

/// <summary>
/// Represents the container for an item in a NavigationView control.
/// </summary>
public partial class NavigationViewItem : NavigationViewItemBase
{
    public NavigationViewItem()
    {
        MenuItems = new AvaloniaList<object>();
    }

    protected override void OnNavigationViewItemBaseDepthChanged()
    {
        UpdateItemIndentation();
        PropagateDepthToChildren(Depth + 1);
    }

    protected override void OnNavigationViewItemBaseIsSelectedChanged()
    {
        UpdateVisualState();
    }

    protected override void OnNavigationViewItemBasePositionChanged()
    {
        UpdateVisualState();
        ReparentRepeater();

        // We can't set the Flyout position in Styles, so we change the position here
        if (_rootGrid != null)
        {
            var flyout = _rootGrid.GetValue(FlyoutBase.AttachedFlyoutProperty) as PopupFlyoutBase;
            if (flyout != null)
            {
                flyout.Placement = (Position == NavigationViewRepeaterPosition.TopPrimary ||
                    Position == NavigationViewRepeaterPosition.TopFooter) ?
                    PlacementMode.BottomEdgeAlignedLeft :
                    PlacementMode.RightEdgeAlignedTop;

            }
        }
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        _appliedTemplate = false;

        UnhookEventsAndClearFields();

        base.OnApplyTemplate(e);

        _presenter = e.NameScope.Find<NavigationViewItemPresenter>(s_tpNVIPresenter);

        _rootGrid = e.NameScope.Find<Grid>(s_tpNVIRootGrid);
        if (_rootGrid != null)
        {
            var flyout = FlyoutBase.GetAttachedFlyout(_rootGrid) as PopupFlyoutBase;
            if (flyout != null)
            {
                flyout.Closing += OnFlyoutClosing;
            }
        }

        var navView = GetNavigationView;
        //WinUI must have a different order of doing things b/c OnApplyTemplate is called BEFORE
        //OnElementPrepared in Avalonia, so NavigationView & SplitView refs don't exist yet. WinUI
        //must do this reversed and not add the item to the tree until AFTER OnElementPrepared
        //To compensate, we'll set the target now
        //Conveniently, this also means we don't need to impl winui #5039, because we are already
        //in the visual tree so the splitview reference is made
        if (navView == null)
        {
            navView = this.FindAncestorOfType<NavigationView>();
            SetNavigationViewParent(navView);
        }

        var splitView = GetSplitView;
        if (splitView != null)
        {
            _splitViewRevokers = new FACompositeDisposable(
                splitView.GetPropertyChangedObservable(SplitView.IsPaneOpenProperty).Subscribe(OnSplitViewPropertyChanged),
                splitView.GetPropertyChangedObservable(SplitView.DisplayModeProperty).Subscribe(OnSplitViewPropertyChanged),
                splitView.GetPropertyChangedObservable(SplitView.CompactPaneLengthProperty).Subscribe(OnSplitViewPropertyChanged));

            UpdateCompactPaneLength();
            UpdateIsClosedCompact();
        }

        //var navView = GetNavigationView;
        if (navView != null)
        {
            _repeater = e.NameScope.Find<ItemsRepeater>(s_tpNVIMenuItemsHost);
            if (_repeater != null)
            {
                (_repeater.Layout as StackLayout).DisableVirtualization = true;

                _repeater.ElementPrepared += navView.OnRepeaterElementPrepared;
                _repeater.ElementClearing += navView.OnRepeaterElementClearing;

                _repeater.ItemTemplate = navView.ItemsFactory;
            }

            UpdateRepeaterItemsSource();
        }

        _flyoutContentGrid = e.NameScope.Find<Panel>(s_tpFlyoutContentGrid);

        _appliedTemplate = true;

        UpdateItemIndentation();
        UpdateVisualState();
        ReparentRepeater();
        // We dont want to update the repeater visibilty during OnApplyTemplate if NavigationView is in a mode when items are shown in a flyout
        if (!ShouldRepeaterShowInFlyout)
        {
            ShowHideChildren();
        }
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (change.Property == IconSourceProperty)
        {
            OnIconPropertyChanged(change);
        }
        else if (change.Property == ContentProperty)
        {
            OnContentChanged(change);
        }
        else if (change.Property == InfoBadgeProperty)
        {
            UpdateVisualStateForInfoBadge();
        }
        else if (change.Property == MenuItemsProperty)
        {
            OnMenuItemsPropertyChanged();
        }
        else if (change.Property == MenuItemsSourceProperty)
        {
            OnMenuItemsSourcePropertyChanged();
        }
    }

    private void UpdateRepeaterItemsSource()
    {
        if (_repeater != null)
        {
            if (_repeater.ItemsSourceView != null)
            {
                _repeater.ItemsSourceView.CollectionChanged -= OnItemsSourceViewChanged;
            }

            var miSource = MenuItemsSource;

            _repeater.ItemsSource = miSource != null ? miSource : _menuItems;

            if (_repeater.ItemsSourceView != null)
            {
                _repeater.ItemsSourceView.CollectionChanged += OnItemsSourceViewChanged;
            }
        }
    }

    private void OnItemsSourceViewChanged(object sender, NotifyCollectionChangedEventArgs args)
    {
        UpdateVisualStateForChevron();
    }

    private void OnSplitViewPropertyChanged(AvaloniaPropertyChangedEventArgs args)
    {
        if (args.Property == SplitView.CompactPaneLengthProperty)
        {
            UpdateCompactPaneLength();
        }
        else if (args.Property == SplitView.IsPaneOpenProperty ||
            args.Property == SplitView.DisplayModeProperty)
        {
            UpdateIsClosedCompact();
            ReparentRepeater();
        }
    }

    private void UpdateCompactPaneLength()
    {
        var splitView = GetSplitView;
        if (splitView != null)
        {
            double paneLength = splitView.CompactPaneLength;
            CompactPaneLength = paneLength;

            if (_presenter != null)
            {
                _presenter.UpdateCompactPaneLength(paneLength, IsOnLeftNav);
            }
        }
    }

    private void UpdateIsClosedCompact()
    {
        var splitView = GetSplitView;
        if (splitView != null)
        {
            _isClosedCompact = !splitView.IsPaneOpen &&
                (splitView.DisplayMode == SplitViewDisplayMode.CompactOverlay || splitView.DisplayMode == SplitViewDisplayMode.CompactInline);

            UpdateVisualState();
        }
    }

    private void UpdateVisualStateForClosedCompact()
    {
        if (_presenter != null)
        {
            _presenter.UpdateClosedCompactVisualState(IsTopLevelItem, _isClosedCompact);
        }
    }

    private void UpdateNavigationViewItemToolTip()
    {
        var tip = ToolTip.GetTip(this);

        if (tip == null || tip == _suggestedToolTipContent)
        {
            if (ShouldEnableToolTip)
            {
                if (tip != _suggestedToolTipContent)
                    ToolTip.SetTip(this, _suggestedToolTipContent);
            }
            else
            {
                ToolTip.SetTip(this, null);
            }
        }
    }

    private void SuggestedToolTipChanged(object newContent)
    {
        object newToolTip = null;
        if (newContent is string s)
        {
            newToolTip = s;
        }

        // Both customer and NavigationViewItem can update ToolTipContent by winrt::ToolTipService::SetToolTip or XAML
        // If the ToolTipContent is not the same as m_suggestedToolTipContent, then it's set by customer.
        // Customer's ToolTip take high priority, and we never override Customer's ToolTip.
        var toolTip = ToolTip.GetTip(this);
        if (_suggestedToolTipContent != null)
        {
            if (toolTip == _suggestedToolTipContent)
            {
                ToolTip.SetTip(this, null);
            }
        }

        _suggestedToolTipContent = newToolTip;
    }

    protected virtual void OnIsExpandedPropertyChanged()
    {
        //Added this...
        UpdateVisualStateForChevron();
    }

    protected virtual void OnIconPropertyChanged(AvaloniaPropertyChangedEventArgs args)
    {
        UpdateVisualState();
    }

    protected virtual void OnContentChanged(AvaloniaPropertyChangedEventArgs args)
    {
        SuggestedToolTipChanged(args.NewValue);
        UpdateVisualState();

        if (!IsOnLeftNav)
        {
            var navView = GetNavigationView;
            navView?.TopNavigationViewItemContentChanged();
        }
    }

    protected virtual void OnMenuItemsPropertyChanged()
    {
        // We shouldn't need this now as Avalonia has no support for x:Load
        // see WinUI #6808
        //if (_menuItems.Count > 0)
        //{
        //    LoadElementsForDisplayingChildren();
        //}

        UpdateRepeaterItemsSource();
        UpdateVisualStateForChevron();
    }

    protected virtual void OnMenuItemsSourcePropertyChanged()
    {
        // See above
        //LoadElementsForDisplayingChildren();
        UpdateRepeaterItemsSource();
        UpdateVisualStateForChevron();
    }
        
    private void OnHasUnrealizedChildrenPropertyChanged()
    {
        UpdateVisualStateForChevron();
    }

    private void ShowSelectionIndicator(bool vis)
    {
        if (SelectionIndicator != null)
        {
            SelectionIndicator.Opacity = vis ? 1.0 : 0.0;
        }
    }

    private void UpdateVisualStateForIconAndContent(bool showIcon, bool showContent)
    {
        if (_presenter != null)
        {
            //Possible states :iconleft, :icononly, :contentonly
            ((IPseudoClasses)_presenter.Classes).Set(SharedPseudoclasses.s_pcIconLeft, showIcon && showContent);
            ((IPseudoClasses)_presenter.Classes).Set(SharedPseudoclasses.s_pcIconOnly, showIcon && !showContent);
            ((IPseudoClasses)_presenter.Classes).Set(SharedPseudoclasses.s_pcContentOnly, !showIcon);
        }
    }

    private void UpdateVisualStateForNavigationViewPositionChange()
    {
        // v2: We no longer need to propagate the styles with ControlThemes, however,
        // for compat and external support we will still set the pseudoclasses
        switch (Position)
        {
            case NavigationViewRepeaterPosition.LeftNav:
            case NavigationViewRepeaterPosition.LeftFooter:
                PseudoClasses.Set(SharedPseudoclasses.s_pcLeftNav, true);
                PseudoClasses.Set(SharedPseudoclasses.s_pcTopNav, false);
                PseudoClasses.Set(SharedPseudoclasses.s_pcTopOverflow, false);

                if (_presenter != null)
                {
                    ((IPseudoClasses)_presenter.Classes).Set(SharedPseudoclasses.s_pcLeftNav, true);
                    ((IPseudoClasses)_presenter.Classes).Set(SharedPseudoclasses.s_pcTopNav, false);
                    ((IPseudoClasses)_presenter.Classes).Set(SharedPseudoclasses.s_pcTopOverflow, false);
                }

                break;

            case NavigationViewRepeaterPosition.TopPrimary:
            case NavigationViewRepeaterPosition.TopFooter:
                PseudoClasses.Set(SharedPseudoclasses.s_pcLeftNav, false);
                PseudoClasses.Set(SharedPseudoclasses.s_pcTopNav, true);
                PseudoClasses.Set(SharedPseudoclasses.s_pcTopOverflow, false);

                if (_presenter != null)
                {
                    ((IPseudoClasses)_presenter.Classes).Set(SharedPseudoclasses.s_pcLeftNav, false);
                    ((IPseudoClasses)_presenter.Classes).Set(SharedPseudoclasses.s_pcTopNav, true);
                    ((IPseudoClasses)_presenter.Classes).Set(SharedPseudoclasses.s_pcTopOverflow, false);
                }
                break;

            case NavigationViewRepeaterPosition.TopOverflow:
                PseudoClasses.Set(SharedPseudoclasses.s_pcLeftNav, false);
                PseudoClasses.Set(SharedPseudoclasses.s_pcTopNav, false);
                PseudoClasses.Set(SharedPseudoclasses.s_pcTopOverflow, true);

                if (_presenter != null)
                {
                    ((IPseudoClasses)_presenter.Classes).Set(SharedPseudoclasses.s_pcLeftNav, false);
                    ((IPseudoClasses)_presenter.Classes).Set(SharedPseudoclasses.s_pcTopNav, false);
                    ((IPseudoClasses)_presenter.Classes).Set(SharedPseudoclasses.s_pcTopOverflow, true);
                }
                break;
        }

        UpdateVisualStateForClosedCompact();
    }

    private void UpdateVisualStateForToolTip()
    {
        UpdateNavigationViewItemToolTip();
    }

    internal void UpdateVisualState()
    {
        if (!_appliedTemplate)
            return;

        if (_presenter != null)
        {
            ((IPseudoClasses)_presenter.Classes).Set(s_pcSelected, IsSelected);
        }

        UpdateVisualStateForNavigationViewPositionChange();

        bool showIcon = ShouldShowIcon;
        bool showContent = ShouldShowContent;

        if (IsOnLeftNav)
        {
            if (_presenter != null)
            {
                //This is supposed to be for backwards compatibility with RS4-, but
                //is apparently still used in the NVIPresenterWhenOnLeftPane style
                ((IPseudoClasses)_presenter.Classes).Set(s_pcIconCollapsed, !showIcon);
                //Only using IconCollapsed, IconVisible is default
            }
        }
        else
        {
            if (_presenter != null)
            {
                ((IPseudoClasses)_presenter.Classes).Set(s_pcIconCollapsed, false);
            }
        }

        UpdateVisualStateForToolTip();

        UpdateVisualStateForIconAndContent(showIcon, showContent);

        UpdateVisualStateForInfoBadge();

        UpdateVisualStateForChevron();
    }

    private void UpdateVisualStateForChevron()
    {
        if (_presenter != null)
        {
            //auto const chevronState = HasChildren() && !(m_isClosedCompact && ShouldRepeaterShowInFlyout()) ? 
            //                          (IsExpanded() ? c_chevronVisibleOpen : c_chevronVisibleClosed) : c_chevronHidden;
            //winrt::VisualStateManager::GoToState(presenter, chevronState, true);

            //States :chevronopen, :chevronclosed, :chevronhidden

            bool show = HasChildren && !(_isClosedCompact && ShouldRepeaterShowInFlyout);
            bool expand = IsExpanded;

            if (_presenter != null)
            {
                ((IPseudoClasses)_presenter.Classes).Set(SharedPseudoclasses.s_pcChevronOpen, show && expand);
                ((IPseudoClasses)_presenter.Classes).Set(SharedPseudoclasses.s_pcChevronClosed, show & !expand);
                ((IPseudoClasses)_presenter.Classes).Set(SharedPseudoclasses.s_pcChevronHidden, !show);
            }
        }
    }

    internal void ShowHideChildren()
    {
        if (_repeater == null)
            return;

        bool shouldShowChildren = IsExpanded;
        _repeater.IsVisible = shouldShowChildren;

        if (ShouldRepeaterShowInFlyout)
        {
            if (shouldShowChildren)
            {
                if (!_isRepeaterParentedToFlyout)
                {
                    ReparentRepeater();
                }

                Dispatcher.UIThread.Post(() => FlyoutBase.ShowAttachedFlyout(_rootGrid));
            }
            else
            {
                FlyoutBase.GetAttachedFlyout(_rootGrid)?.Hide();
            }
        }
    }

    private void ReparentRepeater()
    {
        if (HasChildren && _repeater != null)
        {
            if (ShouldRepeaterShowInFlyout && !_isRepeaterParentedToFlyout)
            {
                _rootGrid.Children.Remove(_repeater);
                _flyoutContentGrid.Children.Add(_repeater);
                _isRepeaterParentedToFlyout = true;

                PropagateDepthToChildren(0);
            }
            else if (!ShouldRepeaterShowInFlyout && _isRepeaterParentedToFlyout)
            {
                _flyoutContentGrid.Children.Remove(_repeater);
                _rootGrid.Children.Add(_repeater);
                _isRepeaterParentedToFlyout = false;

                PropagateDepthToChildren(1);
            }
        }
    }

    private void UpdateItemIndentation()
    {
        if (_presenter != null)
        {
            var newLeftmargin = Depth * _itemIndentation;
            _presenter.UpdateContentLeftIndentation(newLeftmargin);
        }
    }

    internal void PropagateDepthToChildren(int depth)
    {
        if (_repeater == null || _repeater.ItemsSourceView == null)
            return;

        var count = _repeater.ItemsSourceView.Count;

        for (int i = 0; i < count; i++)
        {
            if (_repeater.TryGetElement(i) is NavigationViewItemBase nvib)
            {
                nvib.Depth = depth;
            }
        }
    }

    internal void OnExpandCollapseChevronTapped(object sender, RoutedEventArgs args)
    {
        IsExpanded = !IsExpanded;
        args.Handled = true;
    }

    private void OnFlyoutClosing(object sender, CancelEventArgs args)
    {
        IsExpanded = false;
    }

    internal void RotateExpandCollapseChevron(bool isExpanded)
    {
        if (_presenter != null)
        {
            _presenter.RotateExpandCollapseChevron(isExpanded);
        }
    }

    private void UnhookEventsAndClearFields()
    {
        if (_rootGrid != null)
        {
            var flyout = FlyoutBase.GetAttachedFlyout(_rootGrid) as PopupFlyoutBase;
            if (flyout != null)
            {
                flyout.Closing -= OnFlyoutClosing;
            }
            _rootGrid = null;
        }

        _splitViewRevokers?.Dispose();
        _splitViewRevokers = null;

        var navView = GetNavigationView;
        if (navView != null && _repeater != null)
        {
            _repeater.ElementPrepared -= navView.OnRepeaterElementPrepared;
            _repeater.ElementClearing -= navView.OnRepeaterElementClearing;

            if (_repeater.ItemsSourceView != null)
            {
                _repeater.ItemsSourceView.CollectionChanged -= OnItemsSourceViewChanged;
            }
            _repeater.ItemsSource = null;
            _repeater = null;
        }

        _presenter = null;
        _flyoutContentGrid = null;
    }

    private void UpdateVisualStateForInfoBadge()
    {
        if (_presenter != null)
            ((IPseudoClasses)_presenter.Classes).Set(s_pcInfoBadge, InfoBadge != null);
    }

    public override string ToString()
    {
        return Content?.ToString() ?? "NavigationViewItem";
    }


    private FACompositeDisposable _splitViewRevokers;
    private NavigationViewItemPresenter _presenter;
    private object _suggestedToolTipContent;
    private ItemsRepeater _repeater;
    private Panel _flyoutContentGrid;
    private Grid _rootGrid;

    private bool _isClosedCompact;
    private bool _appliedTemplate;
    //private bool _hasKeyboardFocus;//TODO: needed?
    private bool _isRepeaterParentedToFlyout;
}
