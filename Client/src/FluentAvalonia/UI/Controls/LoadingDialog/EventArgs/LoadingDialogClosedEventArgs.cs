namespace FluentAvalonia.UI.Controls;

/// <summary>
/// Provides data for the Closed event.
/// </summary>
public class LoadingDialogClosedEventArgs : EventArgs
{
    /// <summary>
    /// Gets the <see cref="LoadingDialogResult"/> of the button click event.
    /// </summary>
    public LoadingDialogResult Result { get; }

    internal LoadingDialogClosedEventArgs(LoadingDialogResult res)
    {
        Result = res;
    }
}
