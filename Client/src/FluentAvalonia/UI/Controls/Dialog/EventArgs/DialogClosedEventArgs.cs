namespace FluentAvalonia.UI.Controls;

/// <summary>
/// Provides data for the Closed event.
/// </summary>
public class DialogClosedEventArgs : EventArgs
{
    /// <summary>
    /// Gets the <see cref="DialogResult"/> of the button click event.
    /// </summary>
    public DialogResult Result { get; }

    internal DialogClosedEventArgs(DialogResult res)
    {
        Result = res;
    }
}
