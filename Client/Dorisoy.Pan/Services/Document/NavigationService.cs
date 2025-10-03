using FluentAvalonia.UI.Media.Animation;
using Frame = FluentAvalonia.UI.Controls.Frame;

namespace Dorisoy.PanClient.Services;


/// <summary>
/// 导航服务
/// </summary>
public class NavigationService
{
    private Frame _frame;
    private Panel _overlayHost;

    public static NavigationService Instance { get; } = new NavigationService();

    /// <summary>
    /// 上一页
    /// </summary>
    public Control PreviousPage { get; set; }

    public void SetFrame(Frame f)
    {
        _frame = f;
    }

    public void SetOverlayHost(Panel p)
    {
        _overlayHost = p;
    }

    public void Navigate(Type t)
    {
        _frame.Navigate(t);
    }

    public void NavigateFromContext(object dataContext, NavigationTransitionInfo transitionInfo = null, UserControl mainView = null)
    {
        _frame.NavigateFromObject(dataContext,
            new FluentAvalonia.UI.Navigation.FrameNavigationOptions
            {
                IsNavigationStackEnabled = true,
                TransitionInfoOverride = transitionInfo ?? new SuppressNavigationTransitionInfo()
            }, mainView);
    }

    public void ShowControlDefinitionOverlay(Type targetType)
    {
        if (_overlayHost != null)
        {
            (_overlayHost.Children[0] as ControlDefinitionOverlay).TargetType = targetType;
            (_overlayHost.Children[0] as ControlDefinitionOverlay).Show();
        }
    }

    public void ClearOverlay()
    {
        _overlayHost?.Children.Clear();
    }
}


