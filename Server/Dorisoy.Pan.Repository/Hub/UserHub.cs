using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Dorisoy.Pan.Data.Dto;
using Newtonsoft.Json;
using System.Linq;


namespace Dorisoy.Pan.Repository;

public class UserHub : Hub<IHubClient>
{
    private IConnectionMappingRepository _userInfoInMemory;
    private readonly IRoomRepository _repository;
    private readonly IRandomRoomQueue _randomRoomQueue;
    private readonly ILiveViewService _liveViewService;

    public UserHub(IConnectionMappingRepository userInfoInMemory,
        IRandomRoomQueue randomRoomQueue, 
        ILiveViewService liveViewService)
    {
        _userInfoInMemory = userInfoInMemory;
        _randomRoomQueue = randomRoomQueue;
        _liveViewService = liveViewService;
    }

    /// <summary>
    /// 用户离开
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public async Task Leave(Guid id)
    {
        var userInfo = _userInfoInMemory.GetUserInfoByName(id);
        _userInfoInMemory.Remove(userInfo);
        await Clients.AllExcept(new List<string> { Context.ConnectionId })
            .UserLeft(id);
    }

    /// <summary>
    /// 用户注销
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public async Task Logout(Guid id)
    {
        var userInfo = _userInfoInMemory.GetUserInfoByName(id);
        if (userInfo != null)
        {
            _userInfoInMemory.Remove(userInfo);
            await Clients.AllExcept(new List<string> { Context.ConnectionId })
                .UserLeft(id);
        }
    }

    /// <summary>
    /// 最近活动
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public async Task RecentActivity(Guid id)
    {
        var userInfo = _userInfoInMemory.GetUserInfoByName(id);
        if (userInfo != null)
        {
            _userInfoInMemory.Remove(userInfo);
            await Clients.AllExcept(new List<string> { Context.ConnectionId })
                .UserLeft(id);
        }
    }

    /// <summary>
    /// 强制退出
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public async Task ForceLogout(Guid id)
    {
        var userInfo = _userInfoInMemory.GetUserInfoByName(id);
        if (userInfo != null)
        {
            _userInfoInMemory.Remove(userInfo);
            await Clients.Client(userInfo.ConnectionId)
                   .ForceLogout(userInfo);

            await Clients.AllExcept(new List<string> { userInfo.ConnectionId })
                .UserLeft(id);
        }
    }

    /// <summary>
    /// 用户上线加入
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    public async Task Join(string data)
    {
        var userInfo = JsonConvert.DeserializeObject<UserInfoToken>(data);
        if (!_userInfoInMemory.AddUpdate(userInfo, Context.ConnectionId))
        {
            // 新用户:对所有连接的客户端调用方法（指定连接除外）
            await Clients.AllExcept(new List<string> { Context.ConnectionId })
                .NewOnlineUser(_userInfoInMemory.GetUserInfo(userInfo));
        }
        else
        {
            // 现有用户再次加入
        }

        await Clients.Client(Context.ConnectionId)
            .Joined(_userInfoInMemory.GetUserInfo(userInfo));

        await Clients.Client(Context.ConnectionId)
            .OnlineUsers(_userInfoInMemory.GetAllUsersExceptThis(userInfo));
    }

    /// <summary>
    /// 推送文件目录消息到指定用户
    /// </summary>
    /// <param name="folderId"></param>
    /// <param name="lstUsers"></param>
    /// <returns></returns>
    public async Task SendFolderNotification(Guid folderId, List<Guid> lstUsers)
    {
        var sendNotificationUsers = _userInfoInMemory.GetOnlineUserByList(lstUsers);
        foreach(var onlineUser in sendNotificationUsers)
        {
            await Clients.Client(onlineUser.ConnectionId)
                .FolderNotification(folderId.ToString());
        }
    }

    /// <summary>
    /// 推送消息到指定用户
    /// </summary>
    /// <param name="message"></param>
    /// <param name="uid"></param>
    /// <returns></returns>
    public Task SendDirectMessage(string message, Guid uid)
    {
        var userInfoSender = _userInfoInMemory.GetUserInfoByConnectionId(Context.ConnectionId);
        var userInfoReciever = _userInfoInMemory.GetUserInfoByName(uid);
        if (userInfoReciever != null)
            return Clients.Client(userInfoReciever.ConnectionId).OnSubscribeMessage(message, userInfoSender);
        else
            return Clients.All.OnSubscribeMessage(message, userInfoSender);
    }

