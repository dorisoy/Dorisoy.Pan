using ReactiveUI;
using Avalonia.Controls;

namespace FluentAvalonia.UI.Controls;

public interface IFrameNavigatedFrom : IActivatableViewModel
{
    public UserControl Host { get; set; }
}
