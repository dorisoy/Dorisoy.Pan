using System.Net.Http.Headers;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using Refit;

namespace Dorisoy.Pan;

public class RefitServiceBuilder
{
    protected static readonly RefitSettings _refitSettings = new RefitSettings
    {
        ContentSerializer = new NewtonsoftJsonContentSerializer(new JsonSerializerSettings
        {
            ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor,
            ContractResolver = new CamelCasePropertyNamesContractResolver(),
            Converters = { new StringEnumConverter() }
        })
    };

    protected static HttpClient _httpClient;


    /// <summary>
    /// 负责定位发送请求的接口的媒介
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="url"></param>
    /// <returns></returns>
    public static T Build<T>(string url, bool useToken = true) where T : class
    {
        if (useToken && Globals.IsAuthenticated)
        {
            _httpClient = new HttpClient(new AuthenticatedHttpClientHandler(GetToken))
            {
                BaseAddress = new Uri(url)
            };
        }
        else
        {
            _httpClient = new HttpClient()
            {
                BaseAddress = new Uri(url)
            };
        }
        return RestService.For<T>(_httpClient, _refitSettings);
    }

    private async static Task<string> GetToken()
    {
        // 如果需要，AquireTokenAsync调用将提示用户界面
        // 否则默认使用刷新令牌返回一个有效的访问令牌
        //var token = await context.AcquireTokenAsync("http://my.service.uri/app", "clientId", 
        //    new Uri("callback://complete"));
        //return token;

        try
        {
            //if (string.IsNullOrEmpty(Globals.AccessToken))
            //{
            //    var _auth = Locator.Current.GetService<IAuthenticationService>();
            //    var result = await _auth.RefreshTokenAsync();
            //    if (result != null)
            //    {
            //        if (!string.IsNullOrEmpty(result.BearerToken))
            //        {
            //            Globals.AccessToken = result.BearerToken;
            //        }
            //        else
            //        {
            //            Globals.AccessToken = "";
            //            Globals.IsAuthenticated = false;
            //        }
            //    }
            //}

            return await Task.FromResult(Globals.AccessToken);
        }
        catch (Exception)
        {
            return await Task.FromResult(Globals.AccessToken);
        }
    }

    public static string Cacher(string name, params object[] args)
    {
        var cacheKey = $"{name}.{string.Join(".", args)}";
        return cacheKey;
    }
}

public class AuthenticatedHttpClientHandler : HttpClientHandler
{
    private readonly Func<Task<string>> getToken;
    public AuthenticatedHttpClientHandler(Func<Task<string>> getToken)
    {
        if (getToken != null)
            this.getToken = getToken;
    }
    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        try
        {
            var auth = request.Headers.Authorization;
            if (auth != null && Globals.IsAuthenticated)
            {
                var token = await getToken().ConfigureAwait(false);
                request.Headers.Authorization = new AuthenticationHeaderValue(auth.Scheme, token);
            }
        }
        catch (Exception) { }

        return await base.SendAsync(request, cancellationToken).ConfigureAwait(false);
    }


}
