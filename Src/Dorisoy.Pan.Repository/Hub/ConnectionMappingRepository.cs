using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dorisoy.Pan.Data.Dto;
using Microsoft.AspNetCore.SignalR;

namespace Dorisoy.Pan.Repository
{
    public class ConnectionMappingRepository : IConnectionMappingRepository
    {
        IHubContext<UserHub, IHubClient> _hubContext;
        public ConnectionMappingRepository(IHubContext<UserHub, IHubClient> hubContext)
        {
            _hubContext = hubContext;
        }
        private ConcurrentDictionary<Guid, UserInfoToken> _onlineUser { get; set; } = new ConcurrentDictionary<Guid, UserInfoToken>();
        public bool AddUpdate(UserInfoToken tempUserInfo, string connectionId)
        {
            var userAlreadyExists = _onlineUser.ContainsKey(tempUserInfo.Id);

            var userInfo = new UserInfoToken
            {
                Id = tempUserInfo.Id,
                ConnectionId = connectionId,
                Email = tempUserInfo.Email
            };

            _onlineUser.AddOrUpdate(tempUserInfo.Id, userInfo, (key, value) => userInfo);

            return userAlreadyExists;
        }
        public void Remove(UserInfoToken tempUserInfo)
        {
            UserInfoToken userInfo;
            _onlineUser.TryRemove(tempUserInfo.Id, out userInfo);
        }
        public IEnumerable<UserInfoToken> GetAllUsersExceptThis(UserInfoToken tempUserInfo)
        {
            return _onlineUser.Values.Where(item => item.Id != tempUserInfo.Id);
        }
        public UserInfoToken GetUserInfo(UserInfoToken tempUserInfo)
        {
            UserInfoToken user;
            _onlineUser.TryGetValue(tempUserInfo.Id, out user);
            return user;
        }
        public UserInfoToken GetUserInfoByName(Guid id)
        {
            UserInfoToken user;
            _onlineUser.TryGetValue(id, out user);
            return user;
        }
        public UserInfoToken GetUserInfoByConnectionId(string connectionId)
        {
            foreach (var onlineUser in _onlineUser)
            {
                var user = onlineUser.Value;
                if (user.ConnectionId == connectionId)
                {
                    return user;
                }
            }
            return null;
        }
        public List<UserInfoToken> GetOnlineUserByList(List<Guid> users)
        {
            var lstOnlineUsers = new List<UserInfoToken>();
            foreach (var onlineUser in _onlineUser)
            {
                if(users.Any(c=>c==onlineUser.Value.Id) )
                {
                    lstOnlineUsers.Add(onlineUser.Value);
                }
            }
            return lstOnlineUsers;
        }
        public async Task SendFolderNotification(List<Guid> users, Guid folderId )
        {
            var userNotificationList = GetOnlineUserByList(users);
            if (userNotificationList.Count() > 0)
            {
                foreach (var userNotification in userNotificationList)
                {
                    await _hubContext.Clients.Client(userNotification.ConnectionId).FolderNotification(folderId.ToString());
                }
            }
        }
        public async Task RemovedFolderNotification(List<Guid> users, Guid folderId)
        {
            var userNotificationList = GetOnlineUserByList(users);
            if (userNotificationList.Count() > 0)
            {
                foreach (var userNotification in userNotificationList)
                {
                    await _hubContext.Clients.Client(userNotification.ConnectionId).RemoveFolderNotification(folderId.ToString());
                }
            }
        }

    }
}
