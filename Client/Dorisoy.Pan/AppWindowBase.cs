using AsyncAwaitBestPractices;
using Dorisoy.Pan.ViewModels;

namespace Dorisoy.Pan;

//[SuppressMessage("ReSharper", "VirtualMemberNeverOverridden.Global")]
public class AppWindowBase : AppWindow
{
    public CancellationTokenSource? ShowAsyncCts { get; set; }

    protected AppWindowBase()
    {
    }

    public void ShowWithCts(CancellationTokenSource cts)
    {
        ShowAsyncCts?.Cancel();
        ShowAsyncCts = cts;
        Show();
    }

    public Task ShowAsync()
    {
        ShowAsyncCts?.Cancel();
        ShowAsyncCts = new CancellationTokenSource();

        var tcs = new TaskCompletionSource<bool>();
        ShowAsyncCts.Token.Register(s =>
        {
            ((TaskCompletionSource<bool>)s!).SetResult(true);
        }, tcs);

        Show();

        return tcs.Task;
    }

    protected override void OnClosed(EventArgs e)
    {
        base.OnClosed(e);

        if (ShowAsyncCts is not null)
        {
            ShowAsyncCts.Cancel();
            ShowAsyncCts = null;
        }
    }

    protected override void OnLoaded(RoutedEventArgs e)
    {
        base.OnLoaded(e);

        if (DataContext is ViewModelBase viewModel)
        {
            // 运行同步加载然后异步卸载
            viewModel.OnLoaded();

            // 无法在此处阻止，因此我们将在UI线程上以异步方式运行
            Dispatcher.UIThread.InvokeAsync(async () =>
            {
                await viewModel.OnLoadedAsync();

            }).SafeFireAndForget();
        }
    }

    protected override void OnUnloaded(RoutedEventArgs e)
    {
        base.OnUnloaded(e);

        if (DataContext is not ViewModelBase viewModel)
            return;

        // 运行同步加载然后异步卸载
        //viewModel.OnUnloaded();

        //// 无法在此处阻止，因此我们将在UI线程上以异步方式运行
        //Dispatcher.UIThread.Invoke( () =>
        //{

        //}).SafeFireAndForget();
    }
}
