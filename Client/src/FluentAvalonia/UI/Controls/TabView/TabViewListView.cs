﻿using System.Collections;
using System.Collections.Specialized;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Shapes;
using Avalonia.Controls.Templates;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Threading;
using Avalonia.VisualTree;
using FluentAvalonia.Core;

namespace FluentAvalonia.UI.Controls.Primitives;

/// <summary>
/// Represents the ListView used in the TabStrip of a <see cref="TabView"/>
/// </summary>
/// <remarks>
/// This control should not be used outside of a TabView
/// </remarks>
[PseudoClasses(s_pcReorder)]
[TemplatePart(s_tpScrollViewer, typeof(ScrollViewer))]
public class TabViewListView : ListBox
{
    private static object _lock = new object();

    public TabViewListView()
    {
        AddHandler(PointerPressedEvent, OnPointerPressedPreview, RoutingStrategies.Tunnel);

        // So... Avalonia doesn't put the Drag/Drop events in a control base class like WPF/UWP does
        // (UIElement), which means, if we take this subscription here, there is a good chance this
        // will get the event first before anything else (in this case the TabView), which means 
        // we have no way of knowing how the user wants the drag drop interaction to work.
        // So, we have to handle these events here, and translate them into our own events that can
        // then be subscribed to by the TabView - we will only make the CLR events, as we don't 
        // need the full routed event, and we will mark these handled
        // As the TabView only needs DragOver (the most important and Drop, we will only do those)
        AddHandler(DragDrop.DragOverEvent, OnDragOver, RoutingStrategies.Bubble, true);
        AddHandler(DragDrop.DropEvent, OnDrop, RoutingStrategies.Bubble, true);

        AddHandler(DragDrop.DragEnterEvent, OnDragEnter, RoutingStrategies.Bubble, true);
        AddHandler(DragDrop.DragLeaveEvent, OnDragLeave, RoutingStrategies.Bubble, true);

        ItemsView.CollectionChanged += OnItemsChanged;
    }

    /// <summary>
    /// Defines the <see cref="CanReorderItems"/> property
    /// </summary>
    public static readonly StyledProperty<bool> CanReorderItemsProperty =
        AvaloniaProperty.Register<TabViewListView, bool>(nameof(CanReorderItems));

    /// <summary>
    /// Defines the <see cref="CanDragItems"/> property
    /// </summary>
    public static readonly StyledProperty<bool> CanDragItemsProperty =
        AvaloniaProperty.Register<TabViewListView, bool>(nameof(CanDragItems));

    /// <summary>
    /// Gets or sets whether this ListView can reorder items
    /// </summary>
    public bool CanReorderItems
    {
        get => GetValue(CanReorderItemsProperty);
        set => SetValue(CanReorderItemsProperty, value);
    }

    /// <summary>
    /// Gets or sets whether dragging items is supported on this listview
    /// </summary>
    public bool CanDragItems
    {
        get => GetValue(CanDragItemsProperty);
        set => SetValue(CanDragItemsProperty, value);
    }

    internal ScrollViewer Scroller { get; private set; }

    // See constructor for why we have these events
    public event EventHandler<DragEventArgs> DragOver;
    public event EventHandler<DragEventArgs> Drop;

    /// <summary>
    /// Occurs when a drag operation that involves one of the items in the view is initiated.
    /// </summary>
    public event DragItemsStartingEventHandler DragItemsStarting;

