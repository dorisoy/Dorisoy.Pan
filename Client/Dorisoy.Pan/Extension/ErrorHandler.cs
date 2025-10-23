namespace Dorisoy.Pan;

public static class AsyncErrorHandler
{
    private static bool IsAuthenticated = true;

    /// <summary>
    /// 处理异常提示
    /// </summary>
    /// <param name="ex"></param>
    public async static void Handle(this Exception ex)
    {
        await Dispatcher.UIThread.InvokeAsync(() =>
        {
            var resultHint = new ContentDialog()
            {
                Content = ex.Message,
                Title = "提示",
                PrimaryButtonText = "确认"
            };
            _ = resultHint.ShowAsync();
        });
    }

    /// <summary>
    /// 处理异常提示
    /// </summary>
    /// <param name="ex"></param>
    public async static void HandleException(this Exception ex)
    {
        await Dispatcher.UIThread.InvokeAsync(() =>
        {
            if (ex is OperationCanceledException)
            {
                var resultHint = new ContentDialog()
                {
                    Content = ex.Message,
                    Title = "提示",
                    PrimaryButtonText = "确认"
                };
                _ = resultHint.ShowAsync();
            }
            else if (ex is TaskCanceledException)
            {
                var resultHint = new ContentDialog()
                {
                    Content = ex.Message,
                    Title = "提示",
                    PrimaryButtonText = "确认"
                };
                _ = resultHint.ShowAsync();
            }
            else if (ex is Refit.ApiException s)
            {
                if (IsAuthenticated && s.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    IsAuthenticated = false;

                    //账户失效", "账户会话已经过期，请重新登录！

                    var resultHint = new ContentDialog()
                    {
                        Content = s.Message,
                        Title = "提示",
                        PrimaryButtonText = "确认"
                    };
                    _ = resultHint.ShowAsync();
                }
            }
        });
    }
}
