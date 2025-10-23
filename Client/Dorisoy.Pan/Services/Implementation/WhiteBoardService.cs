
namespace Dorisoy.Pan.Services;

/// <summary>
/// 白板服务
/// </summary>
public class WhiteBoardService : IWhiteBoardService
{
    private readonly IAppState _appState;
    private readonly MakeRequest _makeRequest;
    private static string _url;
    private IMapper _mapper;
    private readonly AppSettings _appSettings;

    public WhiteBoardService(MakeRequest makeRequest, IMapper mapper, AppSettings appSettings)
    {
        _makeRequest = makeRequest;
        _mapper = mapper;
        _appSettings = appSettings;
        _url = _appSettings.GetHost();
        _appState = Locator.Current.GetService<IAppState>();
    }

    /// <summary>
    /// 加入小组验证
    /// </summary>
    /// <param name="room"></param>
    /// <param name="nickname"></param>
    /// <returns></returns>
    public async Task<JoinRoomValidationResult> ValidateJoinRoomAsync(string room, string nickname, CancellationToken calToken = default)
    {
        try
        {
            var api = RefitServiceBuilder.Build<IRoomApi>(_url, false);
            var result = await _makeRequest.Start(api.ValidateJoin(room, nickname, calToken), calToken);
            return result;
        }
        catch (Exception ex)
        {
            return new();
        }
    }

    /// <summary>
    /// 获取参与人员
    /// </summary>
    /// <param name="room"></param>
    /// <returns></returns>
    public async Task<List<Connections>> GetParticipantsAsync(string room, CancellationToken calToken = default)
    {
        try
        {
            var api = RefitServiceBuilder.Build<IRoomApi>(_url, false);
            var result = await _makeRequest.Start(api.GetParticipantsInRoom(room, calToken), calToken);
            var users = _mapper.ProjectTo<Connections>(result.AsQueryable());
            return users.ToList();
        }
        catch (Exception ex)
        {
            return new();
        }
    }

    /// <summary>
    /// 获取小组
    /// </summary>
    /// <param name="room"></param>
    /// <returns></returns>
    public async Task<List<Rooms>> GetRoomsAsync(Guid uid, CancellationToken calToken = default)
    {
        try
        {
            var api = RefitServiceBuilder.Build<IRoomApi>(_url, false);
            var result = await _makeRequest.Start(api.GetOnlineUsers(uid, calToken), calToken);
            var users = _mapper.ProjectTo<Rooms>(result.AsQueryable());
            return users.ToList();
        }
        catch (Exception ex)
        {
            return new();
        }
    }


    /// <summary>
    /// 删除小组
    /// </summary>
    /// <param name="roomid"></param>
    /// <returns></returns>
    public async Task<bool> DeleteRoomAsync(string roomid, CancellationToken calToken = default)
    {
        try
        {
            var api = RefitServiceBuilder.Build<IRoomApi>(_url, false);
            var result = await _makeRequest.Start(api.DeleteRoom(roomid, calToken), calToken);
            return result;
        }
        catch (Exception ex)
        {
            return false;
        }
    }

    /// <summary>
    /// 创建小组
    /// </summary>
    /// <param name="uid"></param>
    /// <param name="nickname"></param>
    /// <param name="room"></param>
    /// <returns></returns>
    public async Task<bool> CreateRoomAsync(Guid uid, string nickname, string room, string password, CancellationToken calToken = default)
    {
        try
        {
            var api = RefitServiceBuilder.Build<IRoomApi>(_url, false);
            var result = await _makeRequest.Start(api.CreateRoom(uid, nickname, room, password, calToken), calToken);
            return result;
        }
        catch (Exception ex)
        {
            return false;
        }
    }
}
