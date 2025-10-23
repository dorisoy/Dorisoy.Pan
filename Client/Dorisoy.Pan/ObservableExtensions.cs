using Microsoft.Extensions.Logging;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace Dorisoy.Pan;

public static class ObservableExtensions
{
    /// <summary>
    ///将错误发送到stderr
    /// </summary>
    public static IDisposable SubscribeWithLog<T>(this IObservable<T> observable, Action<T> onNext, Action onCompleted)
    {
        var logger = Locator.Current.GetService<ILogger>();

        return observable.Subscribe(
            onNext,
            e => logger.LogError(e, "Unhandled exception occured on observable"),
            onCompleted);
    }

    /// <summary>
    /// 将错误发送到stderr
    /// </summary>
    public static IDisposable SubscribeWithLog<T>(this IObservable<T> observable, Action onCompleted)
    {
        var logger = Locator.Current.GetService<ILogger>();

        return observable.Subscribe(
            _ => { },
            e => logger.LogError(e, "Unhandled exception occured on observable"),
            onCompleted);
    }

    /// <summary>
    /// 将错误发送到stderr
    /// </summary>
    public static IDisposable SubscribeWithLog<T>(this IObservable<T> observable, Action<T> onNext)
    {
        var logger = Locator.Current.GetService<ILogger>();

        return observable.Subscribe(
            onNext,
            e => logger.LogError(e, "Unhandled exception occured on observable"));
    }

    /// <summary>
    /// 将错误发送到stderr
    /// </summary>
    public static IDisposable SubscribeWithLog<T>(this IObservable<T> observable)
    {
        var logger = Locator.Current.GetService<ILogger>();

        return observable.Subscribe(
            _ => { },
            e => logger.LogError(e, "Unhandled exception occured on observable"));
    }
}