    /// <summary>
    /// 推送视频到指定用户
    /// </summary>
    /// <param name="url"></param>
    /// <param name="suser"></param>
    /// <param name="tuid"></param>
    /// <param name="pushing"></param>
    /// <returns></returns>
    public Task PushVideo(string url, Guid suserId, string suser, Guid tuid, bool pushing = false, object view = null)
    {
        var userInfoSender = _userInfoInMemory.GetUserInfoByConnectionId(Context.ConnectionId);
        var userInfoReciever = _userInfoInMemory.GetUserInfoByName(tuid);
        if (userInfoReciever != null)
            return Clients
                .Client(userInfoReciever.ConnectionId)
                .OnSubscribeVideo(url, suserId, suser, userInfoSender, pushing, view);
        else
            return Task.CompletedTask;
    }

    public Task CanclePushVideo(Guid suserId)
    {
        var userInfoSender = _userInfoInMemory.GetUserInfoByConnectionId(Context.ConnectionId);
        var userInfoReciever = _userInfoInMemory.GetUserInfoByName(suserId);
        if (userInfoReciever != null)
            return Clients
                .Client(userInfoReciever.ConnectionId)
                .OnSubscribeCanclePushVideo(userInfoSender);
        else
            return Task.CompletedTask;
    }



    //Room


    /// <summary>
    /// 创建房间
    /// </summary>
    /// <param name="uid"></param>
    /// <param name="nickname"></param>
    /// <param name="room"></param>
    /// <param name="password"></param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    public async Task CreateRoom(Guid uid, string nickname, string room, string password)
    {
        try
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, room);
            await _repository.CreateRoomAsync(uid, room, false, nickname, password, Context.ConnectionId, GetIPAddress());

