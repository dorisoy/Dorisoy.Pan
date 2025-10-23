namespace Dorisoy.Pan.ViewModels;

public partial class ExceptionViewModel : ViewModelBase
{
    public Exception? Exception { get; set; }

    public string? Message => Exception?.Message;

    public string? ExceptionType => Exception?.GetType().Name ?? "";
}
