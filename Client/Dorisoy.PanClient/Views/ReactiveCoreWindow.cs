using Avalonia;
using Avalonia.Data;
using FluentAvalonia.UI.Windowing;

namespace Dorisoy.PanClient;

public class ReactiveCoreWindow<TViewModel> : AppWindow, IViewFor<TViewModel>, IViewFor, IActivatableView where TViewModel : class
{
    public static readonly AvaloniaProperty<TViewModel> ViewModelProperty = AvaloniaProperty.Register<ReactiveCoreWindow<TViewModel>, TViewModel>("ViewModel");

    public TViewModel ViewModel
    {
        get
        {
            return (TViewModel)((AvaloniaObject)(object)this).GetValue(ViewModelProperty);
        }
        set
        {
            ((AvaloniaObject)(object)this).SetValue(ViewModelProperty, value, BindingPriority.LocalValue);
        }
    }

    object IViewFor.ViewModel
    {
        get
        {
            return ViewModel;
        }
        set
        {
            ViewModel = (TViewModel)value;
        }
    }

    public ReactiveCoreWindow()
    {
        ((StyledElement)(object)this).DataContextChanged += delegate
        {
            ViewModel = ((StyledElement)(object)this).DataContext as TViewModel;
        };
    }
}