            await Clients.Client(Context.ConnectionId)
                .RoomCreated(room);
        }
        catch (Exception)
        {
        }
    }

    /// <summary>
    /// 获取房间
    /// </summary>
    /// <param name="uid"></param>
    /// <returns></returns>
    public async Task GetRoom(Guid uid)
    {
        var rooms = await _repository.GetRooms(uid);
        await Clients.Caller
            .GetRooms(rooms);
    }

    /// <summary>
    /// 离开房间
    /// </summary>
    /// <param name="uid"></param>
    /// <returns></returns>
    public async Task LeaveRoom(Guid uid)
    {
        _randomRoomQueue.Remove(Context.ConnectionId);
        var connectionRoom = await _repository.DisconnectAsync(Context.ConnectionId);
        if (connectionRoom != null)
        {
            await Clients.OthersInGroup(connectionRoom.Room)
                .LeftRoom(uid, connectionRoom.Nickname);
        }
        _liveViewService.Remove(Context.ConnectionId);
    }

    /// <summary>
    /// 加入房间
    /// </summary>
    /// <param name="uid"></param>
    /// <param name="nickname"></param>
    /// <param name="room"></param>
    /// <param name="ip"></param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    public async Task JoinRoom(Guid uid, string nickname, string room, string ip)
    {
        var exists = await _repository.RoomExistsAsync(room);
        if (!exists)
            throw new InvalidOperationException($"Room '{room}' does not exist.");

        var participantsInRoom = await _repository.GetActiveParticipantsInRoomAsync(room);
        if (participantsInRoom.Count > 50)
        {
            throw new InvalidOperationException($"房间 '{room}' 已经满.");
        }

        if (participantsInRoom.Select(s => s.User).Contains(nickname))
        {
            throw new InvalidOperationException($"昵称 '{nickname}' 已经被占用 '{room}'.");
        }

        await Groups.AddToGroupAsync(Context.ConnectionId, room);

        await _repository.JoinRoomAsync(uid, room, nickname, Context.ConnectionId, ip);

        await Clients.OthersInGroup(room)
            .JoinedRoom( uid, nickname, ip);
    }


    /// <summary>
    /// 随机加入房间
    /// </summary>
    /// <param name="uid"></param>
    /// <param name="nickname"></param>
    /// <returns></returns>
    public async Task JoinRandomRoom(Guid uid, string nickname)
    {
        UserInQueue? userInQueue = _randomRoomQueue.Dequeue();
        if (userInQueue == null)
        {
            _randomRoomQueue.Enqueue(Context.ConnectionId, nickname, GetIPAddress());
            return;
        }

        string room = RoomNameGenerator.Generate();

        await Groups.AddToGroupAsync(userInQueue.ConnectionId, room);
        await Groups.AddToGroupAsync(Context.ConnectionId, room);

        await _repository.CreateRoomAsync(uid, room, true, "123456", userInQueue.Nickname, userInQueue.ConnectionId, userInQueue.IpAddress);

        await _repository.JoinRoomAsync(uid, room, userInQueue.Nickname, userInQueue.ConnectionId, userInQueue.IpAddress);


        var ipAddress = GetIPAddress();
        await _repository.JoinRoomAsync(uid, room, nickname, Context.ConnectionId, ipAddress);

        await Clients.Group(room)
            .JoinRandomRoom(room);

        await _liveViewService.AddAsync(userInQueue.ConnectionId, userInQueue.IpAddress);
        await _liveViewService.AddAsync(Context.ConnectionId, ipAddress);
    }

    /// <summary>
    /// 绘制点
    /// </summary>
    /// <param name="nickname"></param>
    /// <param name="room"></param>
    /// <param name="data"></param>
    /// <returns></returns>
    public async Task DrawPoint(string nickname, string room, byte[] data)
    {
        await Clients.OthersInGroup(room)
            .DrewPoint(nickname, data);
    }

    /// <summary>
    /// 绘制线
    /// </summary>
    /// <param name="nickname"></param>
    /// <param name="room"></param>
    /// <param name="data"></param>
    /// <returns></returns>
    public async Task DrawLine(string nickname, string room, byte[] data)
    {
        await Clients.OthersInGroup(room)
            .DrawLine(nickname, data);
    }

    /// <summary>
    /// Ping检查
    /// </summary>
    /// <returns></returns>
    public Task Ping()
    {
        return Clients.Caller
            .Pong();
    }

    /// <summary>
    /// 获取本机IP地址
    /// </summary>
    /// <returns></returns>
    private string? GetIPAddress()
    {
        var httpContext = Context.GetHttpContext();
        if (httpContext == null)
        {
            return null;
        }
        return httpContext.Request.Headers["X-Forwarded-For"];
    }



    /// <summary>
    /// 发送消息
    /// </summary>
    /// <param name="recieverId"></param>
    /// <param name="message"></param>
    /// <returns></returns>
    public Task SendMessage(Guid senderId, Guid recieverId, string message)
    {
        //发送人
        var userInfoSender = _userInfoInMemory.GetUserInfoByUserId(senderId);

        //var userInfoReciever = _userInfoInMemory.GetUserInfoByName(tuid);
        //if (userInfoReciever != null)
        //    return Clients
        //        .Client(userInfoReciever.ConnectionId)

        //接受人
        var userInfoReciever = _userInfoInMemory.GetUserInfoByName(recieverId);
        if (userInfoReciever != null)
            return Clients
                .Client(userInfoReciever.ConnectionId)
                .OnReceiveMessage(message, userInfoSender);
        else
            return Clients.All.OnReceiveMessage(message, userInfoSender);
    }


    public Task Typing(string connectionId, string name)
    {
        return Clients
            .Client(connectionId)
            .TypingMessage(connectionId, name);
    }

    /// <summary>
    /// 断开用户
    /// </summary>
    public void Disconnect()
    {
        Context.Abort();
    }

    /// <summary>
    /// 获取连接用户
    /// </summary>
    /// <returns></returns>
    public Task GetConnectedUsers()
    {
        var allUsers = _userInfoInMemory.GetAllUsers();
        return Clients.Caller.GetAllUsers(allUsers);
    }


    // override
    /// <summary>
    /// 在连接后
    /// </summary>
    /// <returns></returns>
    public override async Task OnConnectedAsync()
    {
        await base.OnConnectedAsync();
        var userId = Context.GetHttpContext().Request.Query["userId"];
        var email = Context.GetHttpContext().Request.Query["email"];
        var ip = Context.GetHttpContext().Request.Query["ip"];

        if (!String.IsNullOrEmpty(userId))
        {
            // 删除列表中已存在的用户
            _userInfoInMemory.RemoveInId(Guid.Parse(userId));

            var userInfo = new UserInfoToken { ConnectionId = Context.ConnectionId, Email = email, Id = Guid.Parse(userId), IP = ip };

            // 添加用户
            _userInfoInMemory.AddUpdate(userInfo, Context.ConnectionId);

            var chatUserList = _userInfoInMemory.GetAllUsers();

            await Clients.All.OnlineUsers(chatUserList);
        }
    }


    public async void GetOnlineUsers()
    {
        var chatUserList = _userInfoInMemory.GetAllUsers();
        await Clients.All.OnlineUsers(chatUserList);
    }

    /// <summary>
    /// 在断开后
    /// </summary>
    /// <param name="ex"></param>
    /// <returns></returns>
    public override async Task OnDisconnectedAsync(Exception ex)
    {
        var userInfo = _userInfoInMemory.GetUserInfoByConnectionId(Context.ConnectionId);
        if (userInfo == null)
            return;

        //移除当前用户
        _userInfoInMemory.Remove(userInfo);

        //客户端注销
        await Clients
            .AllExcept(new List<string> { userInfo.ConnectionId }).
            UserLeft(userInfo.Id);

        //电子白板
        _randomRoomQueue.Remove(Context.ConnectionId);

        if (_repository != null)
        {
            var connectionRoom = await _repository.DisconnectAsync(Context.ConnectionId);
            if (connectionRoom != null)
            {
                await Clients.OthersInGroup(connectionRoom.Room)
                    .LeftRoom(userInfo.Id, connectionRoom.Nickname);
            }
        }

        _liveViewService.Remove(Context.ConnectionId);

        await base.OnDisconnectedAsync(ex);
    }

}

