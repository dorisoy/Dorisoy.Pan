using System.Net;
using Polly;
using Polly.Retry;


namespace Dorisoy.PanClient;

/// <summary>
/// 负责网络请求服务
/// </summary>
public class MakeRequest
{
    private readonly ILogger _logger;

    //private CompositeDisposable disposer { get; } = new CompositeDisposable();
    public MakeRequest(ILogger logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Polly策略
    ///  ------------------------------------
    /// WaitAndRetryAsync: 该策略将等待并重试retryCount次。在每次重试时，通过使用当前重试号码调用sleepDurationProvider（1表示第一次重试，2表示第二次重试等）来计算等待时间。
    ///  ------------------------------------
    /// CircuitBreaker: 当系统遇到严重的问题时，快速回馈失败比让用户/调用者等待要好，限制系统出错的消耗，有助于系统恢复，比如，当我们去调用一个第三方的API，有很长一段时间API都没有响应，可能对方服务器瘫痪了，如果我们的系统还不停地重试，不公会加重系统的负担，还有可能导致系统其它任务受影响，因此，当系统出错的次数超过了指定的阈值，就是中断当前线程，等待一段时间后再继续
    /// ------------------------------------
    /// Timeout: 当系统超时一定时间的等待，就位就可以判断不可能会有成功的结果；比如平时一个网络请求瞬间就完成了，如果有一次网络请求超过了30秒还没有完成，我们就可以判定不可能会返回成功的结果了，因此，我们需要设置系统的超时时间，避免系统长时间就无谓的等待
    ///  ------------------------------------
    ///  Fallback:有些错误无法避免，就要有备用的方案，当无法避免的错误发生时，我们要有一个合理的返回来代替失败
    ///  ------------------------------------
    ///  Bulkhead Isolation:当系统的一处出现故障时，可能触发多个失败的调用，对资源有较大的消耗，下游系统出现故障可能导致上游的故障的调用，甚至可能蔓延到导致系统崩溃，所以要将可控的操作限制在一个固定大小的资源池中，以隔离有潜在可能相互影响的操作
    ///  ------------------------------------
    ///  Cache:一般我们会把频繁使用且不会怎么变化 的资源缓存起来，以提高系统的响应速度，如果不对缓存资源的调用进行封装，那么我们调用的时候就要先判断缓存中有没有这个资源，有的话就从缓存返回，否则就从资源存储的地方获取后缓存起来再返回，而且有时还要考虑缓存过期和如何更新缓存的问题
    ///  ------------------------------------ 
    ///  Policy Wrap:一种操作会有多种不同的故障，而不同的故障处理需要不同的策略，这些不同的策略必须包在一起，作为一个策略包，才能应用在同一种操作上，这就是Polly的弹性特性，即各种不同的策略能够灵活地组合起来
    /// </summary>
    //protected readonly AsyncRetryPolicy _policy = Policy
    //    .Handle<HttpRequestException>()
    //    .WaitAndRetryAsync(retryCount: 3, sleepDurationProvider: (attempt) => TimeSpan.FromSeconds(5));


    private AsyncRetryPolicy _policy
    {
        get
        {
            //HttpStatusCode[] retryStatus =
            //{
            //    //404
            //    HttpStatusCode.NotFound,
            //    //503
            //    HttpStatusCode.ServiceUnavailable,
            //    //408
            //    HttpStatusCode.RequestTimeout
            //};

            return Policy
               .Handle<HttpRequestException>()
               .WaitAndRetryAsync(new[]
                {
                    // 表示重试3次，第一次1秒后重试，第二次2秒后重试，第三次4秒后重试
                    TimeSpan.FromSeconds(1),
                    TimeSpan.FromSeconds(5),
                    TimeSpan.FromSeconds(8)
                }, (result, span, count, context) =>
                {
                    _logger.Write($"Policy: Retry delegate fired, attempt {count}", LogLevel.Info);
                });
        }
    }


    /// <summary>
    /// 直接执行请求的方法（不使用缓存）
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="task"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    public async Task<T> Start<T>(Task<T> task, CancellationToken token = default)
    {
        try
        {
            var ret = await _policy.ExecuteAsync(async (token) =>
            {
                try
                {
                    var t = await task;
                    return t;
                }
                catch (Exception ex)
                {
                    _logger.Write(ex.Message, LogLevel.Error);
                    return default;
                }
            }, token);

            return ret;
        }
        catch (HttpRequestException)
        {
            return await Task.FromResult(default(T));
        }
        catch (Refit.ApiException ae)
        {
            if (ae.StatusCode == HttpStatusCode.Unauthorized)
            {
                Globals.AccessToken = "";
                Globals.IsAuthenticated = false;
            }
            return await Task.FromResult(default(T));
        }
        catch (Exception)
        {
            return await Task.FromResult(default(T));
        }
    }

    ///// <summary>
    ///// 缓存执行请求的方法,如果有缓存则从缓存中读取旧数据，然后更新新数据到缓存
    ///// </summary>
    ///// <typeparam name="T"></typeparam>
    ///// <param name="task"></param>
    ///// <param name="cacheKey">缓存键</param>
    ///// <param name="force">是否强制缓存</param>
    ///// <param name="token"></param>
    ///// <returns></returns>
    //public async Task<T> StartUseCache<T>(Task<T> task, string cacheKey, bool force = false, CancellationToken token = default, TimeSpan? span = null)
    //{
    //    try
    //    {

    //        //var current = Connectivity.NetworkAccess;
    //        //if (current != NetworkAccess.Internet)
    //        //{
    //        //    return await Task.FromResult(default(T));
    //        //}

    //        /*
    //        BlobCache.LocalMachine  –常规缓存的数据。从服务器检索的对象，即新闻项，用户列表等。
    //        BlobCache.UserAccount  –用户在您的应用程序中执行的设置。一些平台将此同步到云。
    //        BlobCache.Secure  –如果要保存敏感数据，则在此放置数据。
    //        BlobCache.InMemory  –顾名思义；每当您的应用被杀死时，它就会将数据保存在内存中，其中的数据也将保留在内存中。
    //         */

    //        //var cache = BlobCache.LocalMachine;

    //        ////如果强制获取
    //        //if (force)
    //        //{
    //        //    //删除缓存
    //        //    await cache.Invalidate(cacheKey);
    //        //}

    //        if (span == null)
    //            span = new TimeSpan(hours: 0, minutes: 0, seconds: 5);

    //        //GetAndFetchLatest 此方法尝试返回缓存值，同时调用Func以返回最新值。当最新数据返回时，它将替换缓存中以前的数据。此方法最适合从Internet加载动态数据，同时仍然显示用户早期的数据。此方法返回一个IObservable，该IObservable可能返回*两个*结果（首先是缓存数据，然后是最新数据）。因此，在Subscribe方法中，编写代码以在第二个结果出现时合并第二个结果，这对于UI应用程序非常重要。这也意味着等待此方法是一个坏主意，请始终使用Subscribe。
    //        IObservable<T> cachedConferences = cache.GetAndFetchLatest(cacheKey, async () =>
    //        {
    //            return await _policy.ExecuteAsync(async (token) =>
    //            {
    //                return await task;
    //            }, token);
    //        },
    //        offset =>
    //        {
    //            TimeSpan elapsed = DateTimeOffset.Now - offset;
    //            return elapsed > span;
    //        });

    //        var result = await cachedConferences.FirstOrDefaultAsync();
    //        if (result == null)
    //        {
    //            return await _policy.ExecuteAsync(async (token) =>
    //            {
    //                try
    //                {
    //                    return await task;
    //                }
    //                catch (Exception ex)
    //                {
    //                    return default;
    //                }
    //            }, token);
    //        }

    //        return result;
    //    }
    //    catch (HttpRequestException)
    //    {
    //        return await Task.FromResult(default(T));
    //    }
    //    catch (KeyNotFoundException)
    //    {
    //        return await Task.FromResult(default(T));
    //    }
    //    catch (TaskCanceledException)
    //    {
    //        return await Task.FromResult(default(T));
    //    }
    //    catch (ObjectDisposedException)
    //    {
    //        return await Task.FromResult(default(T));
    //    }
    //    catch (Refit.ApiException ae)
    //    {
    //        if (ae.StatusCode == HttpStatusCode.Unauthorized)
    //        {
    //            Globals.AccessToken = "";
    //            Globals.IsAuthenticated = false;
    //        }
    //        return await Task.FromResult(default(T));
    //    }
    //    catch (Exception)
    //    {
    //        return await Task.FromResult(default(T));
    //    }
    //}

    ///// <summary>
    ///// 缓存执行请求的方法,如果有缓存则从缓存中读取旧数据，然后更新新数据到缓存
    ///// </summary>
    ///// <typeparam name="T"></typeparam>
    ///// <param name="task"></param>
    ///// <param name="cacheKey"></param>
    ///// <param name="token"></param>
    ///// <param name="span"></param>
    ///// <returns></returns>
    //public IObservable<T> StartUseCache_Rx<T>(Task<T> task, string cacheKey, CancellationToken token = default, TimeSpan? span = null)
    //{
    //    try
    //    {
    //        //var current = Connectivity.NetworkAccess;
    //        //if (current != NetworkAccess.Internet)
    //        //{
    //        //    return default;
    //        //}

    //        var cache = BlobCache.LocalMachine;

    //        var tmp = cache.GetObject<T>(cacheKey);

    //        //缓存过期时间8小时
    //        if (span == null)
    //            span = new TimeSpan(hours: 0, minutes: 0, seconds: 5);

    //        //GetAndFetchLatest 此方法尝试返回缓存值，同时调用Func以返回最新值。当最新数据返回时，它将替换缓存中以前的数据。此方法最适合从Internet加载动态数据，同时仍然显示用户早期的数据。此方法返回一个IObservable，该IObservable可能返回*两个*结果（首先是缓存数据，然后是最新数据）。因此，在Subscribe方法中，编写代码以在第二个结果出现时合并第二个结果，这对于UI应用程序非常重要。这也意味着等待此方法是一个坏主意，请始终使用Subscribe。

    //        // fetchFunc: 将获取任务的方法。
    //        // fetchPredicate:一个可选Func，用于确定是否应提取更新的项。如果找不到缓存的版本，则忽略此参数并始终获取该项。
    //        // absoluteExpiration: 可选的到期日期.
    //        // shouldInvalidateOnError:如果为True，则当fetchFunc中发生异常时，缓存将被清除。
    //        // cacheValidationPredicate:一个可选Func，用于确定是否应缓存获取的值.

    //        IObservable<T> cachedConferences = cache.GetAndFetchLatest(cacheKey, async () =>
    //        {
    //            var data = await _policy.ExecuteAsync(async (token) =>
    //            {
    //                 //var current = Connectivity.NetworkAccess;
    //                 //if (current != NetworkAccess.Internet)
    //                 //{
    //                 //    return default;
    //                 //}

    //                 try
    //                 {
    //                     return await task;
    //                 }
    //                 catch (Exception ex)
    //                 {
    //                     return default;
    //                 }
    //             }, token);

    //            return data;
    //        },
    //        offset =>
    //        {
    //            TimeSpan elapsed = DateTimeOffset.Now - offset;
    //            return elapsed > span;
    //        });

    //        return cachedConferences;
    //    }
    //    catch (HttpRequestException)
    //    {
    //        return default;
    //    }
    //    catch (KeyNotFoundException)
    //    {
    //        return default;
    //    }
    //    catch (TaskCanceledException)
    //    {
    //        return default;
    //    }
    //    catch (ObjectDisposedException)
    //    {
    //        return default;
    //    }
    //    catch (Refit.ApiException ae)
    //    {
    //        if (ae.StatusCode == HttpStatusCode.Unauthorized)
    //        {
    //            Globals.AccessToken = "";
    //            Globals.IsAuthenticated = false;
    //        }
    //        return default;
    //    }
    //    catch (Exception)
    //    {
    //        return default;
    //    }
    //}
}
