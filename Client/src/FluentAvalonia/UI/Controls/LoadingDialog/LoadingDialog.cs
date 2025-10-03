﻿using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Logging;
using Avalonia.Threading;
using Avalonia.VisualTree;
using FluentAvalonia.Core;

namespace FluentAvalonia.UI.Controls;


public partial class LoadingDialog : ContentControl, ICustomKeyboardNavigation
{
    public LoadingDialog(UserControl control)
    {
        PseudoClasses.Add(SharedPseudoclasses.s_pcHidden);
        Content = control;
    }


    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        //if (_primaryButton != null)
        //    _primaryButton.Click -= OnButtonClick;
        //if (_secondaryButton != null)
        //    _secondaryButton.Click -= OnButtonClick;
        //if (_closeButton != null)
        //    _closeButton.Click -= OnButtonClick;

        base.OnApplyTemplate(e);

        //_primaryButton = e.NameScope.Get<Button>(s_tpPrimaryButton);
        //_primaryButton.Click += OnButtonClick;
        //_secondaryButton = e.NameScope.Get<Button>(s_tpSecondaryButton);
        //_secondaryButton.Click += OnButtonClick;
        //_closeButton = e.NameScope.Get<Button>(s_tpCloseButton);
        //_closeButton.Click += OnButtonClick;

