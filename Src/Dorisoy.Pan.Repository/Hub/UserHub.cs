using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Dorisoy.Pan.Data.Dto;

namespace Dorisoy.Pan.Repository
{
    public class UserHub : Hub<IHubClient>
    {
        private IConnectionMappingRepository _userInfoInMemory;

        public UserHub(IConnectionMappingRepository userInfoInMemory)
        {
            _userInfoInMemory = userInfoInMemory;
        }

        public async Task Leave(Guid id)
        {
            var userInfo = _userInfoInMemory.GetUserInfoByName(id);
            _userInfoInMemory.Remove(userInfo);
            await Clients.AllExcept(new List<string> { Context.ConnectionId })
                .UserLeft(id);
        }
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

        public async Task Join(UserInfoToken userInfo)
        {
            if (!_userInfoInMemory.AddUpdate(userInfo, Context.ConnectionId))
            {
                // new user
                await Clients.AllExcept(new List<string> { Context.ConnectionId })
                    .NewOnlineUser(_userInfoInMemory.GetUserInfo(userInfo));
            }
            else
            {
                // existing user joined again
            }

            await Clients.Client(Context.ConnectionId)
                .Joined(_userInfoInMemory.GetUserInfo(userInfo));

            await Clients.Client(Context.ConnectionId)
                .OnlineUsers(_userInfoInMemory.GetAllUsersExceptThis(userInfo));
        }

        public async Task SendFolderNotification(Guid folderId, List<Guid> lstUsers)
        {
            var sendNotificationUsers = _userInfoInMemory.GetOnlineUserByList(lstUsers);

            foreach(var onlineUser in sendNotificationUsers)
            {
                await Clients.Client(onlineUser.ConnectionId)
                    .FolderNotification(folderId.ToString());
            }
        }

        public Task SendDirectMessage(string message, Guid targetUserName)
        {
            var userInfoSender = _userInfoInMemory.GetUserInfoByConnectionId(Context.ConnectionId);
            var userInfoReciever = _userInfoInMemory.GetUserInfoByName(targetUserName);
            return Clients.Client(userInfoReciever.ConnectionId).SendDM(message, userInfoSender);
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            var userInfo = _userInfoInMemory.GetUserInfoByConnectionId(Context.ConnectionId);
            if (userInfo == null)
                return;
            _userInfoInMemory.Remove(userInfo);
            await Clients.AllExcept(new List<string> { userInfo.ConnectionId }).UserLeft(userInfo.Id);
        }
    }
}
