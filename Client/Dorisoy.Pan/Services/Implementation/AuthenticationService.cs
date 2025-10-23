namespace Dorisoy.Pan.Services;

public class AuthenticationService : IAuthenticationService
{
    private readonly MakeRequest _makeRequest;
    private static string _url;
    private IMapper _mapper;
    private readonly AppSettings _appSettings;

    public AuthenticationService(MakeRequest makeRequest, IMapper mapper, AppSettings appSettings)
    {
        _makeRequest = makeRequest;
        _mapper = mapper;
        _appSettings = appSettings;
        _url = _appSettings.GetHost();
    }

    /// <summary>
    /// 登录
    /// </summary>
    /// <param name="userName"></param>
    /// <param name="password"></param>
    /// <returns></returns>
    public async Task<UserAuthDto> LoginAsync(string userName, string password,string remoteIp, CancellationToken calToken = default)
    {
        try
        {
            var data = new UserAuthDto();
            var model = new LoginModel
            {
                UserName = userName,
                Password = password,
                RemoteIp = remoteIp,
            };

            var api = RefitServiceBuilder.Build<IAuthenticationApi>(_url, false);
            var result = await _makeRequest.Start(api.LoginAsync(model, calToken), calToken);
            if (result != null)
            {
                data = result;
                Globals.AccessToken = data?.BearerToken;
                Globals.IsAuthenticated = true;
            }

            return data;
        }
        catch (Exception e)
        {
            return null;
        }
    }


    /// <summary>
    /// 注销
    /// </summary>
    /// <returns></returns>
    public async Task<bool> LogOutAsync(CancellationToken calToken = default)
    {
        try
        {
            var userId = Globals.CurrentUser.Id;
            var token = new RevokeTokenRequest() { Token = Globals.AccessToken };
            var api = RefitServiceBuilder.Build<IAuthenticationApi>(_url);
            var result = await _makeRequest.Start(api.LogOutAsync(token, userId, calToken), calToken);
            if (result != null)
            {
                return true;
            }
            return false;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public async Task<UserAuthDto> RefreshTokenAsync(CancellationToken calToken = default)
    {
        try
        {
            var api = RefitServiceBuilder.Build<IAuthenticationApi>(_url, false);
            var result = await _makeRequest.Start(api.RefreshTokenAsync(Globals.AccessToken, calToken), calToken);
            if (result != null)
                return result;
            else
                return null;
        }
        catch (System.Net.Sockets.SocketException)
        {
            return null;
        }
        catch (Refit.ApiException)
        {
            return null;
        }
        catch (Exception)
        {
            return null;
        }
    }

}