    /// <summary>
    /// Occurs when a drag operation that involves one of the items in the view is ended.
    /// </summary>
    public event TypedEventHandler<TabViewListView, DragItemsCompletedEventArgs> DragItemsCompleted;

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        Scroller = e.NameScope.Find<ScrollViewer>(s_tpScrollViewer);
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (change.Property == BoundsProperty)
        {
            double viewportWidth = Scroller.Viewport.Width;
            _noAutoScrollRect = new Rect(viewportWidth * 0.1, 0,
                viewportWidth - (viewportWidth * 0.1), Scroller.Viewport.Height);
        }
    }

    private void OnItemsChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
        var tv = this.FindAncestorOfType<TabView>();
        if (tv != null)
        {
            tv.OnItemsChanged(e);
        }
    }

    protected override bool NeedsContainerOverride(object item, int index, out object recycleKey)
    {
        bool isItem = item is TabViewItem;
        recycleKey = isItem ? null : nameof(TabViewItem);
        return !isItem;
    }

    protected override Control CreateContainerForItemOverride(object item, int index, object recycleKey)
    {
        var cont = this.FindDataTemplate(item, ItemTemplate)?.Build(item);

        if (cont is TabViewItem tvi)
        {
            tvi.IsContainerFromTemplate = true;
            return tvi;
        }

        return new TabViewItem();
    }

    protected override void PrepareContainerForItemOverride(Control element, object item, int index)
    {
        var tvi = element as TabViewItem;

        // WinUI: Due to virtualization, a TabViewItem might be recycled to display a different tab data item.
        //        In that case, there is no need to set the TabWidthMode of the TabViewItem or its parent TabView
        //        as they are already set correctly here.
        //
        //        We know we are currently looking at a TabViewItem being recycled if its parent TabView has
        //        already been set.
        if (tvi.ParentTabView == null)
        {
            var parentTV = element.FindAncestorOfType<TabView>();
            if (parentTV != null)
            {
                tvi.OnTabViewWidthModeChanged(parentTV.TabWidthMode);
                tvi.ParentTabView = parentTV;
            }
        }

        // Special b/c we use the header and not Content. Somehow this *just works* in
        // WinUI b/c they don't have to do this.
        if (element == item || tvi.IsContainerFromTemplate)
            return;

        tvi.Header = item;

        var itemTemplate = this.FindDataTemplate(item, ItemTemplate);

        if (itemTemplate != null)
            tvi.HeaderTemplate = itemTemplate;

        // Don't call base here, we've done everything we need
    }
       
    private void OnPointerPressedPreview(object sender, PointerPressedEventArgs args)
    {
        if (CanDragItems || CanReorderItems)
        {
            if (Presenter.Panel is not TabViewStackPanel)
                return;

            var currentPoint = args.GetCurrentPoint(this);
            if (currentPoint.Properties.PointerUpdateKind == PointerUpdateKind.LeftButtonPressed)
            {
                _initialPoint = args.GetPosition(this);
                _dragItem = (args.Source as Visual).FindAncestorOfType<TabViewItem>(true);
                _dragIndex = IndexFromContainer(_dragItem);

                if (double.IsNaN(_cxDrag))
                {
                    UpdateDragInfo();
                }
            }
        }
    }

    protected override void OnPointerMoved(PointerEventArgs e)
    {
        if (_initialPoint.HasValue)
        {
            if (!_isInReorder)
            {
                // We have a possible drag / reorder operation under way, we first need to see
                // if we meet the requirements
                var currentPoint = e.GetCurrentPoint(this);
                var delta = _initialPoint.Value - currentPoint.Position;

                if (Math.Abs(delta.X) > _cxDrag || Math.Abs(delta.Y) > _cyDrag)
                {
                    // Possible actions
                    // 1 - CanDragItems && CanReorderItems
                    //         Let DragDrop handle behavior. We can both reorder and drag. This TabView will
                    //         give visual feedback as the item is dragged and data is sent to DragDrop
                    // 2 - CanDragItems && !CanReorderItems
                    //         Dragging is permitted, but we can't reorder this TabView. This TabView will not 
                    //         accept the drop target and will give no visual feedback as the item is dragged.
                    //         But the data is sent to DragDrop and other TabView's can pick it up
                    // 3 - !CanDragItems && CanReorderItems
                    //         Dragging is only permitted in the context of this TabView to reorder the tabs.
                    //         The TabView will display visual feedback as the tab is dragged, BUT the data
                    //         is not sent to DragDrop and other TabViews will not respond
                    // 4 - !CanDragItems && !CanReorderItems
                    //         This case is handled in the PreviewPointerPressed handler. No dragging actions
                    //         are permitted on this TabView

                    if (!CanDragItems)
                    {
                        // This is a reorder action only (case 3)
                        // Reorder only actions we handle ourselves
                        _processReorder = true;
                        BeginReorder(e);
                        e.Handled = true;
                    }
                    else if (!CanReorderItems)
                    {
                        // This is a drag action only (case 2)
                        // We hand this over to DragDrop

                        BeginDragDrop(e, false);
                        e.Handled = true;
                        return;
                    }
                    else
                    {
                        // This is a full drag / reorder action (case 1)
                        // We hand this over to DragDrop & will use the DragDrop handlers
                        // to manage everything.
                        BeginDragDrop(e, true);
                        e.Handled = true;
                        return;
                    }
                }
            }
            else if (!_isInDrag) // I didn't think PointerMoved worked during Drag, but somehow it is
            {
                var scrollCheck = CheckAutoScroll(e.GetPosition(Scroller));

                switch (scrollCheck)
                {
                    case AutoScrollAction.ScrollStarted:
                        // All reorder logic is disabled when auto scrolling, so reset the panel
                        // to draw normally
                        (ItemsPanelRoot as TabViewStackPanel).ClearReorder();
                        break;

                    case AutoScrollAction.ScrollEnded:
                        (ItemsPanelRoot as TabViewStackPanel).EnterReorder(_dragIndex);
                        break;

                    default:
                        // No DragDrop, we're just reordering this ListView
                        HandleReorder(e.GetPosition(ItemsPanelRoot));
                        break;
                }

                e.Handled = true;
            }
        }

        base.OnPointerMoved(e);
    }

    protected override void OnPointerReleased(PointerReleasedEventArgs e)
    {
        base.OnPointerReleased(e);

        if (_isInReorder)
            EndReorder();

        _initialPoint = null;
        _dragItem = null;
        _dragIndex = -1;
    }

    protected override void OnPointerCaptureLost(PointerCaptureLostEventArgs e)
    {
        base.OnPointerCaptureLost(e);

        // This is called as soon as DragDrop begins, make sure we don't do this in that
        // case because we're in the middle of doing something
        if (!_isInDrag)
        {
            if (_isInReorder)
                EndReorder();

            _initialPoint = null;
            _dragItem = null;
            _dragIndex = -1;
        }
    }

    private void UpdateDragInfo()
    {
        FAUISettings.GetSystemDragSize(VisualRoot.RenderScaling, out _cxDrag, out _cyDrag);
    }

    private void BeginReorder(PointerEventArgs args)
    {
        if (_dragItem == null)
        {
            // Invalid state, but seems to only happen if you use lightning quick reflexes 
            // to drag an item, so no error, but don't do anything
            return;
        }

        PseudoClasses.Set(s_pcReorder, true);
        // This triggers the ItemsPanel to enter reorder state which will insert a blank space
        // where the current item in measured
        (ItemsPanelRoot as TabViewStackPanel).EnterReorder(_dragIndex);
        _isInReorder = true;

        if (_dragReorderPopup == null)
        {
            _dragReorderPopup = new Popup
            {
                WindowManagerAddShadowHint = false,
                PlacementTarget = this,
                Placement = PlacementMode.Pointer
            };

            ((ISetLogicalParent)_dragReorderPopup).SetParent(this);
        }

        _dragReorderPopup.Child = new Rectangle
        {
            Fill = new VisualBrush
            {
                Visual = _dragItem,
                Stretch = Stretch.None
            },
            Width = _dragItem.Bounds.Width,
            Height = _dragItem.Bounds.Height,
            Opacity = 0.5
        };

        _popupOffset = -args.GetPosition(_dragItem);

        // Seems moving a popup window while in drag drop doesn't work
        // as despite the configure position stuff getting called,
        // the popup either remains in place or moves to the top of the
        // screen...TODO: File bug report, for now disabling preview popup
        // if dragdrop manages the drag/reorder operation
        if (!_isInDrag && FAUISettings.UseTabViewDragReorderPreview())
        {
            _dragReorderPopup.HorizontalOffset = _popupOffset.X;
            _dragReorderPopup.VerticalOffset = _popupOffset.Y;
            _dragReorderPopup.IsOpen = true;
        }

        // We need to clear the tooltip here otherwise it will end up showing
        // during the drag operation - and that's undesirable
        // We'll store it so we can restore it later
        _dragItemToolTip = ToolTip.GetTip(_dragItem);
        if (ToolTip.GetIsOpen(_dragItem))
        {
            ToolTip.SetIsOpen(_dragItem, false);
        }

        ToolTip.SetTip(_dragItem, null);

        // Capture the drag item so pointer events are transferred and will work outside of the window
        if (!_isInDrag)
            args.Pointer.Capture(_dragItem);
    }

    private void HandleReorder(Point panelPoint)
    {
        // No Visual effect on the drag source ListView
        if (!CanReorderItems && _isInDrag)
            return;

        int currentDragIndex = _dragIndex;

        if (ItemsPanelRoot.Bounds.Contains(panelPoint))
        {
            currentDragIndex = (ItemsPanelRoot as TabViewStackPanel)
                .GetInsertionIndexFromPoint(panelPoint, _dragIndex);
        }

        (ItemsPanelRoot as TabViewStackPanel).ChangeReorderIndex(currentDragIndex);

        if (!_isInDrag && _initialPoint.HasValue)
        {
            _dragReorderPopup.Host?.ConfigurePosition(this, PlacementMode.Pointer, _popupOffset);
        }
    }

    private void EndReorder()
    {
        PseudoClasses.Set(s_pcReorder, false);
        if (_dragReorderPopup != null && _dragReorderPopup.IsOpen)
        {
            _dragReorderPopup.IsOpen = false;
        }
        _isInReorder = false;
        var reorderIndex = (ItemsPanelRoot as TabViewStackPanel).ClearReorder();

        // if we're reordering this listview, we do that automatically - no need for the user
        // to handle this themselves
        if (reorderIndex != -1 && _processReorder)
        {
            if (ItemsSource is IList l && l.Count > 0)
            {
                lock (_lock)
                {
                    if (l.IsReadOnly)
                        return;

                    var oldItem = l[_dragIndex];
                    l.RemoveAt(_dragIndex);
                    l.Insert(reorderIndex, oldItem);
                }
            }

            SelectedIndex = reorderIndex;
            _processReorder = false;
        }

        // It seems if the ToolTip is open, changing its content leads to bad things
        // Make sure it's hidden before we do anything
        if (ToolTip.GetIsOpen(_dragItem))
        {
            ToolTip.SetIsOpen(_dragItem, false);
        }

        ToolTip.SetTip(_dragItem, _dragItemToolTip);
    }

    private async void BeginDragDrop(PointerEventArgs args, bool hasReorder)
    {
        // First fire DragItemsStarting
        var disArgs = new DragItemsStartingEventArgs
        {
            Items = new[] { ItemsView.GetAt(_dragIndex) },
            Data = new Data.DataPackage()
        };
        DragItemsStarting?.Invoke(this, disArgs);

        if (disArgs.Cancel)
        {
            _initialPoint = null;
            _dragItem = null;
            _dragIndex = -1;
            return;
        }

        _isInDrag = true;

        var effects = disArgs.Data.RequestedOperation;

        if (hasReorder)
        {
            _processReorder = true;
            BeginReorder(args);
            effects |= DragDropEffects.Move;
        }
        else
        {
            // We need to clear the tooltip here otherwise it will end up showing
            // during the drag operation - and that's undesirable
            // We'll store it so we can restore it later
            _dragItemToolTip = ToolTip.GetTip(_dragItem);
            if (ToolTip.GetIsOpen(_dragItem))
            {
                ToolTip.SetIsOpen(_dragItem, false);
            }
        }

        var dropResult =
            await DragDrop.DoDragDrop(args, disArgs.Data, effects);

        _isInDrag = false;
        if (hasReorder)
        {
            EndReorder();
        }
        else
        {
            (ItemsPanelRoot as TabViewStackPanel).ClearReorder();

            if (ToolTip.GetIsOpen(_dragItem))
            {
                ToolTip.SetIsOpen(_dragItem, false);
            }
            ToolTip.SetTip(_dragItem, _dragItemToolTip);
        }

        DragItemsCompleted?.Invoke(this, new DragItemsCompletedEventArgs(dropResult, disArgs.Items));

        _initialPoint = null;
        _dragItem = null;
        _dragIndex = -1;
    }

    private void OnDragEnter(object sender, DragEventArgs e)
    {
        // If dragging over a TabView strip that didn't initialize the DragDrop, and hasn't
        // receieved any drag interaction, this will still be null - initialize now
        //if (ItemsPanelRoot == null)
        //    ItemsPanelRoot = (TabViewStackPanel)Presenter.Panel;

        // If this is the TabViewListView spawning the DragDrop, so long as we can do drag drop,
        // enable it on this ListView when move over it
        if (_isInDrag)
        {
            e.DragEffects = DragDropEffects.Move | DragDropEffects.Copy | DragDropEffects.Link;
        }

        if (_isInReorder)
        {
            e.DragEffects |= DragDropEffects.Move;
            (ItemsPanelRoot as TabViewStackPanel).EnterReorder(_dragIndex);
        }
    }

    private void OnDragOver(object sender, DragEventArgs e)
    {
        DragOver?.Invoke(this, e);

        e.Handled = true;

        // Don't do any reorder processing until the user tells us its ok
        // However, if we're the drag source TabView, automatically allow
        // assuming we can - which _isInDrag will be true
        if (e.DragEffects == DragDropEffects.None && !_isInDrag)
            return;

        var scrollCheck = CheckAutoScroll(e.GetPosition(Scroller));

        switch (scrollCheck)
        {
            case AutoScrollAction.ScrollStarted:
                // All reorder logic is disabled when auto scrolling, so reset the panel
                // to draw normally
                (ItemsPanelRoot as TabViewStackPanel).ClearReorder();
                break;

            case AutoScrollAction.ScrollEnded:
                (ItemsPanelRoot as TabViewStackPanel).EnterReorder(_dragIndex);
                break;

            default:
                // No DragDrop, we're just reordering this ListView
                HandleReorder(e.GetPosition(ItemsPanelRoot));
                break;
        }
    }

    private void OnDragLeave(object sender, DragEventArgs e)
    {
        // This is a disgusting hack but we need to make sure we reset the insertion point
        // logic if we drag away from this ListView so we don't get left with a gap
        // The problem is, the event is raised on template components and not the actual
        // ListView - which means we don't actually know if we're leaving this control or
        // just a component (like a border that makes up the TabItem or its close button)
        // Searching the visual tree doesn't work because it will always return true to find
        // a TabViewListView. Searching for a TabViewItem and cancelling if found only works
        // if you drag leave over an emtpy space in the Tabstrip. If you drag over a tab and
        // into the main content area, that test fails, and the listview doesn't reset
        // Pointer events don't really work - and we don't even have that so we're caching 
        // the last point from DragOver. But pointer is only working here if we test against
        // the border, which means we slightly shrink the bounds of the listview and hit test
        // We could solve this easily with the raw PointerEvents from InputManager but that
        // shutsdown during drag drop, so there isn't really much we can do here. So hack it is.
        // There still seems to be an issue along the bottom border when dragging back in, but
        // at this point I'm done dealing with this issue...it works good enough
        if (e.Source is StyledElement v)
        {
            bool isCloseButton = (v as Visual).FindAncestorOfType<Button>() != null;
            if (isCloseButton)
                return;
            var pt = e.GetPosition(this);
            var rect = new Rect(Bounds.Size);
            rect = rect.Inflate(-2);
            if (!rect.Contains(pt))
            {
                if (_isInReorder)
                {
                    // Keep reorder active, but return to default state
                    (ItemsPanelRoot as TabViewStackPanel).ChangeReorderIndex(_dragIndex);
                }
                else
                {
                    (ItemsPanelRoot as TabViewStackPanel).ClearReorder();
                }
            }
        }
    }

    private void OnDrop(object sender, DragEventArgs e)
    {
        // Don't fire this event if we're the drag source
        if (_isInDrag)
            return;

        Drop?.Invoke(this, e);

        if (!_isInDrag)
        {
            (ItemsPanelRoot as TabViewStackPanel).ClearReorder();
        }

        _processReorder = true;
    }

    private AutoScrollAction CheckAutoScroll(Point scrollerPoint)
    {
        // We're not scrolling
        if (Scroller.HorizontalScrollBarVisibility != ScrollBarVisibility.Visible)
            return AutoScrollAction.NoAction;

        if (_scrollDirection == -1 && scrollerPoint.X > _noAutoScrollRect.X)
        {
            StopScrollTimer();
            return AutoScrollAction.ScrollEnded;
        }
        else if (_scrollDirection == 1 && scrollerPoint.X < _noAutoScrollRect.Right)
        {
            StopScrollTimer();
            return AutoScrollAction.ScrollEnded;
        }
        else if (!_noAutoScrollRect.Contains(scrollerPoint))
        {
            if (scrollerPoint.X < _noAutoScrollRect.X && Scroller.Offset.X > 0)
            {
                _scrollDirection = -1;
                StartScrollTimer();
                return AutoScrollAction.ScrollStarted;
            }
            else if (scrollerPoint.X > _noAutoScrollRect.Right &&
                Scroller.Offset.X < (Scroller.Extent.Width - Scroller.Viewport.Width))
            {
                _scrollDirection = 1;
                StartScrollTimer();
                return AutoScrollAction.ScrollStarted;
            }
        }

        // no scrolling needed
        return AutoScrollAction.NoAction;
    }

    private void StartScrollTimer()
    {
        if (_scrollTimer == null)
        {
            _scrollTimer = new DispatcherTimer(TimeSpan.FromMilliseconds(50),
                DispatcherPriority.Render, OnScrollTimerTick);
        }

        _scrollTimer.Start();
    }

    private void StopScrollTimer()
    {
        _scrollTimer?.Stop();
        _scrollDirection = 0;
    }

    private void OnScrollTimerTick(object sender, EventArgs e)
    {
        if (_scrollDirection == -1)
        {
            Scroller.Offset = new Vector(Scroller.Offset.X - 25, Scroller.Offset.Y);

            if (Scroller.Offset.X == 0)
            {
                StopScrollTimer();
            }
        }
        else if (_scrollDirection == 1)
        {
            Scroller.Offset = new Vector(Scroller.Offset.X + 25, Scroller.Offset.Y);

            if (Scroller.Offset.X == (Scroller.Extent.Width - Scroller.Viewport.Width))
            {
                StopScrollTimer();
            }
        }
    }

    private TabViewItem _dragItem;
    private int _dragIndex = -1;
    private bool _isInDrag = false;
    private bool _isInReorder = false;
    private bool _processReorder;
    private Point? _initialPoint;
    private double _cxDrag = double.NaN;
    private double _cyDrag = double.NaN;

    //private object _currentItem;

    private static Popup _dragReorderPopup;
    private Point _popupOffset;
    private object _dragItemToolTip;

    private DispatcherTimer _scrollTimer;
    private int _scrollDirection;
    private Rect _noAutoScrollRect;

    private const string s_tpScrollViewer = "ScrollViewer";

    private const string s_pcReorder = ":reorder";

    private enum AutoScrollAction
    {
        NoAction,
        ScrollStarted,
        ScrollEnded,
    }
}
