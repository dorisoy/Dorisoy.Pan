namespace Dorisoy.Pan.Services;

public class OnlineUserService : IOnlineUserService
{
    private readonly MakeRequest _makeRequest;
    private static string _url;
    private IMapper _mapper;
    private readonly AppSettings _appSettings;

    public OnlineUserService(MakeRequest makeRequest, IMapper mapper, AppSettings appSettings)
    {
        _makeRequest = makeRequest;
        _mapper = mapper;
        _appSettings = appSettings;
        _url = _appSettings.GetHost();
    }

    public async Task<List<OnlinUserUserModel>> GetOnlineUsers(CancellationToken calToken = default)
    {
        try
        {
            var api = RefitServiceBuilder.Build<IUserApi>(_url, false);
            var result = await _makeRequest.Start(api.GetOnlineUsers(calToken), calToken);
            var users = _mapper.ProjectTo<OnlinUserUserModel>(result.AsQueryable());
            return users.ToList();
        }
        catch (Exception e)
        {
            return null;
        }
    }

}

