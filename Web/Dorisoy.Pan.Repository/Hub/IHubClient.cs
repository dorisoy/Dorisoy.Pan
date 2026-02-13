using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Dorisoy.Pan.Data.Dto;

namespace Dorisoy.Pan.Repository
{
    public interface IHubClient
    {
        Task UserLeft(Guid id);

        Task NewOnlineUser(UserInfoToken userInfo);

        Task Joined(UserInfoToken userInfo);

        Task OnlineUsers(IEnumerable<UserInfoToken> userInfo);

        Task Logout(UserInfoToken userInfo);

        Task ForceLogout(UserInfoToken userInfo);

        Task SendDM(string message, UserInfoToken userInfo);

        Task FolderNotification(string folderId);

        Task RemoveFolderNotification(string folderId);


    }
}
