namespace Dorisoy.PanClient.ViewModels;

[View(typeof(ExceptionDialog))]
public partial class ExceptionViewModel : ViewModelBase
{
    public Exception? Exception { get; set; }

    public string? Message => Exception?.Message;

    public string? ExceptionType => Exception?.GetType().Name ?? "";
    protected override void DisposeManaged()
    {
    }

    protected override void DisposeUnmanaged()
    {

    }
}
