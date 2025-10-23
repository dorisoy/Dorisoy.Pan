using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Dorisoy.Pan.Data.Dto;

namespace Dorisoy.Pan.Repository;

public interface IHubClient
{
    Task UserLeft(Guid id);
    Task NewOnlineUser(UserInfoToken userInfo);
    Task Joined(UserInfoToken userInfo);
    Task OnlineUsers(IEnumerable<UserInfoToken> userInfo);
    Task GetOnlineUsers();
    Task Logout(UserInfoToken userInfo);
    Task ForceLogout(UserInfoToken userInfo);
    Task OnSubscribeMessage(string message, UserInfoToken userInfo);
    Task OnSubscribeVideo(string url,Guid suserId, string suser, UserInfoToken userInfo, bool pushing = false, object view = null);
    Task OnSubscribeCanclePushVideo(UserInfoToken suser);
    Task FolderNotification(string folderId);
    Task RemoveFolderNotification(string folderId);


    //用于白板教室协作RoomCreated
    Task RoomCreated(string room);
    Task LeftRoom(Guid uid,string room);
    Task GetRooms(IList<Rooms> rooms);
    Task JoinedRoom(Guid uid, string nickname, string ip);
    Task JoinRandomRoom(string room);
    Task DrewPoint(string nickname, byte[] data);
    Task DrawLine(string nickname,  byte[] data);
    Task Pong();


    //用于聊天服务 
    Task OnReceiveMessage(string message, UserInfoToken userInfo);
    Task TypingMessage(string connectionId, string name);
    Task GetAllUsers(IEnumerable<UserInfoToken> users);

}
