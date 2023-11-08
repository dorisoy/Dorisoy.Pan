using System.Diagnostics;
using AutoMapper;
using Avalonia;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.DependencyInjection;
using Dorisoy.PanClient.Utils;

namespace Dorisoy.PanClient.Services;

/// <summary>
/// 白板服务
/// </summary>
public class WhiteBoardService : IWhiteBoardService
{
    private readonly IAppState _appState;
    private readonly SourceCache<Rooms, Guid> _items;
    private readonly MakeRequest _makeRequest;
    private readonly string _url;
    private IMapper _mapper;

    public IObservable<IChangeSet<Rooms, Guid>> Connect() => _items.Connect();
    public HubConnection Connection { get; private set; }

    public WhiteBoardService(MakeRequest makeRequest, AppSettings appSettings)
    {
        try
        {
            _appState = Locator.Current.GetService<IAppState>();
            _makeRequest = makeRequest;
            _mapper = Locator.Current.GetService<IMapper>();

            _items = new SourceCache<Rooms, Guid>(e => e.Id);
            _url = appSettings.HostUrl.EndsWith("/") ? (appSettings.HostUrl + "api") : (appSettings.HostUrl + "/api");
            var hub = appSettings.HostUrl.EndsWith("/") ? (appSettings.HostUrl + "actionHub") : (appSettings.HostUrl + "/actionHub");

            //自动连接actionHub
            Connection = new HubConnectionBuilder()
              .WithUrl(hub)
              .WithAutomaticReconnect()
              .AddMessagePackProtocol()
              .Build();
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.Message);
        }
    }


    public async Task GetRoomsAsync()
    {
        try
        {
            Connection.On<List<Rooms>>("GetRooms", (rooms) =>
            {
                _items.AddOrUpdate(rooms);
            });

            _items.Clear();

            if (Connection.State == HubConnectionState.Disconnected)
                await Connection.StartAsync();

            await Connection.InvokeAsync("GetRoom", Globals.CurrentUser.Id);
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.Message);
        }
    }


    public async Task CreateRoomAsync()
    {
        try
        {
            Connection.On<string>("RoomCreated", (room) =>
            {
                _appState.Room = room;
            });

            if (Connection.State == HubConnectionState.Disconnected)
                await Connection.StartAsync();

            await Connection.InvokeAsync("CreateRoom",
                Globals.CurrentUser.Id,
                _appState.Nickname,
                _appState.Room);

            await GetRoomsAsync();
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.Message);
        }
    }


    public async Task LeaveRoomAsync(Guid uid)
    {
        try
        {
            if (Connection.State == HubConnectionState.Disconnected)
                await Connection.StartAsync();

            //LeaveRoom
            await Connection.InvokeAsync("LeaveRoom", Globals.CurrentUser.Id);
            await GetRoomsAsync();
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.Message);
        }
    }

    /// <summary>
    /// 加入房间
    /// </summary>
    /// <returns></returns>
    public async Task JoinRoomAsync()
    {
        try
        {
            if (Connection.State == HubConnectionState.Disconnected)
                await Connection.StartAsync();

            //JoinedRoom
            await Connection.InvokeAsync("JoinRoom", Globals.CurrentUser.Id, _appState.Nickname, _appState.Room, Utilities.GetLocalIP());

            await GetRoomsAsync();
        }
        catch (Exception ex)
        {
            //{"Unable to complete handshake with the server due to an error: The protocol 'messagepack' is not supported."}
            Debug.WriteLine(ex.Message);
        }
    }

    public async Task JoinRandomRoomAsync()
    {
        try
        {
            if (Connection.State == HubConnectionState.Disconnected)
                await Connection.StartAsync();

            await Connection.InvokeAsync("JoinRandomRoom", _appState.Nickname);
            await GetRoomsAsync();
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.Message);
        }
    }

    public async Task DrawPointAsync(double x, double y)
    {
        try
        {
            var data = PayloadConverter.ToBytes(x, y, _appState.BrushSettings.BrushThickness, _appState.BrushSettings.BrushColor);
            await Connection.InvokeAsync("DrawPoint", _appState.Nickname, _appState.Room, data);
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.Message);
        }
    }

    public async Task DrawLineAsync(List<Point> points)
    {
        try
        {
            var data = PayloadConverter.ToBytes(points, _appState.BrushSettings.BrushThickness, _appState.BrushSettings.BrushColor);
            await Connection.InvokeAsync("DrawLine", _appState.Nickname, _appState.Room, data);
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.Message);
        }
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