        // v2- Removed this as I don't think its necessary anymore (called from ShowAsync)
        //SetupDialog();
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);

        if (change.Property == FullSizeDesiredProperty)
        {
            OnFullSizedDesiredChanged(change);
        }
    }

    protected override bool RegisterContentPresenter(ContentPresenter presenter)
    {
        if (presenter.Name == "Content")
            return true;

        return base.RegisterContentPresenter(presenter);
    }

    protected override void OnKeyUp(KeyEventArgs e)
    {
        if (e.Handled)
        {
            base.OnKeyUp(e);
            return;
        }

        base.OnKeyUp(e);
    }


    public async Task<LoadingDialogResult> ShowAsync(Action<Action> close) => await ShowAsyncCore(null, close);


    public async Task<LoadingDialogResult> ShowAsync(Window w, Action<Action> close) => await ShowAsyncCore(w, close);


    private async Task<LoadingDialogResult> ShowAsyncCore(Window window, Action<Action> close, LoadingDialogPlacement placement = LoadingDialogPlacement.Popup)
    {
        if (placement == LoadingDialogPlacement.InPlace)
            throw new NotImplementedException("InPlace not implemented yet");


        //等待执行
        _tcs = new TaskCompletionSource<LoadingDialogResult>();

        OnOpening();

        if (Parent != null)
        {
            _originalHost = (Control)Parent;
            switch (_originalHost)
            {
                case Panel p:
                    _originalHostIndex = p.Children.IndexOf(this);
                    p.Children.Remove(this);
                    break;
                case Decorator d:
                    d.Child = null;
                    break;
                case ContentControl cc:
                    cc.Content = null;
                    break;
                case ContentPresenter cp:
                    cp.Content = null;
                    break;
            }
        }

        _host ??= new DialogHost();

        _host.Content = this;

        OverlayLayer ol = null;

        if (window != null)
        {
            ol = OverlayLayer.GetOverlayLayer(window);
            _lastFocus = window.FocusManager.GetFocusedElement();
        }
        else
        {
            if (Application.Current.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime al)
            {
                foreach (var item in al.Windows)
                {
                    if (item.IsActive)
                    {
                        window = item;
                        break;
                    }
                }

                //Fallback, just in case
                window ??= al.MainWindow;

                _lastFocus = window.FocusManager.GetFocusedElement();
                ol = OverlayLayer.GetOverlayLayer(window);
            }
            else if (Application.Current.ApplicationLifetime is ISingleViewApplicationLifetime sl)
            {
                _lastFocus = TopLevel.GetTopLevel(sl.MainView).FocusManager.GetFocusedElement();
                ol = OverlayLayer.GetOverlayLayer(sl.MainView);
            }
        }


        if (ol == null)
            throw new InvalidOperationException();

        ol.Children.Add(_host);

        IsVisible = true;

        ol.UpdateLayout();

        ShowCore();

        SetupDialog();

        //关闭回调
        close.Invoke(Hide);

        return await _tcs.Task;
    }

    /// <summary>
    /// Closes the current <see cref="LoadingDialog"/> without a result (<see cref="LoadingDialogResult"/>.<see cref="LoadingDialogResult.None"/>)
    /// </summary>
    public void Hide() => Hide(LoadingDialogResult.None);

    /// <summary>
    /// Closes the current <see cref="LoadingDialog"/> with the given <see cref="LoadingDialogResult"/> <para>ddd</para>
    /// </summary>
    /// <param name="dialogResult">The <see cref="LoadingDialogResult"/> to return</param>
    public void Hide(LoadingDialogResult dialogResult)
    {
        _result = dialogResult;
        HideCore();
    }

    /// <summary>
    /// Called when the primary button is invoked
    /// </summary>
    protected virtual void OnPrimaryButtonClick(LoadingDialogButtonClickEventArgs args)
    {
        PrimaryButtonClick?.Invoke(this, args);
    }

    /// <summary>
    /// Called when the secondary button is invoked
    /// </summary>
    protected virtual void OnSecondaryButtonClick(LoadingDialogButtonClickEventArgs args)
    {
        SecondaryButtonClick?.Invoke(this, args);
    }

    /// <summary>
    /// Called when the close button is invoked
    /// </summary>
    protected virtual void OnCloseButtonClick(LoadingDialogButtonClickEventArgs args)
    {
        CloseButtonClick?.Invoke(this, args);
    }

    /// <summary>
    /// Called when the LoadingDialog is requested to be opened
    /// </summary>
    protected virtual void OnOpening()
    {
        Opening?.Invoke(this, EventArgs.Empty);
    }

    /// <summary>
    /// Called after the LoadingDialog is initialized but just before its presented on screen
    /// </summary>
    protected virtual void OnOpened()
    {
        Opened?.Invoke(this, EventArgs.Empty);
    }

    /// <summary>
    /// Called when the LoadingDialog has been requested to close, but before it actually closes
    /// </summary>
    /// <param name="args"></param>
    protected virtual void OnClosing(LoadingDialogClosingEventArgs args)
    {
        Closing?.Invoke(this, args);
    }

    /// <summary>
    /// Called when the LoadingDialog has been closed and removed from the tree
    /// </summary>
    protected virtual void OnClosed(LoadingDialogClosedEventArgs args)
    {
        Closed?.Invoke(this, args);
    }

    private void ShowCore()
    {
        IsVisible = true;
        PseudoClasses.Set(SharedPseudoclasses.s_pcHidden, false);
        PseudoClasses.Set(SharedPseudoclasses.s_pcOpen, true);

        OnOpened();
    }

    private void HideCore()
    {
        // v2 - No longer disabling the dialog during a deferral so we need to make sure that if
        //      multiple requests to close come in, we don't handle them
        if (_hasDeferralActive)
            return;

        // v2- Changed to match logic in TeachingTip for deferral, fixing #239 where cancel
        //     was being handled before the deferral.
        var args = new LoadingDialogClosingEventArgs(_result);

        var deferral = new Deferral(() =>
        {
            Dispatcher.UIThread.VerifyAccess();
            _hasDeferralActive = false;

            if (!args.Cancel)
            {
                FinalCloseDialog();
            }
        });

        args.SetDeferral(deferral);
        _hasDeferralActive = true;

        args.IncrementDeferralCount();
        OnClosing(args);
        args.DecrementDeferralCount();
    }

    // Internal only for UnitTests
    internal void SetupDialog()
    {
        if (_primaryButton == null)
            ApplyTemplate();

        //PseudoClasses.Set(s_pcPrimary, !string.IsNullOrEmpty(PrimaryButtonText));
        //PseudoClasses.Set(s_pcSecondary, !string.IsNullOrEmpty(SecondaryButtonText));
        //PseudoClasses.Set(s_pcClose, !string.IsNullOrEmpty(CloseButtonText));

        var curFocus = TopLevel.GetTopLevel(this).FocusManager.GetFocusedElement() as Control;
        bool setFocus = false;
        if (curFocus.FindAncestorOfType<LoadingDialog>() == null)
        {
            setFocus = true;
        }

        var p = Presenter;
        switch (DefaultButton)
        {
            case LoadingDialogButton.Primary:
                break;
            case LoadingDialogButton.Secondary:
                break;
            case LoadingDialogButton.Close:
                break;
            default:
                break;
        }
    }

    // This is the exit point for the LoadingDialog
    // This method MUST be called to finalize everything
    private async void FinalCloseDialog()
    {
        // Prevent interaction when closing...double/mutliple clicking on the buttons to close
        // the dialog was calling this multiple times, which would cause the OverlayLayer check
        // below to fail (as this would be removed from the tree). This is a simple workaround
        // to make sure we don't error out
        IsHitTestVisible = false;

        // For a better experience when animating closed, we need to make sure the
        // focus adorner is not showing (if using keyboard) otherwise that will hang
        // around and not fade out and it just looks weird. So focus this to force the
        // adorner to hide, then continue forward.
        Focus();

        PseudoClasses.Set(SharedPseudoclasses.s_pcHidden, true);
        PseudoClasses.Set(SharedPseudoclasses.s_pcOpen, false);

        // Let the close animation finish (now 0.167s in new WinUI update...)
        // We'll wait just a touch longer to be sure
        await Task.Delay(200);

        OnClosed(new LoadingDialogClosedEventArgs(_result));

        if (_lastFocus != null)
        {
            _lastFocus.Focus(NavigationMethod.Unspecified);
            _lastFocus = null;
        }

        var ol = OverlayLayer.GetOverlayLayer(_host);
        // If OverlayLayer isn't found here, this may be a reentrant call (hit ESC multiple times quickly, etc)
        // Don't fail, and return. If this isn't reentrant, there's bigger issues...
        if (ol == null)
            return;

        ol.Children.Remove(_host);

        _host.Content = null;

        if (_originalHost != null)
        {
            if (_originalHost is Panel p)
            {
                p.Children.Insert(_originalHostIndex, this);
            }
            else if (_originalHost is Decorator d)
            {
                d.Child = this;
            }
            else if (_originalHost is ContentControl cc)
            {
                cc.Content = this;
            }
            else if (_originalHost is ContentPresenter cp)
            {
                cp.Content = this;
            }
        }

        _tcs.TrySetResult(_result);
    }

    private void OnButtonClick(object sender, RoutedEventArgs e)
    {
        // v2 - No longer disabling the dialog during a deferral so we need to make sure that if
        //      multiple requests to close come in, we don't handle them
        if (_hasDeferralActive)
            return;

        var args = new LoadingDialogButtonClickEventArgs();

        var deferral = new Deferral(() =>
        {
            Dispatcher.UIThread.VerifyAccess();
            _hasDeferralActive = false;

            if (args.Cancel)
                return;

            if (sender == _primaryButton)
            {
                if (PrimaryButtonCommand != null && PrimaryButtonCommand.CanExecute(PrimaryButtonCommandParameter))
                {
                    PrimaryButtonCommand.Execute(PrimaryButtonCommandParameter);
                }
                _result = LoadingDialogResult.Primary;
            }
            else if (sender == _secondaryButton)
            {
                if (SecondaryButtonCommand != null && SecondaryButtonCommand.CanExecute(SecondaryButtonCommandParameter))
                {
                    SecondaryButtonCommand.Execute(SecondaryButtonCommandParameter);
                }
                _result = LoadingDialogResult.Secondary;
            }
            else if (sender == _closeButton)
            {
                if (CloseButtonCommand != null && CloseButtonCommand.CanExecute(CloseButtonCommandParameter))
                {
                    CloseButtonCommand.Execute(CloseButtonCommandParameter);
                }
                _result = LoadingDialogResult.None;
            }

            HideCore();
        });

        args.SetDeferral(deferral);
        _hasDeferralActive = true;

        args.IncrementDeferralCount();
        if (sender == _primaryButton)
        {
            OnPrimaryButtonClick(args);
        }
        else if (sender == _secondaryButton)
        {
            OnSecondaryButtonClick(args);
        }
        else if (sender == _closeButton)
        {
            OnCloseButtonClick(args);
        }
        args.DecrementDeferralCount();
    }

    private void OnFullSizedDesiredChanged(AvaloniaPropertyChangedEventArgs e)
    {
        bool newVal = (bool)e.NewValue;
        PseudoClasses.Set(s_pcFullSize, newVal);
    }

    public (bool handled, IInputElement next) GetNext(IInputElement element, NavigationDirection direction)
    {
        var children = this.GetVisualDescendants().OfType<IInputElement>()
            .Where(x => KeyboardNavigation.GetIsTabStop((InputElement)x) && x.Focusable &&
            x.IsEffectivelyVisible && IsEffectivelyEnabled).ToList();

        if (children.Count == 0)
            return (false, null);

        var current = TopLevel.GetTopLevel(this).FocusManager.GetFocusedElement();
        if (current == null)
            return (false, null);

        if (direction == NavigationDirection.Next)
        {
            for (int i = 0; i < children.Count; i++)
            {
                if (children[i] == current)
                {
                    if (i == children.Count - 1)
                    {
                        return (true, children[0]);
                    }
                    else
                    {
                        return (true, children[i + 1]);
                    }
                }
            }
        }
        else if (direction == NavigationDirection.Previous)
        {
            for (int i = children.Count - 1; i >= 0; i--)
            {
                if (children[i] == current)
                {
                    if (i == 0)
                    {
                        return (true, children[children.Count - 1]);
                    }
                    else
                    {
                        return (true, children[i - 1]);
                    }
                }
            }
        }

        return (false, null);
    }


    // Store the last element focused before showing the dialog, so we can
    // restore it when it closes
    private IInputElement _lastFocus;
    private Control _originalHost;
    private int _originalHostIndex;
    private DialogHost _host;
    private LoadingDialogResult _result;
    private TaskCompletionSource<LoadingDialogResult> _tcs;
    private Button _primaryButton;
    private Button _secondaryButton;
    private Button _closeButton;
    private bool _hasDeferralActive;
}
